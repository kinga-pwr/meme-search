using MemeSearch.Logic.Models;

namespace MemeSearch.Logic.Interfaces
{
    public interface ISearchService
    {
        TextSearchResult StandardSearch(string query, int results, int start);
        TextSearchResult AdvancedSearch(SearchParameters parameters, string query, int results, int start);
        ImageSearchResult ImageSearch(ImageSearchParameters parameters, int results, int start);
        bool CanImageSearch();
    }
}
