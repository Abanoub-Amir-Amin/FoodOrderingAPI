using FoodOrderingAPI.DTO;
using FoodOrderingAPI.Models;

namespace FoodOrderingAPI.Services
{
    public interface IDeliveryManService
    {
        Task<DeliveryMan> GetDeliveryManByIdAsync(string deliveryManId);
        Task<DeliveryMan> ApplyToJoinAsync(DeliveryManApplyDto dto);

        Task<bool> GetAvailabilityStatusAsync(string userId);

        Task<bool> UpdateAvailabilityStatusAsync(string userId, bool AvailabilityStatus);
        Task<DeliveryManProfileDto> GetProfileAsync(string userId);

        Task<DeliveryMan> UpdateProfileAsync(string userId, DeliveryManProfileUpdateDTO dto);

        Task<DeliveryMan?> GetBestAvailableDeliveryManAsync();

        Task<DeliveryMan?> GetClosestDeliveryManAsync(double orderLatitude, double orderLongitude);

        Task<DeliveryManUpdateOrderStatusDTO> UpdateOrderStatusAsync(Guid OrderId, StatusEnum newStatus, string deliveryManId);
    }
}
