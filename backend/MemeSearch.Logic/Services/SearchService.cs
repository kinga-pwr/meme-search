﻿using MemeSearch.Logic.Interfaces;
using MemeSearch.Logic.Models;
using Nest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace MemeSearch.Logic.Services
{
    public class SearchService : ISearchService
    {
        private readonly IElasticClient _elasticClient;

        public SearchService(IElasticClient elasticClient)
        {
            _elasticClient = elasticClient;
        }

        public IEnumerable<Meme> StandardSearch(string query, int start = 0)
        {
            return Search(GetStandardQuery, query, new SearchParameters(), start);
        }

        public IEnumerable<Meme> AdvancedSearch(SearchParameters parameters, string query, int start = 0)
        {
            return Search(GetAdvancedQuery, query, parameters, start);
        }

        private static QueryContainer GetStandardQuery(QueryContainerDescriptor<Meme> q, string query, SearchParameters parameters)
        {
            return ParseQuery(q, query, parameters.Fields);
        }

        private static QueryContainer GetAdvancedQuery(QueryContainerDescriptor<Meme> q, string query, SearchParameters parameters)
        {
            // TEXT QUERY (with logic) - TITLE, CONTENT, CATEGORY, DETAILS AND IMAGE
            var queryParts = new List<QueryContainer>
            {
                ParseQuery(q, query, parameters.Fields)
            };

            // STATUS
            if (parameters.Status != null)
            {
                queryParts.Add(q.Term(t => t.Status, parameters.Status));
            }

            // YEAR FROM AND TO
            if (parameters.YearFrom.HasValue || parameters.YearTo.HasValue)
            {
                queryParts.Add(q.Range(r => r.Field(p => p.Year)
                .GreaterThanOrEquals(parameters.YearFrom ?? int.MinValue)
                .LessThanOrEquals(parameters.YearTo ?? int.MaxValue)));
            }

            return q.Bool(b => b.Must(queryParts.ToArray()));
        }

        private IEnumerable<Meme> Search(Func<QueryContainerDescriptor<Meme>, string, SearchParameters, QueryContainer> getQuery, 
            string query, SearchParameters parameters, int start)
        {
            var result = _elasticClient.Search<Meme>(s =>
                s.From(start)
                .Size(20)
                .TrackScores(true)
                .Query(q => getQuery(q, query, parameters))
                .Highlight(h => h
                    .PreTags("<highlight>")
                    .PostTags("</highlight>")
                    .Encoder(HighlighterEncoder.Html)
                    .HighlightQuery(q => HighlightSearch(q, query, parameters))
                    .Fields(f => f.Field(fl => fl.Content)
                                    .Type("plain")
                                    .ForceSource()
                                    .FragmentSize(150)
                                    .Fragmenter(HighlighterFragmenter.Span)
                                    .NumberOfFragments(3)
                                    .NoMatchSize(150)))
                .Sort(sort => sort.Descending(SortSpecialField.Score)));

            return result.Documents.Select((d, idx) =>
            {
                d.ContentHighlight = result.Hits.Select(h => h.Highlight.Any() ? string.Join(" ... ", h.Highlight.First().Value) : null).FirstOrDefault();
                return d;
            });
        }

        #region MainQuery
        private static QueryContainer ParseQuery(QueryContainerDescriptor<Meme> q, string query,
            string[] fields)
        {
            return ParseExpression(q, query, 0, fields).container;
        }
        
        private static (QueryContainer container, int position) ParseExpression(QueryContainerDescriptor<Meme> q, string query,
            int position, string[] fields)
        {
            var executionStack = new Stack<QueryContainer>();
            var lastOperator = "";
            var currentWord = new StringBuilder();

            int i = position;
            while(i < query.Length)
            {
                switch (query[i])
                {
                    case '"':
                        var expression = ReadExactExpression(query, i);
                        executionStack.Push(SearchQuery(q, expression, fields));
                        i += expression.Length;
                        break;
                    
                    case ')':
                        return CalculateFinalResult();
                    
                    case '(':
                        var subExpression = ParseExpression(q, query, i + 1, fields);
                        executionStack.Push(subExpression.container);
                        i = subExpression.position;
                        break;
                    
                    case 'O' when i < query.Length - 2 && query.Substring(i, 3) == "OR ":
                        HandleOperator("OR", ExecuteANDCalculation);
                        break;
                    
                    case 'A' when i < query.Length - 3 && query.Substring(i, 4) == "AND ":
                        HandleOperator("AND", ExecuteORCalculation);
                        break;
                    
                    default:
                        currentWord.Append(query[i]);
                        i++;
                        break;
                }
            }

            return CalculateFinalResult();

            (QueryContainer container, int position) CalculateFinalResult()
            {
                if (currentWord.Length > 0)
                {
                    executionStack.Push(SearchQuery(q, currentWord.ToString(), fields));
                }

                if (executionStack.Count > 1)
                {
                    var finalResult = lastOperator == "OR"
                            ? q.Bool(m => m.Should(executionStack.ToArray()).MinimumShouldMatch(1))
                            : q.Bool(m => m.Must(executionStack.ToArray()));
                    return (finalResult, i + 1);
                }
                else if (executionStack.Count == 1)
                {
                    return (executionStack.Pop(), i + 1);
                }
                else throw new ArgumentException("Wrong syntax");
            }

            void HandleOperator(string operatorName, Func<QueryContainer> oppositeOperatorExecution)
            {
                if (currentWord.Length > 0)
                {
                    executionStack.Push(SearchQuery(q, currentWord.ToString(), fields));
                }

                if (!string.IsNullOrEmpty(lastOperator) && lastOperator != operatorName)
                {
                    var previousResult = oppositeOperatorExecution();
                    executionStack.Clear();
                    executionStack.Push(previousResult);
                }

                lastOperator = operatorName;
                currentWord.Clear();
                i += operatorName.Length + 1;
            }

            QueryContainer ExecuteORCalculation() => q.Bool(m => m.Should(executionStack.ToArray()).MinimumShouldMatch(1));
            QueryContainer ExecuteANDCalculation() => q.Bool(m => m.Must(executionStack.ToArray()));
        }

        private static string ReadExactExpression(string query, int position)
        {
            var endOfExpression = query.IndexOf('"', position + 1);

            if (endOfExpression == -1)
                throw new ArgumentException("Exact expression missing closing paranthesis");

            return query.Substring(position, endOfExpression - position + 1);  
        }

        private static QueryContainer SearchQuery(QueryContainerDescriptor<Meme> q, string searchWord, string[] fields)
        {
            searchWord = searchWord.Trim();

            var result = new List<QueryContainer>();

            var type = TextQueryType.BestFields;
            if (IsPhrase(searchWord))
            {
                type = TextQueryType.Phrase;
            }

            if (fields.Contains("Content") || fields.Contains("Category")
                || fields.Contains("Image"))
            {
                result.Add(q.MultiMatch(mm =>
                    mm.Fields(f => GetTextFields(f, fields))
                    .Query(searchWord)
                    .Type(type)));
            }

            AppendKeywordFields(q, fields, searchWord, result);

            return q.Bool(b => b.Should(result.ToArray()).MinimumShouldMatch(1));
        }
        #endregion

        #region Highlight
        private static QueryContainer HighlightSearch(QueryContainerDescriptor<Meme> q, string query, SearchParameters parameters)
        {
            if (!parameters.Fields.Contains("Content")) return new QueryContainer();

            var sb = new StringBuilder(query);

            sb.Replace("AND", string.Empty);
            sb.Replace("OR", string.Empty);
            sb.Replace("(", string.Empty);
            sb.Replace(")", string.Empty);

            var clean = Regex.Replace(sb.ToString(), "\"(.*?)\"", m => m.Value.Replace(" ", "&nbsp;"));

            var parts = clean.Split(' ', StringSplitOptions.RemoveEmptyEntries);

            return q.Bool(m => m.Should(parts.Select(p => GetHighlightCondition(q, p.Replace("&nbsp;", " "))).ToArray()).MinimumShouldMatch(1));
        }

        private static QueryContainer GetHighlightCondition(QueryContainerDescriptor<Meme> q, string searchWord)
        {
            return IsPhrase(searchWord) 
                ? q.MatchPhrase(c => c
                    .Field(p => p.Content)
                    .Analyzer("standard")
                    .Query(searchWord.Trim('"'))
                    .Slop(2))
                : q.Term(t => t.Content, searchWord);
        }
        #endregion

        #region Fields
        private static void AppendKeywordFields(QueryContainerDescriptor<Meme> q, string[] fields, string searchWord, List<QueryContainer> result)
        {
            if (fields.Contains("Title"))
            {
                result.Add(q.Wildcard(t => t.Title, $"*{searchWord}*"));
            }
        }

        private static IPromise<Fields> GetTextFields(FieldsDescriptor<Meme> f, string[] fields)
        {
            if (fields.Contains("Content"))
            {
                f = f.Field(fl => fl.Content);
            }
            if (fields.Contains("Category"))
            {
                f = f.Field(fl => fl.Category);
            }
            if (fields.Contains("Details"))
            {
                f = f.Field(fl => fl.Category);
            }
            if (fields.Contains("Image"))
            {
                f = f.Field(fl => fl.ImageTags);
            }
            return f;
        }

        private static bool IsPhrase(string searchWord) => searchWord.StartsWith('"') && searchWord.EndsWith('"');
        #endregion
    }
}
