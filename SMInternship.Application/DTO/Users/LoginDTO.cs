using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMInternship.Application.DTO.Users
{
    public class LoginDTO
    {
        [EmailAddress(ErrorMessage = "Wrong email address")]
        public string Email { get; set; }
        public string Password { get; set; }

    }
}
