using Microsoft.AspNetCore.SignalR;

namespace FoodOrderingAPI.Hubs
{
    public class ChatHub:Hub
    {
        public async Task SendMessage(string userId, string message)
        {
            await Clients.User(userId).SendAsync("ReceiveMessage", message);
        }
    }
}
