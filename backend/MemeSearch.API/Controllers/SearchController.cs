using MemeSearch.Logic.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace MemeSearch.API.Controllers
{
    [ApiController]
    public class SearchController : ControllerBase
    {
        private readonly ISearchService _searchService;

        public SearchController(ISearchService searchService)
        {
            _searchService = searchService;
        }

        [HttpGet("IsAlive")]
        public IActionResult IsAlive()
        {
            return Ok("MemeSearch is running");
        }

        [HttpGet("Search/{query}")]
        public IActionResult Search(string query, int start = 0)
        {
            return Ok(_searchService.Search(query, start));
        }
    }
}
