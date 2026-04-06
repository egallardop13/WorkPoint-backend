using DotnetAPI.Models;

namespace DotnetAPI.Services
{
    public interface ICompanyService
    {
        CompanyInfo GetCompanyInfo();
        MetricsInfo GetMetrics(int year, bool status);
        IEnumerable<Budget> GetBudget(int year);
    }
}
