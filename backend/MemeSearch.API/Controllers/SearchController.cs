using Microsoft.AspNetCore.Mvc;

namespace MemeSearch.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class SearchController : ControllerBase
    {
        public SearchController()
        {
        }

        [HttpGet("IsAlive")]
        public IActionResult IsAlive()
        {
            return Ok("MemeSearch is running");
        }
    }
}
