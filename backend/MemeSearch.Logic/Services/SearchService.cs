using MemeSearch.Logic.Interfaces;
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

        public IEnumerable<Meme> Search(string query, int start = 0)
        {
            if (string.IsNullOrWhiteSpace(query)) return null;

            var result = _elasticClient.Search<Meme>(s => 
                s.From(start)
                .Size(20)
                .TrackScores(true)
                .Query(q => ParseQuery(q, query, GetAllSearchableFields))
                .Highlight(h => h
                    .PreTags("<highlight>")
                    .PostTags("</highlight>")
                    .Encoder(HighlighterEncoder.Html)
                    .HighlightQuery(q => HighlightSearch(q, query))
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

        public IEnumerable<Meme> AdvancedSearch()
        {
            // todo: finish this
            return _elasticClient.Search<Meme>(s =>
                s.Query(q => q.DateRange(d => d.Field(f => f.Year)
                            .GreaterThanOrEquals(new DateTime())
                            .LessThanOrEquals(new DateTime())))
                .Sort(sort => sort.Descending(SortSpecialField.Score)))
                .Documents;
        }

        #region MainQuery
        private static QueryContainer ParseQuery(QueryContainerDescriptor<Meme> q, string query,
            Func<FieldsDescriptor<Meme>, IPromise<Fields>> getSearchFields)
        {
            return ParseExpression(q, query, 0, getSearchFields).container;
        }
        
        private static (QueryContainer container, int position) ParseExpression(QueryContainerDescriptor<Meme> q, string query,
            int position, Func<FieldsDescriptor<Meme>, IPromise<Fields>> getSearchFields)
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
                        executionStack.Push(SearchQuery(q, expression, getSearchFields));
                        i += expression.Length;
                        break;
                    
                    case ')':
                        return CalculateFinalResult();
                    
                    case '(':
                        var subExpression = ParseExpression(q, query, i + 1, getSearchFields);
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
                    executionStack.Push(SearchQuery(q, currentWord.ToString(), getSearchFields));
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
                    executionStack.Push(SearchQuery(q, currentWord.ToString(), getSearchFields));
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

        private static QueryContainer SearchQuery(QueryContainerDescriptor<Meme> q, string searchWord, Func<FieldsDescriptor<Meme>, IPromise<Fields>> getSearchFields)
        {
            var type = TextQueryType.BestFields;

            searchWord = searchWord.Trim();
            if (IsPhrase(searchWord))
            {
                type = TextQueryType.Phrase;
            }

            return q.MultiMatch(mm => 
                    mm.Fields(f => getSearchFields(f))
                    .Query(searchWord)
                    .Type(type)
                    .Lenient());
        }
        #endregion

        #region Highlight
        private static QueryContainer HighlightSearch(QueryContainerDescriptor<Meme> q, string query)
        {
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
        private static IPromise<Fields> GetAllSearchableFields(FieldsDescriptor<Meme> f)
        {
            return f.Field(fl => fl.Title)
                    .Field(fl => fl.Content)
                    .Field(fl => fl.Category)
                    .Field(fl => fl.Details);
        }

        private static bool IsPhrase(string searchWord) => searchWord.StartsWith('"') && searchWord.EndsWith('"');
        #endregion
    }
}
