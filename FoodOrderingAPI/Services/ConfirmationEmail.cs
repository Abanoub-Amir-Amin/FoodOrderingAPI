using FoodOrderingAPI.Models;
using Microsoft.AspNetCore.Identity;

namespace FoodOrderingAPI.Services
{
    public class ConfirmationEmail:IConfirmationEmail
    {
        private readonly IEmailSender emailSender;
        private readonly UserManager<User> userManager;
        private readonly IConfiguration _configuration;

        public ConfirmationEmail(IEmailSender emailSender, UserManager<User> userManager, IConfiguration configuration)
        {
            this.emailSender = emailSender;
            this.userManager = userManager;
            this._configuration = configuration;
        }
        public async Task SendConfirmationEmail(string email, User user)
        {
            // Generate the email confirmation token
            var token = await userManager.GenerateEmailConfirmationTokenAsync(user);
            // Build the confirmation callback URL
            string confirmationLink = $"{_configuration["AppSettings:FrontendUrl"]}/confirm-email?userId={user.Id}&token={Uri.EscapeDataString(token)}";
            // Craft a more polished email subject
            var subject = "Welcome to Food Ordering App! Please Confirm Your Email";
            // Create a professional HTML body
            // Customize inline styles, text, and branding as needed
            var messageBody = $@"
                <div style=""font-family:Arial,Helvetica,sans-serif;font-size:16px;line-height:1.6;color:#333;"">
                    <p>Hi {user.UserName},</p>
                    <p>Thank you for creating an account at <strong>Presto App</strong>.
                    To start enjoying all of our features, please confirm your email address by clicking the button below:</p>
                    <p>
                        <a href=""{confirmationLink}"" 
                           style=""background-color:#007bff;color:#fff;padding:10px 20px;text-decoration:none;
                                  font-weight:bold;border-radius:5px;display:inline-block;"">
                            Confirm Email
                        </a>
                    </p>
                    <p>If the button doesn’t work for you, copy and paste the following URL into your browser:
                        <br />
                        <a href=""{confirmationLink}"" style=""color:#007bff;text-decoration:none;"">{confirmationLink}</a>
                    </p>
                    <p>If you did not sign up for this account, please ignore this email.</p>
                    <p>Thanks,<br />
                    Presto App Team</p>
                </div>
            ";
            //Send the Confirmation Email to the User Email Id
            await emailSender.SendEmailAsync(email, subject, messageBody, true);
        }
    }
}
