using DotnetAPI.Models;
using DotnetAPI.Dtos;
using DotnetAPI.Data;
using Microsoft.AspNetCore.Mvc;
namespace DotnetAPI.Controllers
{

[ApiController]
[Route("[controller]")]
public class UserSalaryController : ControllerBase 
{
 DataContextDapper _dapper;
 public UserSalaryController(IConfiguration config){
     _dapper = new DataContextDapper(config);
 }


[HttpGet("GetUsersSalary/")]
    public IEnumerable<UserSalary> GetUsersSalary() {
        string sql = @"SELECT [UserId],
    [Salary]
FROM UsersSchema.UserSalary";
IEnumerable<UserSalary> users = _dapper.LoadData<UserSalary>(sql);
return users;
        // string[] responseArray = new string[] {"Test1", "Test2", testValue};
    // return responseArray;
    }

    [HttpGet("GetUserSalary/{userId}")]
    public UserSalary GetSingleUserSalary(int userId) {
        string sql = @"SELECT [UserId],
    [Salary]
FROM UsersSchema.UserSalary
 WHERE UserId = " + userId.ToString();
UserSalary user = _dapper.LoadDataSingle<UserSalary>(sql);
return user;
    }
[HttpPut("EditUserSalary/")]
public IActionResult EditUserSalary( UserSalary user) {
    string sql = @" UPDATE UsersSchema.UserSalary
   SET [Salary] = '" + user.Salary + 
    "' WHERE UserId = " + user.UserId; ;

    Console.WriteLine(sql);

 if(user.UserId == 0) {
    throw new Exception("Failed to update user salary, user id can not be 0");
        }else if(_dapper.ExecuteSql(sql)) {
        return Ok();
    }
    throw new Exception("Failed to update user salary");
}

[HttpPost("AddUserSalary")]
public IActionResult AddUserSalary(UserSalaryToAddDTO user) {
    string sql = @"INSERT INTO UsersSchema.UserSalary(
    [Salary]
) VALUES (" +
        "'" + user.Salary + 
"')";
    Console.WriteLine(sql);
  if(_dapper.ExecuteSql(sql)) {
        return Ok();
    }
    throw new Exception("Failed to add user");
}
 [HttpDelete("DeleteUserSalary/{userId}")]
    public IActionResult DeleteUserSalary(int userId) {
        string sql = @"DELETE FROM UsersSchema.UserSalary
        WHERE UserId = " + userId.ToString();

 Console.WriteLine(sql);
  if(_dapper.ExecuteSql(sql)) {
        return Ok();
    }
    throw new Exception("Failed to delete user");
    }
}
}

