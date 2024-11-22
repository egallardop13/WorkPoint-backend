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

        [HttpGet("Posts")]
        public IEnumerable<Post> GetPosts()
        {
            string sql = @"SELECT [PostId],
                    [UserId],
                    [PostTitle],
                    [PostContent],
                    [PostCreated],
                    [PostUpdated] 
                FROM UsersSchema.Posts;";
            IEnumerable<Post> posts = _dapper.LoadData<Post>(sql);
            return posts;
        }

         [HttpGet("PostSingle/{postId}")]
        public Post GetPostSingle(int postId)
        {
            string sql = @"SELECT [PostId],
                    [UserId],
                    [PostTitle],
                    [PostContent],
                    [PostCreated],
                    [PostUpdated] 
                FROM UsersSchema.Posts WHERE PostId = " + postId.ToString();
            return _dapper.LoadDataSingle<Post>(sql);
        }

        [HttpGet("PostsByUser/{userId}")]
        public IEnumerable<Post> GetPostsByUser(int userId)
        {
            string sql = @"SELECT [PostId],
                    [UserId],
                    [PostTitle],
                    [PostContent],
                    [PostCreated],
                    [PostUpdated] 
                FROM UsersSchema.Posts WHERE PostId = " + userId.ToString();
            IEnumerable<Post> posts = _dapper.LoadData<Post>(sql);
            return posts;
        }

        [HttpGet("MyPosts")]
        public IEnumerable<Post> GetMyPosts(int userId)
        {
            string sql = @"SELECT [PostId],
                    [UserId],
                    [PostTitle],
                    [PostContent],
                    [PostCreated],
                    [PostUpdated] 
                FROM UsersSchema.Posts WHERE PostId = " + (this.User.FindFirst("userId")?.Value);
            IEnumerable<Post> posts = _dapper.LoadData<Post>(sql);
            return posts;
        }

         [HttpGet("PostsBySearch/{searchParam}")]
        public IEnumerable<Post> PostsBySearch(string searchParam)
        {
            string sql = @"SELECT [PostId],
                    [UserId],
                    [PostTitle],
                    [PostContent],
                    [PostCreated],
                    [PostUpdated] 
                FROM UsersSchema.Posts
                 WHERE PostTitle LIKE '%" + searchParam + "%'" +
                 " OR PostContent LIKE '%" + searchParam + "%'";
            IEnumerable<Post> posts = _dapper.LoadData<Post>(sql);
            return posts;
        }

        [HttpPost("Post")]

        public IActionResult AddPost(PostToAddDTO postToAdd)
        {
            string sql = @"INSERT INTO UsersSchema.Posts(
                [UserId],
                [PostTitle],
                [PostContent],
                [PostCreated],
                [PostUpdated]
            ) VALUES (" +
                (this.User.FindFirst("userId")?.Value ?? "0")
                 + ", '" + postToAdd.PostTitle
                  + "', '" + postToAdd.PostContent
                   + "', GETDATE(), GETDATE())";
            if (_dapper.ExecuteSql(sql))
            {
                return Ok();
            }
            throw new Exception("Failed to create new post");
        }

         [HttpPut("Post")]

        public IActionResult EditPost(PostToEditDTO postToEdit)
        {
            string sql = @"
            UPDATE UsersSchema.Posts
                SET PostTitle = '" + postToEdit.PostTitle +
                "', PostContent = '" + postToEdit.PostContent +
                @"', PostUpdated = GETDATE()
                    WHERE PostId = " + postToEdit.PostID.ToString() +
                    " AND UserId = " + (this.User.FindFirst("userId")?.Value);
            if (_dapper.ExecuteSql(sql))
            {
                return Ok();
            }
            throw new Exception("Failed to update post");

        }

        [HttpDelete("Post/{postId}")]
        public IActionResult DeletePost(int postId)
        {
            string sql = @"DELETE FROM UsersSchema.Posts
                WHERE PostId = " + postId.ToString() +
                " AND UserId = " + (this.User.FindFirst("userId")?.Value);   
            if (_dapper.ExecuteSql(sql))
            {
                return Ok();
            }
            throw new Exception("Failed to delete post");
        }

    }
}