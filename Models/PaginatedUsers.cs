namespace DotnetAPI.Models
{
    public class PaginatedUsers
    {
        public IEnumerable<UserComplete> Users { get; set; } = new List<UserComplete>();
        public int TotalPages { get; set; }
    }
}
