namespace DotnetAPI.Models
{
    public class DepartmentInfo
    {
        public string Department { get; set; } = "";
        public decimal? AvgSalary { get; set; }
        public decimal? MinSalary { get; set; }
        public decimal? MaxSalary { get; set; }
        public decimal? TotalSalary { get; set; }
        public int? EmployeeCount { get; set; }
        public int? ActiveEmployeeCount { get; set; }
    }
}
