namespace FoodOrderingAPI.DTO
{
    public class PaymentTransactionDto
    {
        public int PaymentTransactionID { get; set; }
        public int OrderID { get; set; }
        public int? PaymentMethodID { get; set; }
        public DateTime TransactionDate { get; set; }
        public decimal Amount { get; set; }
        public string Status { get; set; }
        public string TransactionReference { get; set; }
    }
}
