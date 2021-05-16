using MemeSearch.Logic.Models;
using System.Collections.Generic;

namespace MemeSearch.Logic.Interfaces
{
    public interface ISearchService
    {
        IEnumerable<MemeDto> StandardSearch(string query, int results, int start);
        IEnumerable<MemeDto> AdvancedSearch(SearchParameters parameters, string query, int results, int start);
        ImageSearchResult ImageSearch(ImageSearchParameters parameters, int results, int start);
        bool CanImageSearch();
    }
}
