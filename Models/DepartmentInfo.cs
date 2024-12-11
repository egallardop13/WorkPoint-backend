public class DepartmentInfo
{
    public string Department { get; set; } // Nullable depending on your DB schema
    public decimal? AvgSalary { get; set; } // Nullable in case no salaries exist
    public decimal? MinSalary { get; set; } // Nullable in case no salaries exist
    public decimal? MaxSalary { get; set; } // Nullable in case no salaries exist
    public decimal? TotalSalary { get; set; } // Nullable in case of no active employees
    public int? EmployeeCount { get; set; } // Nullable in case no employees exist
    public int? ActiveEmployeeCount { get; set; } // Nullable in case no active employees exist

    public DepartmentInfo()
    {
        if (Department == null)
        {
            Department = "";
        }
    }
}
