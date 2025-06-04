using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMInternship.Domain.Models
{
    public class User
    {
        [Key]
        public int ID { get; set; }
        public bool IsActive { get; set; }

        [Required]
        public string Nickname { get; set; }

        [Required]
        [MinLength(6, ErrorMessage = "Password too short. Min 6 letters")]
        [MaxLength(18, ErrorMessage = "Password too long. Max 6 letters")]
        public string Password { get; set; }

        [Required]
        [EmailAddress(ErrorMessage = "Wrong email address")]
        public string Email { get; set; }

        [Phone(ErrorMessage = "Wrong phone number")]
        public string PhoneNumber { get; set; }
    }
}
