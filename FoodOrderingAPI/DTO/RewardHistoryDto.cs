namespace FoodOrderingAPI.DTO
{
    public class RewardHistoryDto
    {
        public int RewardID { get; set; }
        public string CustomerID { get; set; }  
        public int PointsEarned { get; set; }
        public string Reason { get; set; }
        public DateTime DateEarned { get; set; }
    }

}
