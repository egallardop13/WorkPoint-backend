namespace DotnetAPI.Dtos
{
    public partial class PostToAddDTO
    {
        
        public string PostTitle { get; set; }
        public string PostContent { get; set; }

                public PostToAddDTO()
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