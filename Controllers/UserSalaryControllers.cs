using DotnetAPI.Data;
using DotnetAPI.Dtos;
using DotnetAPI.Models;
using Microsoft.AspNetCore.Mvc;

namespace DotnetAPI.Controllers
{
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
                FROM UsersSchema.UserSalary";
            IEnumerable<UserSalary> users = _dapper.LoadData<UserSalary>(sql);
            return users;
            // string[] responseArray = new string[] {"Test1", "Test2", testValue};
            // return responseArray;
        }

        [HttpDelete("DeleteUserSalary/{userId}")]
        public IActionResult DeleteUserSalary(int userId)
        {
            string sql =
                @"DELETE FROM UsersSchema.UserSalary
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
