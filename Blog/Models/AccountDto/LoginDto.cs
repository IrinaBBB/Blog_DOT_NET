using System.ComponentModel.DataAnnotations;

namespace Blog.Models.AccountDto
{
    public class LoginDto
    {
        [Required]
        public string Username { get; set; }

        [Required]
        public string Password { get; set; }
    }
}