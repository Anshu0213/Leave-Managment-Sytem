using System.ComponentModel.DataAnnotations;

namespace Leave_Managment_Sytem.Models
{
    public class RegisterRequest
    {
        [Required]
        public string Name { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [MinLength(6)]
        public string Password { get; set; }

        [Required]
        public string Role { get; set; }
    }
}
