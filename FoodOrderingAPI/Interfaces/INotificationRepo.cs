namespace FoodOrderingAPI.Interfaces
{
    public interface INotificationRepo
    {
        public void CreateNotificationTo(string userId, string message);
        public void CreateNotificationToAll(string message);

        // Uncomment if group notifications are needed
        //public void CreateNotificationToGroup(string groupName, string message);
        //public void AddToGroup(Guid userId, string groupName);
    }
}
