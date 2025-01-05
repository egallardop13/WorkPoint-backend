namespace DotnetAPI.Dtos
{
    public partial class UpsertDto
    {
        public int Response { get; set; }

        public UpsertDto()
        {
            if (Response == 0)
            {
                Response = -1;
            }
        }
    }
}
