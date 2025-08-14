namespace FoodOrderingAPI.Services
{
    public interface INotificationService
    {
        public void CreateNotificationTo(string userId, string message);
        public void CreateNotificationToAll(string message);
        public void CreateNotificationToGroup(string groupName, string message);

    }
}
