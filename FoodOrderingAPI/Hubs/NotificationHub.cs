using Microsoft.AspNetCore.SignalR;

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
        
        public override Task OnConnectedAsync()
        {
            // Logic when a client connects
            // save connID and user id to User_ConnectionId table
            return base.OnConnectedAsync();
        }
        public override Task OnDisconnectedAsync(Exception? exception)
        {
            // Logic when a client disconnects
            // remove connID from User_ConnectionId table
            return base.OnDisconnectedAsync(exception);
        }
    }
}
