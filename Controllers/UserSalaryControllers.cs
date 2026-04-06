using DotnetAPI.Models;
using DotnetAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DotnetAPI.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class UserSalaryController : ControllerBase
    {
        private readonly ISalaryService _salaryService;

        public UserSalaryController(ISalaryService salaryService)
        {
            _salaryService = salaryService;
        }

        [HttpGet("GetUsersSalary/")]
        public IEnumerable<UserSalary> GetUsersSalary()
        {
            return _salaryService.GetUsersSalary();
        }

        [HttpGet("GetDepartmentsInfo/{department?}")]
        public IEnumerable<DepartmentInfo> GetDepartmentsInfo(
            string? department = null,
            string? query = null,
            string? sort = null
        )
        {
            return _salaryService.GetDepartmentsInfo(department, query, sort);
        }

        [HttpDelete("DeleteUserSalary/{userId}")]
        public IActionResult DeleteUserSalary(int userId)
        {
            if (_salaryService.DeleteUserSalary(userId))
            {
                return Ok();
            }
            throw new Exception("Failed to delete user");
        }
    }
}
