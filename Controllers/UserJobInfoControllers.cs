using DotnetAPI.Models;
using DotnetAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DotnetAPI.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class UserJobInfoController : ControllerBase
    {
        private readonly ISalaryService _salaryService;

        public UserJobInfoController(ISalaryService salaryService)
        {
            _salaryService = salaryService;
        }

        [HttpGet("GetUsersInDepartments/{department}/{page}/{limit}")]
        public IActionResult GetUsersInDepartments(
            string department,
            int page,
            int limit,
            [FromQuery] string? query = null
        )
        {
            if (string.IsNullOrWhiteSpace(department) || page < 1 || limit < 1)
            {
                return BadRequest(
                    "Invalid parameters. Ensure department, page, and limit are provided and valid."
                );
            }

            return Ok(_salaryService.GetUsersInDepartments(department, page, limit, query));
        }

        [HttpGet("GetUsersJobInfo/")]
        public IEnumerable<UserJobInfo> GetUsersJobInfo()
        {
            return _salaryService.GetUsersJobInfo();
        }

        [HttpDelete("DeleteUserJobInfo/{userId}")]
        public IActionResult DeleteUser(int userId)
        {
            if (_salaryService.DeleteUserJobInfo(userId))
            {
                return Ok();
            }
            throw new Exception("Failed to delete user job info");
        }
    }
}
