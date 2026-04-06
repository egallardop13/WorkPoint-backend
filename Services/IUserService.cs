using DotnetAPI.Models;

namespace DotnetAPI.Services
{
    public interface IUserService
    {
        IEnumerable<UserComplete> GetUsers(int userId, bool isActive);
        object? GetUsersWithPagination(int page, int limit, string? query, string? sort);
        int UpsertUser(UserComplete user);
        bool DeleteUser(int userId);
    }
}
