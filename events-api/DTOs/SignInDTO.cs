using System.ComponentModel.DataAnnotations;

namespace events_api.DTOs
{
    public class SignInDTO
    {
        [Required]
        public string Email { get; set; }
        [Required]
        public string Password { get; set; }
    }
}
