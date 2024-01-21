using Core.DataValidators;
using Core.Interfaces;
using Core.Models;
using Infrastructure.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.ComponentModel.DataAnnotations;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace inventory_management_system_backend.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly ISecurityService _securityService;
        private readonly ILogger<UserController> _logger;
        private readonly IConfiguration _configuration;
        public UserController(ILogger<UserController> logger, IUserService userService, ISecurityService securityService, IConfiguration configuration)
        {
            _logger = logger;
            _userService = userService;
            _securityService = securityService;
            _configuration = configuration;
        }

        [HttpGet]
        public async Task<IActionResult> Get([Required][FromQuery] string email)
        {
            var user = await _userService.GetUserByEmail(email);

            if (user is null)
            {
                return NotFound("No user exists with that email");
            }

            return Ok(user);
        }

        [HttpPost("Authenticate")]
        [AllowAnonymous]
        public async Task<IActionResult> Authenticate(CreateUserValidator userInfo)
        {
            var userInDb = await _userService.GetUserByEmail(userInfo.Email);
            if (userInDb is null) return BadRequest(new
            {
                Message = "Invalid credentials",
            });

            var securityInDb = await _securityService.GetByUserEmail(userInfo.Email);
            if (securityInDb is null) return BadRequest(new
            {
                Message = "An unexpected error has occured.",
            });

            if (SecurityService.VerifyPassword(userInfo.Password, securityInDb))
            {
                // Generate JWT
                var issuer = _configuration["Jwt:Issuer"];
                var audience = _configuration["Jwt:Audience"];
                var key = Encoding.ASCII.GetBytes(_configuration["Jwt:Key"]);

                var tokenDescriptor = new SecurityTokenDescriptor
                {
                    Subject = new ClaimsIdentity(new[]
                        {
                            new Claim("Id", Guid.NewGuid().ToString()),
                            new Claim(JwtRegisteredClaimNames.Email, userInfo.Email),
                            new Claim(JwtRegisteredClaimNames.Jti,
                            Guid.NewGuid().ToString())
                        }),
                    Expires = DateTime.UtcNow.AddMinutes(5),
                    Issuer = issuer,
                    Audience = audience,
                    SigningCredentials = new SigningCredentials
                        (new SymmetricSecurityKey(key),
                        SecurityAlgorithms.HmacSha512Signature)
                };

                var tokenHandler = new JwtSecurityTokenHandler();
                var token = tokenHandler.CreateToken(tokenDescriptor);
                var jwtToken = tokenHandler.WriteToken(token);
                var stringToken = tokenHandler.WriteToken(token);

                var response = new
                {
                    Success = true,
                    Token = stringToken
                };

                return Ok(response);
            }
            return BadRequest(new
            {
                Message = "Invalid credentials",
            });
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Post(CreateUserValidator userInfo)
        {
            var userInDb = await _userService.GetUserByEmail(userInfo.Email);
            if (userInDb is not null) return BadRequest(new
            {
                Message = "A user with that email already exists"
            });

            if (!await _userService.Create(userInfo)) return BadRequest(new
            {
                Message = "Something went wrong creating your account"
            });

            var user = _userService.GetUserByEmail(userInfo.Email).Result;

            var userSecurity = new Security()
            {
                UserId = user.Id,
                HashedPassword = SecurityService.HashPassword(userInfo.Password, out byte[] salt),
                Salt = Convert.ToHexString(salt)
            };

            if (!await _securityService.Create(userSecurity)) return BadRequest(new
            {
                Message = "Something went wrong creating your account"
            });

            return Ok(new
            {
                Message = "User has been created!"
            });
        }

        [HttpPut]
        public async Task<IActionResult> Update(UpdateUserValidator updatedUserInfo)
        {
            // Authenticate first

            var userInDb = await _userService.GetUserByEmail(updatedUserInfo.Email);
            if (userInDb is null)
            {
                return BadRequest("Something went wrong");
            }

            if (await _userService.UpdateUser(updatedUserInfo))
            {
                return Ok("User has been updated");
            }

            return BadRequest("Something went wrong");
        }

        [HttpDelete]
        public async Task<IActionResult> Delete(int userId)
        {
            var userInDb = await _userService.GetUserById(userId);
            if (userInDb is null)
            {
                return BadRequest("Something went wrong");
            }

            if (await _userService.DeleteUser(userId))
            {
                return Ok("User has been deleted");
            }

            return BadRequest("Something went wrong");
        }


        public static void ReadJWT(string jwt)
        {
            var handler = new JwtSecurityTokenHandler();
            var token = handler.ReadJwtToken(jwt);

            var keyId = token.Header.Kid;
            var audience = token.Audiences.ToList();
            var claims = token.Claims.Select(claim => (claim.Type, claim.Value)).ToList();


        }
    }
}
