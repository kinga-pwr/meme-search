using MemeSearch.Logic.Models;
using System.Collections.Generic;

namespace MemeSearch.Logic.Interfaces
{
    public interface ISearchService
    {
        IEnumerable<Meme> StandardSearch(string query, int results, int start);
        IEnumerable<Meme> AdvancedSearch(SearchParameters parameters, string query, int results, int start);
        IEnumerable<Meme> ImageSearch(ImageSearchParameters parameters, int results, int start);
        bool CanImageSearch();
    }
}
