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
    public class UserCompleteController : ControllerBase
    {
        DataContextDapper _dapper;

        ReusableSql _reusableSql;

        public UserCompleteController(IConfiguration config)
        {
            _dapper = new DataContextDapper(config);
            _reusableSql = new ReusableSql(config);
        }

        [HttpGet("GetUsers/{userId}/{isActive}")]
        public IEnumerable<UserComplete> GetUsers(int userId, bool isActive)
        {
            string sql = @"EXEC WorkPointSchema.spUsers_Get";
            string parameters = "";
            DynamicParameters sqlParameters = new DynamicParameters();

            if (userId != 0)
            {
                parameters += ", @UserId= @UserIdParameter";
                sqlParameters.Add("@UserIdParameter", userId, DbType.Int32);
            }
            if (isActive)
            {
                parameters += ", @Active= @ActiveParameter";
                sqlParameters.Add("@ActiveParameter", isActive, DbType.Boolean);
            }

            if (parameters.Length > 0)
            {
                sql += parameters.Substring(1);
            }

            IEnumerable<UserComplete> users = _dapper.LoadDataWithParameters<UserComplete>(
                sql,
                sqlParameters
            );
            return users;
        }

        [HttpGet("GetUsersWithPagination/{userId}/{isActive}/{Page}/{Limit}")]
        public IActionResult GetUsersWithPagination(int userId, bool isActive, int Page, int Limit)
        {
            string sql = @"EXEC WorkPointSchema.spUsers_Get_WithPagination";
            string parameters = "";
            DynamicParameters sqlParameters = new DynamicParameters();

            // Safely add @UserId parameter
            if (userId != 0)
            {
                parameters += ", @UserId= @UserIdParameter";
                sqlParameters.Add("@UserIdParameter", userId, DbType.Int32);
            }
            else
            {
                parameters += ", @UserId= @UserIdParameter";
                sqlParameters.Add("@UserIdParameter", null, DbType.Int32);
            }

            // Safely add @Active parameter
            if (isActive)
            {
                parameters += ", @Active= @ActiveParameter";
                sqlParameters.Add("@ActiveParameter", isActive, DbType.Boolean);
            }
            else
            {
                parameters += ", @Active= @ActiveParameter";
                sqlParameters.Add("@ActiveParameter", null, DbType.Boolean);
            }

            // Safely add @Page parameter
            if (Page > 0)
            {
                parameters += ", @Page= @PageParameter";
                sqlParameters.Add("@PageParameter", Page, DbType.Int32);
            }
            else
            {
                parameters += ", @Page= @PageParameter";
                sqlParameters.Add("@PageParameter", 1, DbType.Int32);
            }

            // Safely add @Limit parameter
            if (Limit > 0)
            {
                parameters += ", @Limit= @LimitParameter";
                sqlParameters.Add("@LimitParameter", Limit, DbType.Int32);
            }
            else
            {
                parameters += ", @Limit= @LimitParameter";
                sqlParameters.Add("@LimitParameter", 10, DbType.Int32);
            }

            // Build the final SQL query with parameters
            if (parameters.Length > 0)
            {
                sql += parameters.Substring(1); // Remove the leading comma
            }

            // Log the constructed SQL for debugging
            Console.WriteLine(sql);

            // Execute the stored procedure
            var result = _dapper
                .LoadDataWithParameters<dynamic>(sql, sqlParameters)
                .FirstOrDefault();

            // Ensure result is not null
            if (result == null)
            {
                return NotFound(
                    new
                    {
                        message = "No data found.",
                        arrayUserComplete = new List<UserComplete>(),
                        totalPages = 0,
                    }
                );
            }

            // Return result in the desired format
            return Ok(
                new
                {
                    arrayUserComplete = result.UserComplete, // Extracted JSON array from SP
                    totalPages = result.totalPages // Extracted total pages
                    ,
                }
            );
        }

        [HttpPut("UpsertUser")]
        public IActionResult UpsertUser(UserComplete user)
        {
            if (_reusableSql.UpsertUser(user))
            {
                return Ok();
            }

            throw new Exception("Failed to update user");
        }

        [HttpDelete("DeleteUser/{userId}")]
        public IActionResult DeleteUser(int userId)
        {
            string sql =
                @"WorkPointSchema.spUser_Delete
                @UserId = @UserIdParameter";

            DynamicParameters sqlParameters = new DynamicParameters();
            sqlParameters.Add("@UserIdParameter", userId, DbType.Int32);

            if (_dapper.ExecuteSqlWithParameter(sql, sqlParameters))
            {
                return Ok();
            }

            throw new Exception("Failed to delete user");
        }
    }
}
