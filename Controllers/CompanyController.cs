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
    // [Authorize]
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
    }
}
