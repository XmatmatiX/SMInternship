using SMInternship.Application.DTO.Users;
using SMInternship.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMInternship.Application.Interfaces
{
    public interface IUserService
    {
        User Login(LoginDTO dto);
        User Register(RegisterDTO dto);

    }
}
