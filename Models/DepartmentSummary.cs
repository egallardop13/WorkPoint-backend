using DotnetAPI.Models;

namespace DotnetAPI.Models
{
    public class DepartmentSummary
    {
        public IEnumerable<UserComplete>? Users { get; set; }
        public int TotalPages { get; set; }
        public int TotalUsers { get; set; }
        public int TotalDepartmentUsers { get; set; }
        public decimal TotalActiveSalary { get; set; }
        public decimal TotalInactiveSalary { get; set; }
    }
}
