using DotnetAPI.Models;
using DotnetAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DotnetAPI.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class UserCompleteController : ControllerBase
    {
        private readonly IUserService _userService;

        public UserCompleteController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpGet("GetUsers/{userId}/{isActive}")]
        public IEnumerable<UserComplete> GetUsers(int userId, bool isActive)
        {
            return _userService.GetUsers(userId, isActive);
        }

        [HttpGet("GetUsersWithPagination/{Page}/{Limit}")]
        public IActionResult GetUsersWithPagination(
            int Page,
            int Limit,
            string? query = null,
            string? sort = null
        )
        {
            var result = _userService.GetUsersWithPagination(Page, Limit, query, sort);

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

            return Ok(result);
        }

        [HttpPut("UpsertUser")]
        public IActionResult UpsertUser(UserComplete user)
        {
            int result = _userService.UpsertUser(user);
            if (result == 1)
            {
                return Ok(new { message = "User updated or created successfully." });
            }
            else if (result == 0)
            {
                return StatusCode(409, new { message = "Email already exists.", status = 409 });
            }

            throw new Exception("Failed to update user");
        }

        [HttpDelete("DeleteUser/{userId}")]
        public IActionResult DeleteUser(int userId)
        {
            if (_userService.DeleteUser(userId))
            {
                return Ok();
            }

            throw new Exception("Failed to delete user");
        }
    }
}
