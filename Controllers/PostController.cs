using DotnetAPI.Models;
using DotnetAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DotnetAPI.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class PostController : ControllerBase
    {
        private readonly IPostService _postService;

        public PostController(IPostService postService)
        {
            _postService = postService;
        }

        [HttpGet("Posts/{postId}/{userId}/{searchParam}")]
        public IEnumerable<Post> GetPosts(
            int postId = 0,
            int userId = 0,
            string searchParam = "None"
        )
        {
            return _postService.GetPosts(postId, userId, searchParam);
        }

        [HttpGet("MyPosts")]
        public IEnumerable<Post> GetMyPosts()
        {
            int userId = int.Parse(this.User.FindFirst("userId")?.Value ?? "0");
            return _postService.GetMyPosts(userId);
        }

        [HttpPut("UpsertPost")]
        public IActionResult UpsertPost(Post postToUpsert)
        {
            int userId = int.Parse(this.User.FindFirst("userId")?.Value ?? "0");
            if (_postService.UpsertPost(postToUpsert, userId))
            {
                return Ok();
            }
            throw new Exception("Failed to Upsert a post");
        }

        [HttpDelete("Post/{postId}")]
        public IActionResult DeletePost(int postId)
        {
            int userId = int.Parse(this.User.FindFirst("userId")?.Value ?? "0");
            if (_postService.DeletePost(postId, userId))
            {
                return Ok();
            }
            throw new Exception("Failed to delete post");
        }
    }
}
