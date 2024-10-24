using DotnetAPI.Models;
using DotnetAPI.Dtos;
using DotnetAPI.Data;
using Microsoft.AspNetCore.Mvc;
namespace DotnetAPI.Controllers
{

[ApiController]
[Route("[controller]")]
public class UserJobInfoController : ControllerBase 
{
 DataContextDapper _dapper;
 public UserJobInfoController(IConfiguration config){
     _dapper = new DataContextDapper(config);
 }


[HttpGet("GetUsersJobInfo/")]
    public IEnumerable<UserJobInfo> GetUsersJobInfo() {
        string sql = @"SELECT [UserId],
    [JobTitle],
    [Department]   
FROM UsersSchema.UserJobInfo";
IEnumerable<UserJobInfo> users = _dapper.LoadData<UserJobInfo>(sql);
return users;
        // string[] responseArray = new string[] {"Test1", "Test2", testValue};
    // return responseArray;
    }

    [HttpGet("GetUserJobInfo/{userId}")]
    public UserJobInfo GetSingleUserJobInfo(int userId) {
        string sql = @"SELECT [UserId],
    [JobTitle],
    [Department] 
FROM UsersSchema.UserJobInfo
 WHERE UserId = " + userId.ToString();
UserJobInfo user = _dapper.LoadDataSingle<UserJobInfo>(sql);
return user;
    }
[HttpPut("EditUserJobInfo/")]
public IActionResult EditUserJobInfo( UserJobInfo user) {
    string sql = @" UPDATE UsersSchema.UserJobInfo
   SET [JobTitle] = '" + user.JobTitle + 
        "', [Department] = '" + user.Department + 
    "' WHERE UserId = " + user.UserId; ;

    Console.WriteLine(sql);

 if(user.UserId == 0) {
    throw new Exception("Failed to update user job info, user id can not be 0");
        }else if(_dapper.ExecuteSql(sql)) {
        return Ok();
    }
    throw new Exception("Failed to update user job info");
}

[HttpPost("AddUserJobInfo")]
public IActionResult AddUserJobInfo(UserJobInfoToAddDTO user) {
    string sql = @"INSERT INTO UsersSchema.UserJobInfo(
    [JobTitle],
    [Department]  
) VALUES (" +
        "'" + user.JobTitle + 
        "', '" + user.Department + 
"')";
    Console.WriteLine(sql);
  if(_dapper.ExecuteSql(sql)) {
        return Ok();
    }
    throw new Exception("Failed to add user job info");
}
 [HttpDelete("DeleteUserJobInfo/{userId}")]
    public IActionResult DeleteUser(int userId) {
        string sql = @"DELETE FROM UsersSchema.UserJobInfo
        WHERE UserId = " + userId.ToString();

 Console.WriteLine(sql);
  if(_dapper.ExecuteSql(sql)) {
        return Ok();
    }
    throw new Exception("Failed to delete user job info");
    }
}
}

