using System.Data;
using Dapper;
using DotnetAPI.Data;
using DotnetAPI.Dtos;
using DotnetAPI.Models;
using Microsoft.AspNetCore.Http.HttpResults;

namespace DotnetAPI.Helpers
{
    public class ReusableSql
    {
        private readonly DataContextDapper _dapper;

        public ReusableSql(IConfiguration config)
        {
            _dapper = new DataContextDapper(config);
        }

        public UpsertDto UpsertUser(UserComplete user)
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

            UpsertDto upsert = new UpsertDto();
            upsert.Response = _dapper.LoadDataSingleWithParameters<int>(sql, sqlParameters);
            return upsert;
            // return _dapper.ExecuteSqlWithParameter(sql, sqlParameters);
        }
    }
}
