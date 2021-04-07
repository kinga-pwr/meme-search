using MemeSearch.Logic.Models;
using System.Collections.Generic;

namespace MemeSearch.Logic.Interfaces
{
    public interface ISearchService
    {
        public IEnumerable<Meme> Search(string query, int start = 0);
    }
}
