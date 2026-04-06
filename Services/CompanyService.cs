using System.Data;
using Dapper;
using DotnetAPI.Data;
using DotnetAPI.Helpers;
using DotnetAPI.Models;

namespace DotnetAPI.Services
{
    public class CompanyService : ICompanyService
    {
        private readonly DataContextDapper _dapper;
        private readonly ILogger<CompanyService> _logger;

        public CompanyService(DataContextDapper dapper, ILogger<CompanyService> logger)
        {
            _dapper = dapper;
            _logger = logger;
        }

        public CompanyInfo GetCompanyInfo()
        {
            string sql = @"EXEC WorkPointSchema.spGet_CompanyInfo";

            return _dapper.LoadDataSingleWithParameters<CompanyInfo>(sql, new DynamicParameters());
        }

        public MetricsInfo GetMetrics(int year, bool status)
        {
            string sql = @"EXEC WorkPointSchema.spGet_Metrics";
            string parameters = "";
            DynamicParameters sqlParameters = new DynamicParameters();

            parameters += ", @Year = @YearParameter";
            sqlParameters.Add("@YearParameter", year, DbType.Int32);

            parameters += ", @Status = @StatusParameter";
            sqlParameters.Add("@StatusParameter", status, DbType.Boolean);

            if (parameters.Length > 0)
            {
                sql += parameters.Substring(1);
            }

            var metricsRaw = _dapper.LoadDataSingleWithParameters<dynamic>(sql, sqlParameters);

            var monthlyBreakdown = DataParserHelper.ParseMonthlyData(
                metricsRaw.MonthlyBreakdown?.ToString()
            );

            return new MetricsInfo
            {
                TotalEmployees = metricsRaw.TotalEmployees,
                JoinedOrLeftYearly = metricsRaw.JoinedOrLeftYearly,
                MonthlyBreakdown = monthlyBreakdown,
            };
        }

        public IEnumerable<Budget> GetBudget(int year)
        {
            string sql = @"EXEC WorkPointSchema.spGet_Budget @Year = @YearParameter";
            DynamicParameters sqlParameters = new DynamicParameters();
            sqlParameters.Add("@YearParameter", year, DbType.Int32);

            return _dapper.LoadDataWithParameters<Budget>(sql, sqlParameters);
        }
    }
}
