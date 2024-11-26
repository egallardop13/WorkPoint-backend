using DotnetAPI.Data;
using DotnetAPI.Dtos;
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
            string sql = @"EXEC UsersSchema.spPost_Get";
            string parameters = "";
            if (postId != 0)
            {
                parameters += ", @PostId = " + postId.ToString();
            }
            if (userId != 0)
            {
                parameters += ", @UserId = " + userId.ToString();
            }
            if (searchParam.ToLower() != "none")
            {
                parameters += ", @SearchValue = '" + searchParam + "'";
            }

            if (parameters.Length > 0)
            {
                sql += parameters.Substring(1);
            }
            Console.WriteLine(sql);
            return _dapper.LoadData<Post>(sql);
        }

        [HttpGet("MyPosts")]
        public IEnumerable<Post> GetMyPosts()
        {
            string sql =
                @"EXEC UsersSchema.spPost_Get @PostId = " + this.User.FindFirst("userId")?.Value;
            IEnumerable<Post> posts = _dapper.LoadData<Post>(sql);
            return posts;
        }

        [HttpPut("UpsertPost")]
        public IActionResult UpsertPost(Post postToUpsert)
        {
            string sql =
                @"EXEC UsersSchema.spPosts_Upsert 
                @UserId ="
                + this.User.FindFirst("userId")?.Value
                + ", @PostTitle = '"
                + postToUpsert.PostTitle
                + "', @PostContent = '"
                + postToUpsert.PostContent
                + "'";

            if (postToUpsert.PostId != 0)
            {
                sql += ", @PostId = " + postToUpsert.PostId;
            }
            if (_dapper.ExecuteSql(sql))
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
        //     UPDATE UsersSchema.Posts
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
                @"EXEC UsersSchema.spPost_Delete @PostId = "
                + postId.ToString()
                + ", @UserId = "
                + this.User.FindFirst("userId")?.Value;

            if (_dapper.ExecuteSql(sql))
            {
                return Ok();
            }

            throw new Exception("Failed to delete post");
        }
    }
}
