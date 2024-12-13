namespace DotnetAPI.Models
{
    public class PaginatedUsers
    {
        public IEnumerable<UserComplete> Users { get; set; }
        public int TotalPages { get; set; }

        public PaginatedUsers()
        {
            if (Users == null)
            {
                Users = new List<UserComplete>();
            }
        }
    }
}
