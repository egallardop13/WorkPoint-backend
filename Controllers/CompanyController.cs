using System.Data;
using Dapper;
using DotnetAPI.Data;
using DotnetAPI.Dtos;
using DotnetAPI.Helpers;
using DotnetAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DotnetAPI.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class CompanyController : ControllerBase
    {
        DataContextDapper _dapper;

        ReusableSql _reusableSql;

        public CompanyController(IConfiguration config)
        {
            _dapper = new DataContextDapper(config);
            _reusableSql = new ReusableSql(config);
        }

        [HttpGet("GetCompanyInfo")]
        public CompanyInfo GetCompanyInfo()
        {
            string sql = @"EXEC WorkPointSchema.spGet_CompanyInfo";

            // Execute stored procedure and map the results to the CompanyInfo model
            CompanyInfo companyInfo = _dapper.LoadDataSingleWithParameters<CompanyInfo>(
                sql,
                new DynamicParameters()
            );

            return companyInfo;
        }

        [HttpGet("GetBudget/{year}")]
        public IActionResult GetBudget(int year)
        {
            if (year < 1900 || year > 2100) // Basic validation for a valid year
            {
                return BadRequest("Invalid year. Please provide a year between 1900 and 2100.");
            }

            string sql = @"EXEC WorkPointSchema.spGet_Budget";
            string parameters = "";
            DynamicParameters sqlParameters = new DynamicParameters();

            // Add the year parameter
            parameters += " @Year = @YearParameter";
            sqlParameters.Add("@YearParameter", year, DbType.Int32);

            // Append parameters to the SQL query
            sql += parameters;

            try
            {
                // Execute the stored procedure and fetch the data
                var budgetData = _dapper.LoadDataWithParameters<Budget>(sql, sqlParameters);

                return Ok(budgetData);
            }
            catch (Exception ex)
            {
                // Log the exception
                Console.WriteLine($"Error: {ex.Message}");
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}
