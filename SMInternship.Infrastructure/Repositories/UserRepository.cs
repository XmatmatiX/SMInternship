using SMInternship.Domain.Interfaces;
using SMInternship.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMInternship.Infrastructure.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly Context _context;

        public UserRepository(Context context)
        {
            _context = context;
        }

        public User AddUser(User user)
        {
            var us = _context.Users.Add(user);

            _context.SaveChanges();

            return us.Entity;
        }

        public User GetUserByEmail(string email)
        {
            User user = _context.Users.Where(u => u.Email == email).FirstOrDefault();

            return user;
        }

        public User GetUserByID(int ID)
        {
            User user = _context.Users.Where(u => u.ID == ID).FirstOrDefault();

            return user;
        }



        public User UpdateInfo(User user)
        {
            _context.Attach(user);
            _context.Entry(user).Property(u => u.IsActive).IsModified = true;
            _context.Entry(user).Property(u => u.Password).IsModified = true;
            _context.Entry(user).Property(u => u.Nickname).IsModified = true;
            _context.Entry(user).Property(u => u.Email).IsModified = true;
            _context.Entry(user).Property(u => u.PhoneNumber).IsModified = true;

            _context.SaveChanges();
            return user;
        }
    }
}
