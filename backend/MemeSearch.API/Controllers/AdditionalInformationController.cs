using MemeSearch.Logic.Interfaces;
using MemeSearch.Logic.Models;
using Microsoft.AspNetCore.Mvc;
using System.Linq;

namespace MemeSearch.API.Controllers
{
    [Route("Information")]
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
            return Ok(_addInfoService.Details.Select(elem => new AdditionalInfoItem()
            {
                Name = elem.Key,
                Quantity = elem.Value
            }).OrderByDescending(elem => elem.Quantity));
        }

        [HttpGet("Categories")]
        public IActionResult GetCategories()
        {
            return Ok(_addInfoService.Categories.Select(elem => new AdditionalInfoItem()
            {
                Name = elem.Key,
                Quantity = elem.Value
            }).OrderByDescending(elem => elem.Quantity));
        }
    }
}
