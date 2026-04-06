using DotnetAPI.Models;

namespace DotnetAPI.Services
{
    public interface IPostService
    {
        IEnumerable<Post> GetPosts(int postId, int userId, string searchParam);
        IEnumerable<Post> GetMyPosts(int userId);
        bool UpsertPost(Post post, int userId);
        bool DeletePost(int postId, int userId);
    }
}
