namespace DotnetAPI.Dtos
{
    public partial class PostToEditDTO
    {
        public int PostID { get; set; }

        public string PostTitle { get; set; }
        public string PostContent { get; set; }
       
                public PostToEditDTO()
                {
                    if(PostTitle == null)
                    {
                        PostTitle = "";
                    }
                    if(PostContent == null)
                    {
                        PostContent = "";
                    }
                }
    }
}