using MemeSearch.Logic.Helpers;
using MemeSearch.Logic.Interfaces;
using MemeSearch.Logic.Models;
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
            return Ok("MemeSearch is running...");
        }

        [HttpGet("Search/{query}")]
        public IActionResult Search(string query, int results = 20, int start = 0)
        {
            if (!ValidationHelper.ValidateSearch(query, results, start, out string message))
            {
                return BadRequest(message);
            }

            return Ok(_searchService.StandardSearch(query, results, start));
        }

        [HttpPost("AdvancedSearch/{query}")]
        public IActionResult AdvancedSearch(string query, [FromBody]SearchParameters parameters, int results = 20, int start = 0)
        {
            if (!ValidationHelper.ValidateAdvancedSearch(query, parameters, results, start, out string message))
            {
                return BadRequest(message);
            }

            return Ok(_searchService.AdvancedSearch(parameters, query, results, start));
        }
    }
}
