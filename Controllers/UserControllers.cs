using DotnetAPI.Models;
using Microsoft.AspNetCore.Mvc;
namespace DotnetAPI.Controllers
{

[ApiController]
[Route("[controller]")]
public class UserController : ControllerBase 
{
 DataContextDapper _dapper;
 public UserController(IConfiguration config){
     _dapper = new DataContextDapper(config);
 }
[HttpGet("TestConnection")]
public DateTime TestConnection() {
    return _dapper.LoadDataSingle<DateTime>("SELECT GETDATE() ");
}

[HttpGet("GetUsers/")]
    public IEnumerable<User> GetUsers() {
        string sql = @"SELECT [UserId],
    [FirstName],
    [LastName],
    [Email],
    [Gender],
    [Active]    
FROM UsersSchema.Users";
IEnumerable<User> users = _dapper.LoadData<User>(sql);
return users;
        // string[] responseArray = new string[] {"Test1", "Test2", testValue};
    // return responseArray;
    }

    [HttpGet("GetUsers/{userId}")]
    public User GetSingleUser(int userId) {
        string sql = @"SELECT [UserId],
    [FirstName],
    [LastName],
    [Email],
    [Gender],
    [Active]    
FROM UsersSchema.Users
 WHERE UserId = " + userId.ToString();
User user = _dapper.LoadDataSingle<User>(sql);
return user;
    }
}
}

