using SMInternship.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMInternship.Domain.Interfaces
{
    public interface IUserRepository
    {
        User GetUserByEmail(string email);
        User GetUserByNickname(string nickname);
        User GetUserByID(int ID);
        User AddUser(User user);
        User UpdateInfo(User user);
        bool IsEmailTaken(string email);
        bool IsNicknameTaken(string nickname);
    }
}
