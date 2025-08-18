using FoodOrderingAPI.Hubs;
using FoodOrderingAPI.Models;
using FoodOrderingAPI.Interfaces;
using Microsoft.AspNetCore.SignalR;

namespace FoodOrderingAPI.Repository
{
    public class NotificationRepo : INotificationRepo
    {
        public ApplicationDBContext Context { get; }
        public IHubContext<NotificationHub> HubContext { get; }

        public NotificationRepo(ApplicationDBContext context, IHubContext<NotificationHub> hubContext)
        {
            Context = context;
            HubContext = hubContext;
        }

        public void CreateNotificationTo(string userId, string message)
        {
            var notification = new Notification
            {
                UserId = userId,
                Message = message,
                Type = "UserNotification",
                IsRead = false,
                CreatedAt = DateTime.UtcNow
            };
            Context.Notifications.Add(notification);
            Context.SaveChanges();
            HubContext.Clients.User(userId.ToString()).SendAsync("ReceiveNotification", message, notification.NotificationId);
        }

        public List<Notification> GetNotificationsByUserId(string userId)
        {
            var notifications = Context.Notifications
                .Where(n => n.UserId == userId)
                .OrderByDescending(n => n.CreatedAt)
                .ToList();
            return notifications;
        }

        public void MarkNotificationAsRead(Guid notificationId)
        {
            var notification = Context.Notifications.Find(notificationId);
            if (notification != null)
            {
                notification.IsRead = true;
                Context.SaveChanges();
            }
        }

        public void CreateNotificationToAll(string message)
        {
            var notification = new Notification
            {
                UserId = null,
                Message = message,
                Type = "PublicNotification",
                IsRead = null,
                CreatedAt = DateTime.UtcNow
            };
            Context.Notifications.Add(notification);
            Context.SaveChanges();
            HubContext.Clients.All.SendAsync("ReceivePublicNotification", message);
        }

        //public void CreateNotificationToGroup(string groupName, string message)
        //{
        //    var notification = new Notification
        //    {
        //        UserId = null,
        //        Message = message,
        //        Type = "GroupNotification",
        //        IsRead = null,
        //        CreatedAt = DateTime.UtcNow
        //    };
        //    Context.Notifications.Add(notification);
        //    Context.SaveChanges();
        //    HubContext.Clients.Groups(groupName).SendAsync("ReceiveGroupNotification", message);
        //}

        //public void AddToGroup(Guid userId, string groupName)
        //{
        //    throw new NotImplementedException();
        //}
    }
}
