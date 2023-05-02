using System.ComponentModel.DataAnnotations;

namespace FecebookAPI.Models
{
    public class RegisterModel
    {
        [StringLength(100)]
        public string Username { get; set; }

        [StringLength(11)]
        public string MobileNumber { get; set; }

        [StringLength(50)]
        public string Password { get; set; }
    }
}
 