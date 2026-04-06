using System.ComponentModel.DataAnnotations;

namespace DotnetAPI.Dtos
{
    public partial class UserForLoginDto
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = "";

        [Required]
        [MinLength(8)]
        public string Password { get; set; } = "";
    }
}
