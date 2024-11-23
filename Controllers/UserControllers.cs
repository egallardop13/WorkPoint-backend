// using DotnetAPI.Models;
// using DotnetAPI.Dtos;
// using DotnetAPI.Data;
// using Microsoft.AspNetCore.Mvc;
// namespace DotnetAPI.Controllers
// {

// [ApiController]
// [Route("[controller]")]
// public class UserController : ControllerBase 
// {
//  DataContextDapper _dapper;
//  public UserController(IConfiguration config){
//      _dapper = new DataContextDapper(config);
//  }


// [HttpGet("GetUsers/")]
//     public IEnumerable<User> GetUsers() {
//         string sql = @"SELECT [UserId],
//     [FirstName],
//     [LastName],
//     [Email],
//     [Gender],
//     [Active]    
// FROM UsersSchema.Users";
// IEnumerable<User> users = _dapper.LoadData<User>(sql);
// return users;
//         // string[] responseArray = new string[] {"Test1", "Test2", testValue};
//     // return responseArray;
//     }

//     [HttpGet("GetUsers/{userId}")]
//     public User GetSingleUser(int userId) {
//         string sql = @"SELECT [UserId],
//     [FirstName],
//     [LastName],
//     [Email],
//     [Gender],
//     [Active]    
// FROM UsersSchema.Users
//  WHERE UserId = " + userId.ToString();
// User user = _dapper.LoadDataSingle<User>(sql);
// return user;
//     }
// [HttpPut("EditUser/")]
// public IActionResult EditUser( User user) {
//     string sql = @" UPDATE UsersSchema.Users
//    SET [FirstName] = '" + user.FirstName + 
//         "', [LastName] = '" + user.LastName + 
//         "', [Email] = '" + user.Email + 
//         "', [Gender] = '" + user.Gender +
//         "', [Active] = '" + user.Active +
//     "' WHERE UserId = " + user.UserId; ;

//     Console.WriteLine(sql);

//  if(user.UserId == 0) {
//     throw new Exception("Failed to update user, user id can not be 0");
//         }else if(_dapper.ExecuteSql(sql)) {
//         return Ok();
//     }
//     throw new Exception("Failed to update user");
// }

// [HttpPost("AddUser")]
// public IActionResult AddUser(UserToAddDTO user) {
//     string sql = @"INSERT INTO UsersSchema.Users(
//     [FirstName],
//     [LastName],
//     [Email],
//     [Gender],
//     [Active]    
// ) VALUES (" +
//         "'" + user.FirstName + 
//         "', '" + user.LastName + 
//         "', '" + user.Email + 
//         "', '" + user.Gender +
//         "', '" + user.Active +
// "')";
//     Console.WriteLine(sql);
//   if(_dapper.ExecuteSql(sql)) {
//         return Ok();
//     }
//     throw new Exception("Failed to add user");
// }
//  [HttpDelete("DeleteUser/{userId}")]
//     public IActionResult DeleteUser(int userId) {
//         string sql = @"DELETE FROM UsersSchema.Users
//         WHERE UserId = " + userId.ToString();

//  Console.WriteLine(sql);
//   if(_dapper.ExecuteSql(sql)) {
//         return Ok();
//     }
//     throw new Exception("Failed to delete user");
//     }
// }
// }

