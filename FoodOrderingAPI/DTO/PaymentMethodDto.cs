namespace FoodOrderingAPI.DTO
{
    public class PaymentMethodDto
    {
        public int PaymentMethodID { get; set; }
        public string CustomerID { get; set; }  
        public string MethodType { get; set; }
        public string Provider { get; set; }
        public string AccountNumber { get; set; }
        public DateTime? ExpiryDate { get; set; }
        public bool IsDefault { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
