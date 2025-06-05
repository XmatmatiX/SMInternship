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


        /// <summary>
        /// Add new user to databasse
        /// </summary>
        /// <param name="user">New user data</param>
        /// <returns></returns>
        public User AddUser(User user)
        {
            var us = _context.Users.Add(user);

            _context.SaveChanges();

            return us.Entity;
        }


        /// <summary>
        /// Find user with specified email
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        public User GetUserByEmail(string email)
        {
            User user = _context.Users.Where(u => u.Email.ToLower() == email.ToLower()).FirstOrDefault();

            return user;
        }


        /// <summary>
        /// Find user with specified ID
        /// </summary>
        /// <param name="ID"></param>
        /// <returns></returns>
        public User GetUserByID(int ID)
        {
            User user = _context.Users.Where(u => u.ID == ID).FirstOrDefault();

            return user;
        }


        /// <summary>
        /// Find user with specified nickname
        /// </summary>
        /// <param name="nickname"></param>
        /// <returns></returns>
        public User GetUserByNickname(string nickname)
        {
            User user = _context.Users.Where(u => u.Nickname == nickname).FirstOrDefault();

            return user;
        }


        /// <summary>
        /// Check if email is already taken.
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        public bool IsEmailTaken(string email)
        {
            var result = _context.Users.Any(u => u.Email.ToLower() == email.ToLower());

            return result;
        }


        /// <summary>
        /// Check if nickname is already taken.
        /// </summary>
        /// <param name="nickname"></param>
        /// <returns></returns>
        public bool IsNicknameTaken(string nickname)
        {
            var result = _context.Users.Any(u => u.Nickname.ToLower() == nickname.ToLower());

            return result;
        }


        /// <summary>
        /// Update data about user basing od ID
        /// </summary>
        /// <param name="user">New user data</param>
        /// <returns></returns>
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
