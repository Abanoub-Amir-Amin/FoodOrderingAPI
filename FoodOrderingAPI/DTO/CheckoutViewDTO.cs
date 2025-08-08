using FoodOrderingAPI.Models;

namespace FoodOrderingAPI.DTO
{
    public class CheckoutViewDTO
    {
        public string RestaurantName { get; set; }
        public List<ShoppingCartItemDto> Items { get; set; }
        public string PhoneNumber { get; set; }
        public Address Address { get; set; }

        public decimal SubTotal { get; set; }

        public decimal DelivaryPrice { get; set; }

        public decimal DiscountAmount { get; set; }
        public decimal TotalPrice => SubTotal + DelivaryPrice - DiscountAmount;
        //public string PaymentMethod { get; set; } // أو قائمة بالخيارات المتاحة
    }

}
