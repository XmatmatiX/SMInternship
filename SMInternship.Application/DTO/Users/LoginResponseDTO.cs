using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMInternship.Application.DTO.Users
{
    public class LoginResponseDTO
    {
        public string JWT { get; set; }
        public string Nickname { get; set; }
        public string Email { get; set; }
    }
}
