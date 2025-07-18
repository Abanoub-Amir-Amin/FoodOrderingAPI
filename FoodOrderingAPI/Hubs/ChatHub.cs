using FoodOrderingAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;

namespace FoodOrderingAPI.Hubs
{
    public class ChatHub:Hub
    {
        public ApplicationDBContext DBContext { get; }
        public UserManager<User> UserManager { get; }

        public ChatHub(ApplicationDBContext dBContext, UserManager<User> userManager)
        {
            DBContext = dBContext;
            UserManager = userManager;

        }

        //public async Task SendMessage(string userId, string message)
        //{
        //    Console.WriteLine($"Trying to send message to userId: {userId}");
        //    await Clients.User(userId).SendAsync("ReceiveMessage", message);
        //}

        public override async Task OnConnectedAsync()
        {
            var id = Context.UserIdentifier;
            if (id == null)
            {
                Console.WriteLine("Not authenticated.");
                return;
            }
            var userConnectionId = new User_ConnectionId
            {
                UserId = id,
                ConnectionId = Context.ConnectionId
            };

            DBContext.User_ConnectionId.Add(userConnectionId);
            await DBContext.SaveChangesAsync();

            await base.OnConnectedAsync();
        }
        public override Task OnDisconnectedAsync(Exception? exception)
        {
            var id = Context.UserIdentifier;
            var row = DBContext.User_ConnectionId.Find(id, Context.ConnectionId);
            DBContext.Remove(row);
            DBContext.SaveChanges();
            return Task.CompletedTask;
        }
    }
}
