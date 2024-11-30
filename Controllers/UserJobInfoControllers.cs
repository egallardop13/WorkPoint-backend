using DotnetAPI.Data;
using DotnetAPI.Dtos;
using DotnetAPI.Models;
using Microsoft.AspNetCore.Mvc;

namespace DotnetAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UserJobInfoController : ControllerBase
    {
        DataContextDapper _dapper;

        public UserJobInfoController(IConfiguration config)
        {
            _dapper = new DataContextDapper(config);
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
