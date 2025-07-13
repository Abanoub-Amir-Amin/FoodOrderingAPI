using Microsoft.AspNetCore.SignalR;

namespace FoodOrderingAPI.Hubs
{
    public class NotificationHub:Hub
    {
        public async Task SendNotification(string userId, string message)
        {
            await Clients.User(userId).SendAsync("ReceiveNotification", message);
        }
        public async Task SendToAll(string message)
        {
            await Clients.All.SendAsync("ReceivePublicNotification", message);
        }
        public async Task JoinGroup(string groupName)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, groupName);
        }
        public async Task SendToGroup(string GroupName, string message)
        {
            await Clients.Group(GroupName).SendAsync("ReceiveGroupNotification", message);
        }
        
        public override Task OnConnectedAsync()
        {
            // Logic when a client connects
            return base.OnConnectedAsync();
        }
        public override Task OnDisconnectedAsync(Exception? exception)
        {
            // Logic when a client disconnects
            return base.OnDisconnectedAsync(exception);
        }
    }
}
