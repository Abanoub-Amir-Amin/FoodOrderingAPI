using FoodOrderingAPI.DTO;
using FoodOrderingAPI.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace FoodOrderingAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        public UserManager<User> UserManager { get; }

        public AccountController(UserManager<User> userManager)
        {
            UserManager = userManager;
        }

        [HttpPost("Register")]
        public async Task<IActionResult> RegisterAsync(RegisterDTO dto)
        {
            if(ModelState.IsValid)
            {
                User user = new User();
                user.UserName = dto.UserName;
                user.PhoneNumber = dto.Phone;
                user.Email = dto.Email;
                user.Role = dto.Role;
                user.CreatedAt = DateTime.Now;
                IdentityResult result = await UserManager.CreateAsync(user, dto.Password);
                if(result.Succeeded)
                {
                    return Ok();
                }
                else
                {
                    foreach(var error in result.Errors)
                    {
                        ModelState.AddModelError("Creation error",error.Description); 
                    }
                }
            }
            return BadRequest();
        }
        [HttpPost("Login")]
        public async Task<IActionResult> LoginAsync(LoginDTO dto)
        {
            if (!ModelState.IsValid)
                return BadRequest();

            var appUser = await UserManager.FindByNameAsync(dto.UserName);
            if (appUser == null || !await UserManager.CheckPasswordAsync(appUser, dto.Password))
                return BadRequest("Incorrect user name or password");

            var userClaims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.NameIdentifier, appUser.Id),
                new Claim(ClaimTypes.Name, appUser.UserName)
            };

            var userRoles = await UserManager.GetRolesAsync(appUser);
            foreach (var role in userRoles)
            {
                userClaims.Add(new Claim(ClaimTypes.Role, role));
            }

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("ASDF###@%#$Tf%$65464213sdFG353234324dfgadf"));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: "https://localhost:7060",     // Make sure this matches your middleware
                audience: "http://localhost:5455",     // And this too
                claims: userClaims,
                expires: DateTime.UtcNow.AddMinutes(30),
                signingCredentials: credentials
            );

            var tokenString = new JwtSecurityTokenHandler().WriteToken(token);

            Response.Cookies.Append("AuthToken", tokenString, new CookieOptions
            {
                HttpOnly = true,
                Secure = true, 
                SameSite = SameSiteMode.None,
                Expires = DateTimeOffset.UtcNow.AddMinutes(30)
            });

            return Ok(new
            {
                message = "Logged in successfully",
                expiration = token.ValidTo
            });
        }

    }
}
