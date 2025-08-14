using FoodOrderingAPI.Models;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace FoodOrderingAPI.Hubs
{
    public class NotificationHub:Hub
    {
        public ApplicationDBContext DbContext { get; }

        public NotificationHub(ApplicationDBContext dbContext)
        {
            DbContext = dbContext;
        }
        public async Task SendNotification(string userId, string message)
        {
            await Clients.User(userId).SendAsync("ReceiveNotification", message);
        }
        public async Task SendToAll(string message)
        {
            await Clients.All.SendAsync("ReceivePublicNotification", message);
        }

        //public async Task AddGroup(string groupName)
        //{
        //    await Groups.AddToGroupAsync(Context.ConnectionId, groupName);
        //}
        //public async Task SendToGroup(string GroupName, string message)
        //{
        //    await Clients.Group(GroupName).SendAsync("ReceiveGroupNotification", message);
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

            DbContext.User_ConnectionId.Add(userConnectionId);
            await DbContext.SaveChangesAsync();

            await base.OnConnectedAsync();
        }
        public override Task OnDisconnectedAsync(Exception? exception)
        {
            var id = Context.UserIdentifier;
            var row = DbContext.User_ConnectionId.Find(id, Context.ConnectionId);
            DbContext.Remove(row);
            DbContext.SaveChanges();
            return Task.CompletedTask;
        }
    }
}
