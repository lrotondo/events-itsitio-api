using System.ComponentModel.DataAnnotations;

namespace events_api.DTOs
{
    public class SignUpDTO
    {
        [Required]
        public string Email { get; set; }
        [Required]
        public string Password { get; set; }
        [Required]
        public string FirstName { get; set; }
        [Required]
        public string LastName { get; set; }
        public string UserName
        {
            get
            {
                return Email;
            }
        }
    }
}
