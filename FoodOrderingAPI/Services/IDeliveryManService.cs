using FoodOrderingAPI.DTO;
using FoodOrderingAPI.Models;

namespace FoodOrderingAPI.Services
{
    public interface IDeliveryManService
    {
        Task<DeliveryMan> ApplyToJoinAsync (DeliveryManDto  dto);

        Task<bool> GetAvailabilityStatusAsync(string userId);

        Task<bool> UpdateAvailabilityStatusAsync(string userId, bool AvailabilityStatus);
        Task<DeliveryManDto> GetProfileAsync(string userId);

        Task<DeliveryManDto> UpdateProfileAsync(string userId, DeliveryManDto dto);

        Task<DeliveryMan?> GetBestAvailableDeliveryManAsync();
        Task<DeliveryMan?> GetClosestDeliveryManAsync(double orderLatitude, double orderLongitude);
    }
}
