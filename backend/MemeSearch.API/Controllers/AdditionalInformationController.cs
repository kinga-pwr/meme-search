using MemeSearch.Logic.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace MemeSearch.API.Controllers
{
    [Route("api/Information")]
    [ApiController]
    public class AdditionalInformationController : ControllerBase
    {
        private readonly IAdditionalInformationService _addInfoService;

        public AdditionalInformationController(IAdditionalInformationService addInfoService)
        {
            _addInfoService = addInfoService;
        }

        [HttpGet("Statuses")]
        public IActionResult GetStatuses()
        {
            return Ok(_addInfoService.Statuses);
        }

        [HttpGet("Details")]
        public IActionResult GetDetails()
        {
            return Ok(_addInfoService.Details);
        }

        [HttpGet("Categories")]
        public IActionResult GetCategories()
        {
            return Ok(_addInfoService.Categories);
        }
    }
}
