using Microsoft.AspNetCore.Identity;
using Moq;
using SMInternship.Application.DTO.Users;
using SMInternship.Application.Services;
using SMInternship.Domain.Interfaces;
using SMInternship.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMInternship.Tests.Services
{
    public class UserServiceTests
    {

        [Fact]
        public void Login_NullInput_ReturnNull()
        {
            //Arrange
            var mockRepo = new Mock<IUserRepository>();
            var userService = new UserService(mockRepo.Object);

            LoginDTO data = null;

            //Act
            var result = userService.Login(data);

            //Assert
            Assert.Null(result);
            mockRepo.VerifyNoOtherCalls();
        }

        [Fact]
        public void Login_NoEmailInDatabase_ReturnNull()
        {
            //Arrange
            var mockRepo = new Mock<IUserRepository>();
            var userService = new UserService(mockRepo.Object);

            var data = new LoginDTO()
            {
                Email = "email@example.com",
                Password = "Password123"
            };

            mockRepo.Setup(r => r.GetUserByEmail(data.Email)).Returns((User?)null);

            //Act
            var result = userService.Login(data);

            //Assert
            Assert.Null(result);
            mockRepo.Verify(r => r.GetUserByEmail(It.IsAny<string>()), Times.Once);
            mockRepo.VerifyNoOtherCalls();
        }

        [Fact]
        public void Login_WrongPassword_ReturnNull()
        {
            //Arrange
            var mockRepo = new Mock<IUserRepository>();
            var userService = new UserService(mockRepo.Object);

            var data = new LoginDTO()
            {
                Email = "email@example.com",
                Password = "Password123"
            };
            var user = new User
            {
                Email = data.Email,
                Password = new PasswordHasher<User>().HashPassword(null, "correctpassword")
            };

            mockRepo.Setup(r => r.GetUserByEmail(data.Email)).Returns(user);

            //Act
            var result = userService.Login(data);

            //Assert
            Assert.Null(result);
            mockRepo.Verify(r => r.GetUserByEmail(It.IsAny<string>()), Times.Once);
            mockRepo.VerifyNoOtherCalls();
        }

        [Fact]
        public void Login_CorrectData_ReturnUserData()
        {
            //Arrange
            var mockRepo = new Mock<IUserRepository>();
            var userService = new UserService(mockRepo.Object);

            var data = new LoginDTO()
            {
                Email = "email@example.com",
                Password = "Password123"
            };
            var user = new User
            {
                ID = 5,
                IsActive = true,
                Nickname = "Nickname",
                Email = data.Email,
                Password = new PasswordHasher<User>().HashPassword(null, data.Password),
                PhoneNumber = "123 123 123"
            };

            mockRepo.Setup(r => r.GetUserByEmail(data.Email)).Returns(user);

            //Act
            var result = userService.Login(data);

            //Assert
            Assert.Equal(user.ID, result.ID);
            Assert.Equal(user.IsActive, result.IsActive);
            Assert.Equal(user.Nickname, result.Nickname);
            Assert.Equal(user.Email, result.Email);
            Assert.Equal("", result.Password);
            Assert.Equal(user.PhoneNumber, result.PhoneNumber);
            mockRepo.Verify(r => r.GetUserByEmail(It.IsAny<string>()), Times.Once);
            mockRepo.VerifyNoOtherCalls();
        }

        [Fact]
        public void Register_NullInput_ReturnNull()
        {
            //Arrange
            var mockRepo = new Mock<IUserRepository>();
            var userService = new UserService(mockRepo.Object);

            RegisterDTO data = null;

            //Act
            var result = userService.Register(data);

            //Assert
            Assert.Null(result);
            mockRepo.VerifyNoOtherCalls();
        }

        [Fact]
        public void Register_EmailIsTaken_ReturnNull()
        {
            //Arrange
            var mockRepo = new Mock<IUserRepository>();
            var userService = new UserService(mockRepo.Object);

            var data = new RegisterDTO()
            {
                Email = "email@example.com",
                Nickname = "Nickname",
                Password = "Password",
                PhoneNumber = "123 123 123"
            };

            mockRepo.Setup(r => r.IsEmailTaken(data.Email)).Returns(true);

            //Act
            var result = userService.Register(data);

            //Assert
            Assert.Null(result);
            mockRepo.Verify(r => r.IsEmailTaken(It.IsAny<string>()), Times.Once);
            mockRepo.VerifyNoOtherCalls();
        }

        [Fact]
        public void Register_NicknameIsTaken_ReturnNull()
        {
            //Arrange
            var mockRepo = new Mock<IUserRepository>();
            var userService = new UserService(mockRepo.Object);

            var data = new RegisterDTO()
            {
                Email = "email@example.com",
                Nickname = "Nickname",
                Password = "Password",
                PhoneNumber = "123 123 123"
            };

            mockRepo.Setup(r => r.IsEmailTaken(data.Email)).Returns(false);
            mockRepo.Setup(r => r.IsNicknameTaken(data.Nickname)).Returns(true);

            //Act
            var result = userService.Register(data);

            //Assert
            Assert.Null(result);
            mockRepo.Verify(r => r.IsEmailTaken(It.IsAny<string>()), Times.Once);
            mockRepo.Verify(r => r.IsNicknameTaken(It.IsAny<string>()), Times.Once);
            mockRepo.VerifyNoOtherCalls();
        }

        [Fact]
        public void Register_CorrectRegistration_ReturnNewUserData()
        {
            //Arrange
            var mockRepo = new Mock<IUserRepository>();
            var userService = new UserService(mockRepo.Object);

            var data = new RegisterDTO()
            {
                Email = "email@example.com",
                Nickname = "Nickname",
                Password = "Password",
                PhoneNumber = "123 123 123"
            };

            var user = new User()
            {
                ID = 5,
                IsActive = true,
                Email = data.Email,
                Nickname = data.Nickname,
                Password = new PasswordHasher<User>().HashPassword(null, data.Password),
                PhoneNumber = data.PhoneNumber
            };

            mockRepo.Setup(r => r.IsEmailTaken(data.Email)).Returns(false);
            mockRepo.Setup(r => r.IsNicknameTaken(data.Nickname)).Returns(false);
            mockRepo.Setup(r=>r.AddUser(It.IsAny<User>())).Returns(user);

            //Act
            var result = userService.Register(data);

            //Assert
            Assert.Equal(5, result.ID);
            Assert.True(result.IsActive);
            Assert.Equal("email@example.com", result.Email);
            Assert.Equal("Nickname", result.Nickname);
            Assert.Equal("123 123 123", result.PhoneNumber);
            mockRepo.Verify(r => r.IsEmailTaken(It.IsAny<string>()), Times.Once);
            mockRepo.Verify(r => r.IsNicknameTaken(It.IsAny<string>()), Times.Once);
            mockRepo.Verify(r => r.AddUser(It.IsAny<User>()), Times.Once);
            mockRepo.VerifyNoOtherCalls();
        }

    }
}
