using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMInternship.Application.DTO.Users
{
    public class UserDetailsDTO
    {
        public int ID { get; set; }
        public string Nickname { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }
        public string? PhoneNumber { get; set; }
    }
}
