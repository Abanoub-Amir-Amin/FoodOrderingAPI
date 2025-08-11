using FoodOrderingAPI;
using FoodOrderingAPI.DTO;
using FoodOrderingAPI.Models;
using FoodOrderingAPI.Services;
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
    private readonly UserManager<User> _userManager;
    private readonly IConfiguration _configuration;
    private readonly IEmailSender _emailSender;

    public AuthController(
        JwtTokenService jwtTokenService,
        ApplicationDBContext dbContext,
        UserManager<User> userManager,
        IConfiguration configuration,
        IEmailSender emailSender)
    {
        _jwtTokenService = jwtTokenService;
        _dbContext = dbContext;
        _userManager = userManager;
        this._configuration = configuration;
        this._emailSender = emailSender;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        var user = await _userManager.FindByNameAsync(request.Username);

        if (user == null)
            return Unauthorized("Invalid username or password");

        //// Allow login for Admins without email confirmation
        if (!(user.Role.ToString() == "Admin") && !await _userManager.IsEmailConfirmedAsync(user))
            return Unauthorized("Email not confirmed. Please check your inbox.");

        // Check password
        if (!await _userManager.CheckPasswordAsync(user, request.Password))
            return Unauthorized("Invalid username or password");

        // Generate token
        var token = _jwtTokenService.GenerateToken(user.Id, user.UserName, user.Role.ToString());

        Response.Cookies.Append("AuthToken", token, new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.None,
            Expires = DateTimeOffset.UtcNow.AddDays(5)
        });

        return Ok(new
        {
            Token = token,
            Role = user.Role.ToString(),
            UserId = user.Id
        });
    }
    [HttpPost("forgot-password")]
    public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordDTO model)
    {
        if (string.IsNullOrEmpty(model.Email))
            return BadRequest("Email is required");
        var user = await _userManager.FindByEmailAsync(model.Email);
        if (user == null)
            return NotFound("User not found");
        // Generate password reset token
        var token = await _userManager.GeneratePasswordResetTokenAsync(user);
        var resetLink = $"{_configuration["AppSettings:FrontendUrl"]}/new-password?token={Uri.EscapeDataString(token)}&email={Uri.EscapeDataString(model.Email)}";
        // Send email with reset link (implementation not shown)
        await _emailSender.SendEmailAsync(model.Email, "Reset Password", $"Click here to reset your password: {resetLink}");
        return Ok("Password reset link sent to your email");
    }

    [HttpPost("reset-password")]
    public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDTO model)
    {
        if (string.IsNullOrEmpty(model.Email) || string.IsNullOrEmpty(model.Token) || string.IsNullOrEmpty(model.NewPassword))
            return BadRequest("Invalid request");

        var user = await _userManager.FindByEmailAsync(model.Email);
        if (user == null)
            return NotFound("User not found");

        var result = await _userManager.ResetPasswordAsync(user, model.Token, model.NewPassword);
        if (!result.Succeeded)
            return BadRequest(result.Errors.FirstOrDefault()?.Description);

        return Ok("Password has been reset successfully");
    }
}
