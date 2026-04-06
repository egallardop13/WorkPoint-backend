using System.Data;
using Dapper;
using DotnetAPI.Data;
using DotnetAPI.Models;

namespace DotnetAPI.Services
{
    public class UserService : IUserService
    {
        private readonly DataContextDapper _dapper;
        private readonly ILogger<UserService> _logger;

        public UserService(DataContextDapper dapper, ILogger<UserService> logger)
        {
            _dapper = dapper;
            _logger = logger;
        }

        public IEnumerable<UserComplete> GetUsers(int userId, bool isActive)
        {
            string sql = @"EXEC WorkPointSchema.spUsers_Get";
            string parameters = "";
            DynamicParameters sqlParameters = new DynamicParameters();

            if (userId != 0)
            {
                parameters += ", @UserId= @UserIdParameter";
                sqlParameters.Add("@UserIdParameter", userId, DbType.Int32);
            }
            if (isActive)
            {
                parameters += ", @Active= @ActiveParameter";
                sqlParameters.Add("@ActiveParameter", isActive, DbType.Boolean);
            }

            if (parameters.Length > 0)
            {
                sql += parameters.Substring(1);
            }

            return _dapper.LoadDataWithParameters<UserComplete>(sql, sqlParameters);
        }

        public object? GetUsersWithPagination(int page, int limit, string? query, string? sort)
        {
            string sql = @"EXEC WorkPointSchema.spUsers_Get_WithPagination";
            string parameters = "";
            DynamicParameters sqlParameters = new DynamicParameters();

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

            parameters += ", @Page= @PageParameter";
            sqlParameters.Add("@PageParameter", page > 0 ? page : 1, DbType.Int32);

            parameters += ", @Limit= @LimitParameter";
            sqlParameters.Add("@LimitParameter", limit > 0 ? limit : 10, DbType.Int32);

            if (parameters.Length > 0)
            {
                sql += parameters.Substring(1);
            }

            _logger.LogDebug("{Sql}", sql);

            var result = _dapper
                .LoadDataWithParameters<dynamic>(sql, sqlParameters)
                .FirstOrDefault();

            if (result == null)
            {
                return null;
            }

            return new
            {
                arrayUserComplete = result.UserComplete,
                totalPages = result.totalPages,
                totalUsers = result.totalUsers,
            };
        }

        public int UpsertUser(UserComplete user)
        {
            string sql =
                @"EXEC WorkPointSchema.spUser_Upsert
                @FirstName = @FirstNameParameter,
                @LastName = @LastNameParameter,
                @Email = @EmailParameter,
                @Gender = @GenderParameter,
                @Active = @ActiveParameter,
                @JobTitle = @JobTitleParameter,
                @Department = @DepartmentParameter,
                @Salary = @SalaryParameter,
                @DateHired = @DateHiredParameter,
                @DateExited = @DateExitedParameter,
                @UserId = @UserIdParameter";

            DynamicParameters sqlParameters = new DynamicParameters();
            sqlParameters.Add("@FirstNameParameter", user.FirstName, DbType.String);
            sqlParameters.Add("@LastNameParameter", user.LastName, DbType.String);
            sqlParameters.Add("@EmailParameter", user.Email, DbType.String);
            sqlParameters.Add("@GenderParameter", user.Gender, DbType.String);
            sqlParameters.Add("@ActiveParameter", user.Active, DbType.Boolean);
            sqlParameters.Add("@JobTitleParameter", user.JobTitle, DbType.String);
            sqlParameters.Add("@DepartmentParameter", user.Department, DbType.String);
            sqlParameters.Add("@SalaryParameter", user.Salary, DbType.String);
            sqlParameters.Add("@UserIdParameter", user.UserId, DbType.Int32);
            sqlParameters.Add("@DateHiredParameter", user.DateHired, DbType.DateTime);
            sqlParameters.Add("@DateExitedParameter", user.DateExited, DbType.DateTime);

            return _dapper.LoadDataSingleWithParameters<int>(sql, sqlParameters);
        }

        public bool DeleteUser(int userId)
        {
            string sql =
                @"WorkPointSchema.spUser_Delete
                @UserId = @UserIdParameter";

            DynamicParameters sqlParameters = new DynamicParameters();
            sqlParameters.Add("@UserIdParameter", userId, DbType.Int32);

            return _dapper.ExecuteSqlWithParameter(sql, sqlParameters);
        }
    }
}
