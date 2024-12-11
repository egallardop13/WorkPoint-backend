using System.Data;
using Dapper;
using DotnetAPI.Data;
using DotnetAPI.Dtos;
using DotnetAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DotnetAPI.Controllers
{
    // [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class UserSalaryController : ControllerBase
    {
        DataContextDapper _dapper;

        public UserSalaryController(IConfiguration config)
        {
            _dapper = new DataContextDapper(config);
        }

        [HttpGet("GetUsersSalary/")]
        public IEnumerable<UserSalary> GetUsersSalary()
        {
            string sql =
                @"SELECT [UserId],
                [Salary]
                FROM WorkPointSchema.UserSalary";
            IEnumerable<UserSalary> users = _dapper.LoadData<UserSalary>(sql);
            return users;
            // string[] responseArray = new string[] {"Test1", "Test2", testValue};
            // return responseArray;
        }

        [HttpGet("GetDepartmentsInfo/{department?}")]
        public IEnumerable<DepartmentInfo> GetDepartmentsInfo(string? department = null)
        {
            string sql = @"EXEC WorkPointSchema.spGet_DepartmentsInfo";
            string parameters = "";
            DynamicParameters sqlParameters = new DynamicParameters();

            if (!string.IsNullOrWhiteSpace(department))
            {
                parameters += ", @Department = @DepartmentParameter";
                sqlParameters.Add("@DepartmentParameter", null, DbType.String);
            }

            if (parameters.Length > 0)
            {
                sql += parameters.Substring(1); // Remove leading comma
            }

            IEnumerable<DepartmentInfo> departmentStats =
                _dapper.LoadDataWithParameters<DepartmentInfo>(sql, sqlParameters);
            return departmentStats;
        }

        [HttpDelete("DeleteUserSalary/{userId}")]
        public IActionResult DeleteUserSalary(int userId)
        {
            string sql =
                @"DELETE FROM WorkPointSchema.UserSalary
        WHERE UserId = " + userId.ToString();

            Console.WriteLine(sql);
            if (_dapper.ExecuteSql(sql))
            {
                return Ok();
            }
            throw new Exception("Failed to delete user");
        }
    }
}
