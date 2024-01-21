using System.ComponentModel.DataAnnotations;

namespace Core.DataValidators
{
    public class CreateUserValidator
    {
        [Required]
        public string Email { get; set; }
        [Required]
        public string Password { get; set; }
    }
}