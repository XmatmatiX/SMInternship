using Microsoft.AspNetCore.Identity;
using SMInternship.Application.DTO.Users;
using SMInternship.Application.Interfaces;
using SMInternship.Domain.Interfaces;
using SMInternship.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMInternship.Application.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;

        public UserService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public User Login(LoginDTO dto)
        {
            var user = _userRepository.GetUserByEmail(dto.Email);

            if (user == null)
                return null;

            var isPasswordCorrect = new PasswordHasher<User>().VerifyHashedPassword(user,user.Password,dto.Password);

            if (isPasswordCorrect == PasswordVerificationResult.Failed)
                return null;

            return user;

        }

        public User Register(RegisterDTO dto)
        {
            if (_userRepository.IsEmailTaken(dto.Email))
                return null;
            if (_userRepository.IsNicknameTaken(dto.Nickname))
                return null; 

            var user = new User()
            {
                Nickname = dto.Nickname,
                Email = dto.Email,
                PhoneNumber = dto.PhoneNumber,
                IsActive = true
            };

            var hashedPassword = new PasswordHasher<User>().HashPassword(user, dto.Password);

            user.Password = hashedPassword;

            user = _userRepository.AddUser(user);

            return user;
        }
    }
}
