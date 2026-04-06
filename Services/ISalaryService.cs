using DotnetAPI.Models;

namespace DotnetAPI.Services
{
    public interface ISalaryService
    {
        IEnumerable<UserSalary> GetUsersSalary();
        IEnumerable<DepartmentInfo> GetDepartmentsInfo(string? department, string? query, string? sort);
        bool DeleteUserSalary(int userId);
        IEnumerable<UserJobInfo> GetUsersJobInfo();
        bool DeleteUserJobInfo(int userId);
        DepartmentSummary GetUsersInDepartments(string department, int page, int limit, string? query);
    }
}
