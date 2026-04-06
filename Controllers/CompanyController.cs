using DotnetAPI.Models;
using DotnetAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DotnetAPI.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class CompanyController : ControllerBase
    {
        private readonly ICompanyService _companyService;

        public CompanyController(ICompanyService companyService)
        {
            _companyService = companyService;
        }

        [HttpGet("GetCompanyInfo")]
        public CompanyInfo GetCompanyInfo()
        {
            return _companyService.GetCompanyInfo();
        }

        [HttpGet("GetMetrics/{year}/{status}")]
        public IActionResult GetMetrics(int year, bool status)
        {
            if (year < 1900 || year > 2100)
            {
                return BadRequest("Invalid year. Please provide a year between 1900 and 2100.");
            }

            return Ok(_companyService.GetMetrics(year, status));
        }

        [HttpGet("GetBudget/{year}")]
        public IActionResult GetBudget(int year)
        {
            if (year < 1900 || year > 2100)
            {
                return BadRequest("Invalid year. Please provide a year between 1900 and 2100.");
            }

            return Ok(_companyService.GetBudget(year));
        }
    }
}
