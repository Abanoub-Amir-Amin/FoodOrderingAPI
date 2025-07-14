namespace FoodOrderingAPI.Services
{
    public interface INotificationService
    {
        public void CreateNotificationTo(Guid userId, string message);
        public void CreateNotificationToAll(string message);
        public void CreateNotificationToGroup(string groupName, string message);

    }
}
