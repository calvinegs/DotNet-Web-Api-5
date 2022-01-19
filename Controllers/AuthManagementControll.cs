using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Todo5.Models;

namespace Todo5.Controllers
{
    [Route("api/[controller]")] // api/authManagement
    [ApiController]
    public class AuthManagementController : ControllerBase
    {
        private readonly UserManager<IdentityUser> _userManager;
        // private readonly AppSettings _appSettings;

        public AuthManagementController(
            UserManager<IdentityUser> userManager)
        // public AuthManagementController(
        //     UserManager<IdentityUser> userManager,
        //     IOptionsMonitor<AppSettings> optionsMonitor)
        {
            _userManager = userManager;
            // _appSettings = optionsMonitor.CurrentValue;
        }

        [HttpPost]
        [Route("Register")]
        public async Task<IActionResult> Register([FromBody] UserRegistrationDto user)
        {
            if (ModelState.IsValid)
            {
                // We can utilise the model
                var existingUser = await _userManager.FindByEmailAsync(user.Email);

                if (existingUser != null)
                {
                    return BadRequest(new RegistrationResponse()
                    {
                        Errors = new List<string>() {
                                "Email already in use"
                            },
                        Success = false
                    });
                }

                var newUser = new IdentityUser() { Email = user.Email, UserName = user.Username };
                var isCreated = await _userManager.CreateAsync(newUser, user.Password);
                if (isCreated.Succeeded)
                {
                    // var jwtToken = GenerateJwtToken(newUser);

                    return Ok(new RegistrationResponse()
                    {
                        Success = true,
                        // Token = jwtToken
                    });
                }
                else
                {
                    return BadRequest(new RegistrationResponse()
                    {
                        Errors = isCreated.Errors.Select(x => x.Description).ToList(),
                        Success = false
                    });
                }
            }

            return BadRequest(new RegistrationResponse()
            {
                Errors = new List<string>() {
                        "Invalid payload"
                    },
                Success = false
            });
        }

        [HttpPost]
        [Route("Login")]
        public async Task<IActionResult> Login([FromBody] UserLoginRequest user)
        {
            if (ModelState.IsValid)
            {
                var existingUser = await _userManager.FindByEmailAsync(user.Email);

                if (existingUser == null)
                {
                    return BadRequest(new RegistrationResponse()
                    {
                        Errors = new List<string>() {
                                "Invalid login request"
                            },
                        Success = false
                    });
                }

                var isCorrect = await _userManager.CheckPasswordAsync(existingUser, user.Password);

                if (!isCorrect)
                {
                    return BadRequest(new RegistrationResponse()
                    {
                        Errors = new List<string>() {
                            "Invalid login request"
                        },
                        Success = false
                    });
                }

                // var jwtToken = GenerateJwtToken(existingUser);

                return Ok(new RegistrationResponse()
                {
                    Success = true,
                    // Token = jwtToken
                });
            }

            return BadRequest(new RegistrationResponse()
            {
                Errors = new List<string>() {
                        "Invalid payload"
                    },
                Success = false
            });
        }

        /*         private string GenerateJwtToken(IdentityUser user)
                {
                    var jwtTokenHandler = new JwtSecurityTokenHandler();

                    var key = Encoding.ASCII.GetBytes(_appSettings.Secret);

                    var claims = new ClaimsIdentity(new[]
                    {
                        new Claim("Id", user.Id),
                        new Claim(JwtRegisteredClaimNames.Email, user.Email),
                        new Claim(JwtRegisteredClaimNames.Sub, user.Email),
                        new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
                    });

                    var tokenDescriptor = new SecurityTokenDescriptor
                    {
                        // Subject = new ClaimsIdentity(new []
                        // {
                        //     new Claim("Id", user.Id), 
                        //     new Claim(JwtRegisteredClaimNames.Email, user.Email),
                        //     new Claim(JwtRegisteredClaimNames.Sub, user.Email),
                        //     new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
                        // }),
                        Subject = claims,
                        Expires = DateTime.UtcNow.AddHours(6),
                        SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
                    };

                    var token = jwtTokenHandler.CreateToken(tokenDescriptor);
                    var jwtToken = jwtTokenHandler.WriteToken(token);

                    return jwtToken;
                } */
    }
}