using FoodOrderingAPI.Models;

namespace FoodOrderingAPI.Services
{
    public interface IConfirmationEmail
    {
        Task SendConfirmationEmail(string email, User user);
    }
}
