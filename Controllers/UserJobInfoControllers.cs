using System.Data;
using Dapper;
using DotnetAPI.Data;
using DotnetAPI.Dtos;
using DotnetAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;

namespace DotnetAPI.Controllers
{
    // [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class UserJobInfoController : ControllerBase
    {
        DataContextDapper _dapper;

        public UserJobInfoController(IConfiguration config)
        {
            _dapper = new DataContextDapper(config);
        }

        [HttpGet("GetUsersInDepartments/{department}/{page}/{limit}")]
        public IActionResult GetUsersInDepartments(
            string department,
            int page,
            int limit,
            string? query = null
        )
        {
            if (string.IsNullOrWhiteSpace(department) || page < 1 || limit < 1)
            {
                return BadRequest(
                    "Invalid parameters. Ensure department, page, and limit are provided and valid."
                );
            }

            string sql = @"EXEC WorkPointSchema.spGet_UsersInDepartments";
            string parameters = "";
            DynamicParameters sqlParameters = new DynamicParameters();

            // Add parameters for the stored procedure
            if (!string.IsNullOrWhiteSpace(department))
            {
                parameters += ", @Department = @DepartmentParameter";
                sqlParameters.Add("@DepartmentParameter", department, DbType.String);
            }

            if (page > 0)
            {
                parameters += ", @Page = @PageParameter";
                sqlParameters.Add("@PageParameter", page, DbType.Int32);
            }

            if (limit > 0)
            {
                parameters += ", @Limit = @LimitParameter";
                sqlParameters.Add("@LimitParameter", limit, DbType.Int32);
            }

            if (!string.IsNullOrWhiteSpace(query))
            {
                parameters += ", @Query = @QueryParameter";
                sqlParameters.Add("@QueryParameter", query, DbType.String);
            }

            if (parameters.Length > 0)
            {
                sql += parameters.Substring(1); // Remove leading comma
            }

            try
            {
                // Fetch the unified result set from the stored procedure
                var result = _dapper.LoadDataWithParameters<dynamic>(sql, sqlParameters);

                // Map the result to the appropriate models
                var summary = new DepartmentSummary
                {
                    Users = result
                        .Select(r => new UserComplete
                        {
                            UserId = r.UserId,
                            FirstName = r.FirstName,
                            LastName = r.LastName,
                            Email = r.Email,
                            Gender = r.Gender,
                            Active = r.Active,
                            JobTitle = r.JobTitle,
                            Department = r.Department,
                            Salary = r.Salary,
                            AvgSalary = r.AvgSalary,
                            DateHired = r.DateHired,
                            DateExited = r.DateExited,
                        })
                        .ToList(),
                    TotalPages = result.FirstOrDefault()?.TotalPages ?? 0,
                    TotalUsers = result.FirstOrDefault()?.TotalUsers ?? 0,
                    TotalDepartmentUsers = result.FirstOrDefault()?.TotalDepartmentUsers ?? 0,
                    TotalActiveSalary = result.FirstOrDefault()?.TotalActiveSalary ?? 0,
                    TotalInactiveSalary = result.FirstOrDefault()?.TotalInactiveSalary ?? 0,
                };

                return Ok(summary);
            }
            catch (Exception ex)
            {
                // Log the exception and return an error response
                Console.WriteLine($"Error: {ex.Message}");
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("GetUsersJobInfo/")]
        public IEnumerable<UserJobInfo> GetUsersJobInfo()
        {
            string sql =
                @"SELECT [UserId],
    [JobTitle],
    [Department]   
FROM WorkPointSchema.UserJobInfo";

            IEnumerable<UserJobInfo> users = _dapper.LoadData<UserJobInfo>(sql);
            return users;
            // string[] responseArray = new string[] {"Test1", "Test2", testValue};
            // return responseArray;
        }

        [HttpDelete("DeleteUserJobInfo/{userId}")]
        public IActionResult DeleteUser(int userId)
        {
            string sql =
                @"DELETE FROM WorkPointSchema.UserJobInfo
        WHERE UserId = " + userId.ToString();

            Console.WriteLine(sql);
            if (_dapper.ExecuteSql(sql))
            {
                return Ok();
            }
            throw new Exception("Failed to delete user job info");
        }
    }
}
