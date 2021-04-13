using MemeSearch.Logic.Models;
using System.Collections.Generic;

namespace MemeSearch.Logic.Interfaces
{
    public interface ISearchService
    {
        IEnumerable<Meme> StandardSearch(string query, int start = 0);
        IEnumerable<Meme> AdvancedSearch(SearchParameters parameters, string query, int start = 0);
    }
}
