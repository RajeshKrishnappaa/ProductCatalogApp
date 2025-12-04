using System.ComponentModel.DataAnnotations;

namespace ProdApp.DTOS
{
    public class RegisterDTO
    {
        [Required]
        public string UserName { get; set; }
        [Required]
        public string Password { get; set; }
        [Required]
        public string Email { get; set; }
        //[Required]
        //public string? Role { get; set; }
    }
}
