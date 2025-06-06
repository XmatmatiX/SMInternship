using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using SMInternship.Application.DTO.Users;
using SMInternship.Application.Interfaces;
using SMInternship.Domain.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace SMInternship.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IConfiguration _configuration;

        public UserController(IUserService userService, IConfiguration configuration)
        {
            _userService = userService;
            _configuration = configuration;
        }


        [HttpPost("Login")]
        public IActionResult Login([FromBody]LoginDTO dto)
        {
            var user = _userService.Login(dto);

            if (user == null)
                return BadRequest("Wrong data.");

            var result = CreateToken(user);

            return Ok(result);
        }

        [HttpPost("Register")]
        public IActionResult Register([FromBody]RegisterDTO dto)
        {
            var user = _userService.Register(dto);

            if (user == null)
                return BadRequest("Wrong data.");

            var result = CreateToken(user);

            return Ok(result);
        }

        private string CreateToken(User user)
        {
            var claims = new List<Claim>()
            {
                new Claim(ClaimTypes.Name, user.Nickname),
                new Claim(ClaimTypes.Email, user.Email)
            };

            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(_configuration.GetValue<string>("JwtSettings:Token")));

            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha512);

            var tokenDescriptor = new JwtSecurityToken(
                issuer : _configuration.GetValue<string>("JwtSettings:Issuer"),
                audience : _configuration.GetValue<string>("JwtSettings:audience"),
                claims : claims,
                expires : DateTime.UtcNow.AddDays(3),
                signingCredentials : credentials
                );

            return new JwtSecurityTokenHandler().WriteToken(tokenDescriptor);

        }
    }
}
