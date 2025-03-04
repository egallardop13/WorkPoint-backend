using System.Data;
using Dapper;
using DotnetAPI.Data;
using DotnetAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DotnetAPI.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class PostController : ControllerBase
    {
        private readonly DataContextDapper _dapper;

        public PostController(IConfiguration config)
        {
            _dapper = new DataContextDapper(config);
        }

        [HttpGet("Posts/{postId}/{userId}/{searchParam}")]
        public IEnumerable<Post> GetPosts(
            int postId = 0,
            int userId = 0,
            string searchParam = "None"
        )
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
                sqlParameters.Add("@UserIdParameter", postId, DbType.Int32);
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
            Console.WriteLine(sql);
            return _dapper.LoadDataWithParameters<Post>(sql, sqlParameters);
        }

        [HttpGet("MyPosts")]
        public IEnumerable<Post> GetMyPosts()
        {
            string sql = @"EXEC WorkPointSchema.spPost_Get @PostId = @UserIdParameter";
            DynamicParameters sqlParameters = new DynamicParameters();
            sqlParameters.Add(
                "@UserIdParameter",
                this.User.FindFirst("userId")?.Value,
                DbType.Int32
            );
            return _dapper.LoadDataWithParameters<Post>(sql, sqlParameters);
        }

        [HttpPut("UpsertPost")]
        public IActionResult UpsertPost(Post postToUpsert)
        {
            string sql =
                @"EXEC WorkPointSchema.spPosts_Upsert 
                @UserId = @UserIdParameter, 
                @PostTitle = @PostTitleParameter, 
                @PostContent = @PostContentParameter";

            DynamicParameters sqlParameters = new DynamicParameters();
            sqlParameters.Add(
                "@UserIdParameter",
                this.User.FindFirst("userId")?.Value,
                DbType.Int32
            );
            sqlParameters.Add("@PostTitleParameter", postToUpsert.PostTitle, DbType.String);
            sqlParameters.Add("@PostContentParameter", postToUpsert.PostContent, DbType.String);

            if (postToUpsert.PostId != 0)
            {
                sql += ", @PostId = @PostIdParameter";
                sqlParameters.Add("@PostIdParameter", postToUpsert.PostId, DbType.Int32);
            }
            if (_dapper.ExecuteSqlWithParameter(sql, sqlParameters))
            {
                return Ok();
            }
            throw new Exception("Failed to Upsert a post");
        }

        // [HttpPut("Post")]
        // public IActionResult EditPost(PostToEditDTO postToEdit)
        // {
        //     string sql =
        //         @"
        //     UPDATE WorkPointSchema.Posts
        //         SET PostTitle = '"
        //         + postToEdit.PostTitle
        //         + "', PostContent = '"
        //         + postToEdit.PostContent
        //         + @"', PostUpdated = GETDATE()
        //             WHERE PostId = "
        //         + postToEdit.PostID.ToString()
        //         + " AND UserId = "
        //         + (this.User.FindFirst("userId")?.Value);
        //     if (_dapper.ExecuteSql(sql))
        //     {
        //         return Ok();
        //     }
        //     throw new Exception("Failed to update post");
        // }

        [HttpDelete("Post/{postId}")]
        public IActionResult DeletePost(int postId)
        {
            string sql =
                @"EXEC WorkPointSchema.spPost_Delete @PostId = @PostIdParameter, 
                @UserId = " + this.User.FindFirst("userId")?.Value;

            DynamicParameters sqlParameters = new DynamicParameters();
            sqlParameters.Add("@PostIdParameter", postId, DbType.Int32);
            sqlParameters.Add(
                "@UserIdParameter",
                this.User.FindFirst("userId")?.Value,
                DbType.Int32
            );

            if (_dapper.ExecuteSql(sql))
            {
                return Ok();
            }

            throw new Exception("Failed to delete post");
        }
    }
}
