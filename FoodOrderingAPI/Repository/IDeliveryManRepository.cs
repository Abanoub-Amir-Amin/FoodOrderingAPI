using FoodOrderingAPI.Models;

namespace FoodOrderingAPI.Repository
{
    public interface IDeliveryManRepository
    {
        Task<DeliveryMan> ApplyToJoinAsync (DeliveryMan deliveryManEntity);

        Task<bool> GetAvailabilityStatusAsync(string userId);

        Task<bool> UpdateAvailabilityStatusAsync(string userId , bool AvailabilityStatus);

        Task<DeliveryMan> GetDeliveryManByIdAsync(string userId);

        Task<DeliveryMan> UpdateDeliveryManAsync(DeliveryMan deliveryMan);

        Task<List<DeliveryMan>> GetAvailableDeliveryMenAsync();
        Task<DeliveryMan?> GetClosestDeliveryManAsync(double orderLatitude, double orderLongitude);
    }
}
