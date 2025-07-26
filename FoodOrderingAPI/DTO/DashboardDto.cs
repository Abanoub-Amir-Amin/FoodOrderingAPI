using System.Collections.Generic;

namespace FoodOrderingAPI.DTO
{
    public class DashboardDto
    {
        public List<RestaurantDto> ActiveRestaurants { get; set; }
        public List<RestaurantDto> InactiveRestaurants { get; set; }
        public List<DeliveryManDto> DeliveryMen { get; set; }
        public List<CustomerDTO> Customers { get; set; }
    }
}
