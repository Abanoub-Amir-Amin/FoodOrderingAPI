using FoodOrderingAPI;
using FoodOrderingAPI.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.IdentityModel.Tokens.Jwt;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly JwtTokenService _jwtTokenService;
    private readonly ApplicationDBContext _dbContext;
    private readonly IPasswordHasher<User> _passwordHasher;

    public AuthController(JwtTokenService jwtTokenService, ApplicationDBContext dbContext, IPasswordHasher<User> passwordHasher)
    {
        _dbContext = dbContext;
        _jwtTokenService = jwtTokenService;
        _passwordHasher = passwordHasher;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.UserName == request.Username );

        if (user == null)
            return Unauthorized("Invalid username or password");

        var passwordVerificationResult = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, request.Password);

        if (passwordVerificationResult != PasswordVerificationResult.Success)
            return Unauthorized("Invalid username or password");


        var token = _jwtTokenService.GenerateToken(user.Id, user.UserName, user.Role.ToString());
        
        Response.Cookies.Append("AuthToken", token, new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.None,
            Expires = DateTimeOffset.UtcNow.AddMinutes(30)
        });
        // Return role for client-side redirection (For angular redirection pages)
        return Ok(new
        {
            Token = token,
            Role = user.Role.ToString(),
            UserId = user.Id
        });
    }


}

