namespace DotnetAPI.Models
{
    public class UserForRegistration
    {
        public int UserId { get; set; }
        public string Email { get; set; }     
        public string Password { get; set; }
        public string PasswordConfirm { get; set; }

        public UserForRegistration()
        {
            if (Email == null)
            {
                Email = "";
            }
            if (Password == null)
            {
                Password = "";
            }
            if (PasswordConfirm == null)
            {
                PasswordConfirm = "";
            }
        
        }

    }
}