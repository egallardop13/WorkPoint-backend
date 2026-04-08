using System.Data;
using Dapper;
using DotnetAPI.Data;
using DotnetAPI.Models;

namespace DotnetAPI.Services
{
    public class PostService : IPostService
    {
        private readonly IDataContextDapper _dapper;
        private readonly ILogger<PostService> _logger;

        public PostService(IDataContextDapper dapper, ILogger<PostService> logger)
        {
            _dapper = dapper;
            _logger = logger;
        }

        public IEnumerable<Post> GetPosts(int postId, int userId, string searchParam)
        {
            string sql = @"EXEC WorkPointSchema.spPost_Get";
            string stringParameters = "";
            DynamicParameters sqlParameters = new DynamicParameters();

            if (postId != 0)
            {
                stringParameters += ", @PostId = @PostIdParameter";
                sqlParameters.Add("@PostIdParameter", postId, DbType.Int32);
            }
            if (userId != 0)
            {
                stringParameters += ", @UserId = @UserIdParameter";
                sqlParameters.Add("@UserIdParameter", userId, DbType.Int32);
            }
            if (searchParam.ToLower() != "none")
            {
                stringParameters += ", @SearchValue = @SearchValueParameter";
                sqlParameters.Add("@SearchValueParameter", searchParam, DbType.String);
            }

            if (stringParameters.Length > 0)
            {
                sql += stringParameters.Substring(1);
            }

            _logger.LogDebug("{Sql}", sql);

            return _dapper.LoadDataWithParameters<Post>(sql, sqlParameters);
        }

        public IEnumerable<Post> GetMyPosts(int userId)
        {
            string sql = @"EXEC WorkPointSchema.spPost_Get @UserId = @UserIdParameter";
            DynamicParameters sqlParameters = new DynamicParameters();
            sqlParameters.Add("@UserIdParameter", userId, DbType.Int32);

            return _dapper.LoadDataWithParameters<Post>(sql, sqlParameters);
        }

        public bool UpsertPost(Post post, int userId)
        {
            string sql =
                @"EXEC WorkPointSchema.spPosts_Upsert
                @UserId = @UserIdParameter,
                @PostTitle = @PostTitleParameter,
                @PostContent = @PostContentParameter";

            DynamicParameters sqlParameters = new DynamicParameters();
            sqlParameters.Add("@UserIdParameter", userId, DbType.Int32);
            sqlParameters.Add("@PostTitleParameter", post.PostTitle, DbType.String);
            sqlParameters.Add("@PostContentParameter", post.PostContent, DbType.String);

            if (post.PostId != 0)
            {
                sql += ", @PostId = @PostIdParameter";
                sqlParameters.Add("@PostIdParameter", post.PostId, DbType.Int32);
            }

            return _dapper.ExecuteSqlWithParameter(sql, sqlParameters);
        }

        public bool DeletePost(int postId, int userId)
        {
            string sql =
                @"EXEC WorkPointSchema.spPost_Delete @PostId = @PostIdParameter,
                @UserId = @UserIdParameter";

            DynamicParameters sqlParameters = new DynamicParameters();
            sqlParameters.Add("@PostIdParameter", postId, DbType.Int32);
            sqlParameters.Add("@UserIdParameter", userId, DbType.Int32);

            return _dapper.ExecuteSqlWithParameter(sql, sqlParameters);
        }
    }
}
