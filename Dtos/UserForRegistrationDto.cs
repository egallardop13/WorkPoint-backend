using System.ComponentModel.DataAnnotations;

namespace DotnetAPI.Dtos
{
    public partial class UserForRegistrationDto
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = "";

        [Required]
        [MinLength(8)]
        public string Password { get; set; } = "";

        [Required]
        [Compare("Password")]
        public string PasswordConfirm { get; set; } = "";

        [Required]
        [StringLength(50)]
        public string FirstName { get; set; } = "";

        [Required]
        [StringLength(50)]
        public string LastName { get; set; } = "";

        [Required]
        public string Gender { get; set; } = "";

        [Required]
        [StringLength(100)]
        public string JobTitle { get; set; } = "";

        [Required]
        [StringLength(100)]
        public string Department { get; set; } = "";

        [Range(0, double.MaxValue)]
        public decimal Salary { get; set; }

        public DateTime DateHired { get; set; }

        public DateTime DateExited { get; set; }
    }
}
