using System.ComponentModel.DataAnnotations;

namespace Core.Models
{
    public class Security : BaseModel
    {
        [Required]
        public virtual User User { get; set; }
        [Required]
        public int UserId { get; set; }
        [Required]
        public string HashedPassword { get; set; }
        [Required]
        public string Salt { get; set; }
    }
}
