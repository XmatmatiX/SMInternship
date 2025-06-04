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
        User GetUserByID(int ID);
        User AddUser(User user);
        User UpdateInfo(User user);
    }
}
