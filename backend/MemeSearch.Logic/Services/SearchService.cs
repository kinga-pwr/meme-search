using MemeSearch.Logic.Interfaces;
using MemeSearch.Logic.Models;
using Nest;
using Newtonsoft.Json;
using System.IO;

namespace MemeSearch.Logic.Services
{
    public class SearchService : ISearchService
    {
        private readonly IElasticClient _elasticClient;

        public SearchService(IElasticClient elasticClient)
        {
            _elasticClient = elasticClient;
        }
    }
}
