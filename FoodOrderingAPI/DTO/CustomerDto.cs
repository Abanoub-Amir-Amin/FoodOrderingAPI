using FoodOrderingAPI.Models;

namespace FoodOrderingAPI.DTO
{
    public class CustomerDTO
    {
        public string FirstName { set; get; }
        public string LastName { set; get; }
        public string UserName { set; get; }
        public string Email { set; get; }
        public string PhoneNumber { set; get; }
        public GenderEnum? Gender { set; get; }
        public List<string> Addresses { set; get; }
        //public int LoyaltyPoints { get; set; } = 0;
        public int TotalOrders { get; set; } = 0;
        public int TotalDeliveredOrders { get; set; } = 0;
        public int TotalCancelledOrders { get; set; } = 0;

        // List of in process orders:
        public List<OrderDto> InProcessOrders { get; set; }
        //public List<string> Rewards { get; set; }
        //public int TotalRewardspoints { get; set; }

    }
}
