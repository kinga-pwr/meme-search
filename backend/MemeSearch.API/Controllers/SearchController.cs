using MemeSearch.Logic.Helpers;
using MemeSearch.Logic.Interfaces;
using MemeSearch.Logic.Models;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Net;

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
            query = query.Trim();

            if (!ValidationHelper.ValidateSearch(query, results, start, out string message))
            {
                return BadRequest(message);
            }

            return Ok(_searchService.StandardSearch(query, results, start));
        }

        [HttpPost("AdvancedSearch")]
        public IActionResult AdvancedSearch(string query, [FromBody]SearchParameters parameters, int results = 20, int start = 0)
        {
            if (query is null) query = string.Empty;
            query = query.Trim();

            if (!ValidationHelper.ValidateAdvancedSearch(query, parameters, results, start, out string message))
            {
                return BadRequest(message);
            }

            return Ok(_searchService.AdvancedSearch(parameters, query, results, start));
        }

        [HttpPost("ImageSearch")]
        public IActionResult ImageSearch([FromBody] ImageSearchParameters parameters, int results = 20, int start = 0)
        {
            if (!_searchService.CanImageSearch())
            {
                return StatusCode((int)HttpStatusCode.ServiceUnavailable);
            }

            parameters.Fields = new List<string> { "Image" };

            if (!ValidationHelper.ValidateImageSearch(parameters, results, start, out string message))
            {
                return BadRequest(message);
            }

            var result = _searchService.ImageSearch(parameters, results, start);
            return result != null ? Ok(result) : StatusCode((int)HttpStatusCode.ServiceUnavailable);
        }
    }
}
