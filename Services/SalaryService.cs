using System.Data;
using Dapper;
using DotnetAPI.Data;
using DotnetAPI.Models;

namespace DotnetAPI.Services
{
    public class SalaryService : ISalaryService
    {
        private readonly IDataContextDapper _dapper;
        private readonly ILogger<SalaryService> _logger;

        public SalaryService(IDataContextDapper dapper, ILogger<SalaryService> logger)
        {
            _dapper = dapper;
            _logger = logger;
        }

        public IEnumerable<UserSalary> GetUsersSalary()
        {
            string sql =
                @"SELECT [UserId],
                [Salary]
                FROM WorkPointSchema.UserSalary";

            return _dapper.LoadData<UserSalary>(sql);
        }

        public IEnumerable<DepartmentInfo> GetDepartmentsInfo(string? department, string? query, string? sort)
        {
            string sql = @"EXEC WorkPointSchema.spGet_DepartmentsInfo";
            string parameters = "";
            DynamicParameters sqlParameters = new DynamicParameters();

            if (!string.IsNullOrWhiteSpace(department))
            {
                parameters += ", @Department = @DepartmentParameter";
                sqlParameters.Add("@DepartmentParameter", department, DbType.String);
            }
            if (!string.IsNullOrWhiteSpace(query))
            {
                parameters += ", @Query = @QueryParameter";
                sqlParameters.Add("@QueryParameter", query, DbType.String);
            }
            if (!string.IsNullOrWhiteSpace(sort))
            {
                parameters += ", @Sort = @SortParameter";
                sqlParameters.Add("@SortParameter", sort, DbType.String);
            }

            if (parameters.Length > 0)
            {
                sql += parameters.Substring(1);
            }

            return _dapper.LoadDataWithParameters<DepartmentInfo>(sql, sqlParameters);
        }

        public bool DeleteUserSalary(int userId)
        {
            string sql = "DELETE FROM WorkPointSchema.UserSalary WHERE UserId = @UserIdParam";
            DynamicParameters sqlParameters = new DynamicParameters();
            sqlParameters.Add("@UserIdParam", userId, DbType.Int32);

            return _dapper.ExecuteSqlWithParameter(sql, sqlParameters);
        }

        public IEnumerable<UserJobInfo> GetUsersJobInfo()
        {
            string sql =
                @"SELECT [UserId], [JobTitle], [Department] FROM WorkPointSchema.UserJobInfo";
            return _dapper.LoadData<UserJobInfo>(sql);
        }

        public bool DeleteUserJobInfo(int userId)
        {
            string sql = "DELETE FROM WorkPointSchema.UserJobInfo WHERE UserId = @UserIdParam";
            DynamicParameters sqlParameters = new DynamicParameters();
            sqlParameters.Add("@UserIdParam", userId, DbType.Int32);
            return _dapper.ExecuteSqlWithParameter(sql, sqlParameters);
        }

        public DepartmentSummary GetUsersInDepartments(string department, int page, int limit, string? query)
        {
            string sql = @"EXEC WorkPointSchema.spGet_UsersInDepartments";
            string parameters = "";
            DynamicParameters sqlParameters = new DynamicParameters();

            if (!string.IsNullOrWhiteSpace(department))
            {
                parameters += ", @Department = @DepartmentParameter";
                sqlParameters.Add("@DepartmentParameter", department, DbType.String);
            }
            if (page > 0)
            {
                parameters += ", @Page = @PageParameter";
                sqlParameters.Add("@PageParameter", page, DbType.Int32);
            }
            if (limit > 0)
            {
                parameters += ", @Limit = @LimitParameter";
                sqlParameters.Add("@LimitParameter", limit, DbType.Int32);
            }
            if (!string.IsNullOrWhiteSpace(query))
            {
                parameters += ", @Query = @QueryParameter";
                sqlParameters.Add("@QueryParameter", query, DbType.String);
            }

            if (parameters.Length > 0)
            {
                sql += parameters.Substring(1);
            }

            var result = _dapper.LoadDataWithParameters<dynamic>(sql, sqlParameters);

            return new DepartmentSummary
            {
                Users = result
                    .Select(r => new UserComplete
                    {
                        UserId = r.UserId,
                        FirstName = r.FirstName,
                        LastName = r.LastName,
                        Email = r.Email,
                        Gender = r.Gender,
                        Active = r.Active,
                        JobTitle = r.JobTitle,
                        Department = r.Department,
                        Salary = r.Salary,
                        AvgSalary = r.AvgSalary,
                        DateHired = r.DateHired,
                        DateExited = r.DateExited,
                    })
                    .ToList(),
                TotalPages = result.FirstOrDefault()?.TotalPages ?? 0,
                TotalUsers = result.FirstOrDefault()?.TotalUsers ?? 0,
                TotalDepartmentUsers = result.FirstOrDefault()?.TotalDepartmentUsers ?? 0,
                TotalActiveSalary = result.FirstOrDefault()?.TotalActiveSalary ?? 0,
                TotalInactiveSalary = result.FirstOrDefault()?.TotalInactiveSalary ?? 0,
            };
        }
    }
}
