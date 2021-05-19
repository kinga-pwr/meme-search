using MemeSearch.Logic.Interfaces;
using MemeSearch.Logic.Models;
using Nest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Text.RegularExpressions;

namespace MemeSearch.Logic.Services
{
    public class SearchService : ISearchService
    {
        private readonly IElasticClient _elasticClient;
        private readonly IImageDetectService _imageDetectService;

        public SearchService(IElasticClient elasticClient, IImageDetectService imageDetectService)
        {
            _elasticClient = elasticClient;
            _imageDetectService = imageDetectService;
        }

        public TextSearchResult StandardSearch(string query, int results, int start)
        {
            return Search(GetStandardQuery, query, new SearchParameters(), results, start);
        }

        public TextSearchResult AdvancedSearch(SearchParameters parameters, string query, int results, int start)
        {
            return Search(GetAdvancedQuery, query, parameters, results, start);
        }

        public ImageSearchResult ImageSearch(ImageSearchParameters parameters, int results, int start)
        {
            var imageTags = _imageDetectService.GetImageTags(parameters.Url).Result;
            
            if (imageTags == null) return null;
            if (string.IsNullOrWhiteSpace(imageTags)) return new ImageSearchResult(); 

            if (!parameters.SearchSimilarities)
            {
                imageTags = $"\"{imageTags.Replace(" AND ", " ")}\"";
            }

            return new ImageSearchResult(Search(GetAdvancedQuery, imageTags, parameters, results, start))
            {
                Tags = imageTags
            };
        }

        public bool CanImageSearch() => _imageDetectService.IsAvailable;

        private static QueryContainer GetStandardQuery(QueryContainerDescriptor<Meme> q, string query, SearchParameters parameters)
        {
            return ParseQuery(q, query, parameters.Fields);
        }

        private static QueryContainer GetAdvancedQuery(QueryContainerDescriptor<Meme> q, string query, SearchParameters parameters)
        {
            var queryParts = new List<QueryContainer>();

            if (!string.IsNullOrEmpty(query))
            {
                // TEXT QUERY (with logic) - TITLE, CONTENT, CATEGORY, DETAILS AND IMAGE
                queryParts.Add(ParseQuery(q, query, parameters.Fields));
            }

            // STATUS
            if (parameters.Status?.Any() ?? false)
            {
                queryParts.Add(q.Bool(f => f.Should(parameters.Status.Select(s => q.Term(t => t.Status, s)).ToArray()).MinimumShouldMatch(1)));
            }

            // CATEGORY
            if (parameters.Category?.Any() ?? false)
            {
                queryParts.Add(q.Bool(f => f.Should(parameters.Category.Select(c => q.Match(m => m.Field(f => f.Category).Query(c))).ToArray()).MinimumShouldMatch(1)));
            }

            // DETAILS
            if (parameters.Details?.Any() ?? false)
            {
                queryParts.Add(q.Bool(f => f.Should(parameters.Details.Select(d => q.Match(m => m.Field(f => f.Details).Query(d))).ToArray()).MinimumShouldMatch(1)));
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

        #region Search
        private TextSearchResult Search(Func<QueryContainerDescriptor<Meme>, string, SearchParameters, QueryContainer> getQuery, 
            string query, SearchParameters parameters, int results, int start)
        {
            var result = _elasticClient.Search<Meme>(s =>
                s.From(start)
                .Size(results)
                .TrackScores(true)
                .Query(q => getQuery(q, query, parameters))
                .Highlight(h => h
                    .PreTags("<mark>")
                    .PostTags("</mark>")
                    .Encoder(HighlighterEncoder.Html)
                    .HighlightQuery(q => HighlightSearch(q, query.ToLower(), parameters))
                    .Fields(f => f.Field(fl => fl.ContentToSearch)
                                    .Type("plain")
                                    .ForceSource()
                                    .FragmentSize(150)
                                    .Fragmenter(HighlighterFragmenter.Span)
                                    .NumberOfFragments(2)
                                    .NoMatchSize(150)))
                .Sort(s => GetSorting(s, parameters.Sort, parameters.SortAsc)));

            return new TextSearchResult()
            {
                NumberOfResults = result.Total,
                Memes = result.Documents.Select((d, i) => new MemeDto()
                {
                    Title = d.Title,
                    Content = ClearContent(d.Content),
                    ImageUrl = d.ImageUrl,
                    ImageTags = d.ImageTags,
                    Status = d.Status,
                    Details = d.Details,
                    Year = d.Year,
                    Category = d.Category,
                    Url = d.Url,
                    ContentHighlight = result.Hits.Select(h => h.Highlight.Any() ? string.Join(" ... ", h.Highlight.First().Value) : null).ElementAt(i)
                })
            };
        }    

        private string ClearContent(string content)
        {
            while(content.Contains("\n\n\n\n"))
            {
                content = content.Replace("\n\n\n\n", string.Empty);
            }
            content = content.Replace("\n", "<br/>");
            return content;
        }

        private static IPromise<IList<ISort>> GetSorting(SortDescriptor<Meme> s, string sort, bool sortAsc)
        {
            if (sort != null)
            {
                return s.Field(f => f.Year, sortAsc ? SortOrder.Ascending : SortOrder.Descending);
            }

            return s.Descending(SortSpecialField.Score);
        }
        #endregion Search

        #region MainQuery
        private static QueryContainer ParseQuery(QueryContainerDescriptor<Meme> q, string query,
            IEnumerable<string> fields)
        {
            return ParseExpression(q, query, 0, fields).container;
        }
        
        private static (QueryContainer container, int position) ParseExpression(QueryContainerDescriptor<Meme> q, string query,
            int position, IEnumerable<string> fields)
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

        private static QueryContainer SearchQuery(QueryContainerDescriptor<Meme> q, string searchWord, IEnumerable<string> fields)
        {
            searchWord = searchWord.Trim();

            var result = new List<QueryContainer>();

            var type = TextQueryType.BestFields;
            if (IsPhrase(searchWord))
            {
                type = TextQueryType.Phrase;
            }

            if (fields.Any(f => SearchableTextFields.Contains(f)))
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
            if (!parameters.Fields.Contains("Content") && !parameters.Fields.Contains("Image")) return new QueryContainer();

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
                    .Field(p => p.ContentToSearch)
                    .Analyzer("standard")
                    .Query(searchWord.Trim('"'))
                    .Slop(2))
                : q.Term(t => t.ContentToSearch, searchWord);
        }
        #endregion

        #region Fields
        private static readonly List<string> SearchableTextFields = new List<string>() { "Content", "Category", "Details", "Image" };

        private static void AppendKeywordFields(QueryContainerDescriptor<Meme> q, IEnumerable<string> fields, string searchWord, List<QueryContainer> result)
        {
            if (fields.Contains("Title"))
            {
                result.Add(q.Wildcard(t => t.Title, $"*{searchWord}*"));
            }
        }

        private static IPromise<Fields> GetTextFields(FieldsDescriptor<Meme> f, IEnumerable<string> fields)
        {
            foreach (var field in fields.Where(f => SearchableTextFields.Contains(f)))
            {
                f = f.Field(GetField(field));
            }
            return f;
        }

        private static Expression<Func<Meme, string>> GetField(string name)
        {
            return name switch
            {
                "Title" => f => f.Title,
                "Content" => f => f.ContentToSearch,
                "Category" => f => f.Category,
                "Details" => f => f.Details,
                "Image" => f => f.ImageTags,
                _ => null,
            };
        }

        private static bool IsPhrase(string searchWord) => searchWord.StartsWith('"') && searchWord.EndsWith('"');
        #endregion
    }
}
