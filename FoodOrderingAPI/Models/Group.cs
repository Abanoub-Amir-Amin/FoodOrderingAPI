namespace FoodOrderingAPI.Models
{
    public class Group
    {
        public Guid GroupId { get; set; }
        public string GroupName { get; set; }
        public IEnumerable<User> Users { get; set; } = new HashSet<User>();
    }
}
