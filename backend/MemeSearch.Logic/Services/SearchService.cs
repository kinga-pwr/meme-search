using MemeSearch.Logic.Interfaces;
using MemeSearch.Logic.Models;
using Nest;
using System;
using System.Collections.Generic;
using System.Linq;

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
                .Query(q => GetSearchQuery(q, query, GetAllSearchableFields))
                .Highlight(h => h
                    .PreTags("<highlight>")
                    .PostTags("</highlight>")
                    .Encoder(HighlighterEncoder.Html)
                    .HighlightQuery(q => GetSearchQuery(q, query, GetHighlightFields))
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


        private static QueryContainer GetSearchQuery(QueryContainerDescriptor<Meme> q, string query,
            Func<FieldsDescriptor<Meme>, IPromise<Fields>> getSearchFields)
        {
            var searchWords = query.Split("OR", StringSplitOptions.RemoveEmptyEntries).ToList();
            var result = new List<QueryContainer>();

            foreach (var searchWord in searchWords)
            {
                result.Add(SearchQuery(q, searchWord, getSearchFields));
            }

            return q.Bool(m => m.Should(result.ToArray()).MinimumShouldMatch(1));
        }

        private static QueryContainer SearchQuery(QueryContainerDescriptor<Meme> q, string searchWord, Func<FieldsDescriptor<Meme>, IPromise<Fields>> getSearchFields)
        {
            var type = TextQueryType.BestFields;

            searchWord = searchWord.Trim();
            if (searchWord.StartsWith('"') && searchWord.EndsWith('"'))
            {
                type = TextQueryType.Phrase;
            }

            return q.MultiMatch(mm => 
                    mm.Fields(f => getSearchFields(f))
                    .Query(searchWord)
                    .Type(type)
                    .Lenient());
        }

        #region Fields
        private static IPromise<Fields> GetAllSearchableFields(FieldsDescriptor<Meme> f)
        {
            return f.Field(fl => fl.Title)
                    .Field(fl => fl.Content)
                    .Field(fl => fl.Category)
                    .Field(fl => fl.Details);
        }

        private static IPromise<Fields> GetHighlightFields(FieldsDescriptor<Meme> f)
        {
            return f.Field(fl => fl.Category);
        }
        #endregion
    }
}
