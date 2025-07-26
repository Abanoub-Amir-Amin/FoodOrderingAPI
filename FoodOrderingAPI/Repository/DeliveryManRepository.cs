using FoodOrderingAPI.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace FoodOrderingAPI.Repository
{
    public class DeliveryManRepository : IDeliveryManRepository
    {
        private readonly ApplicationDBContext _context;
        

        public DeliveryManRepository(ApplicationDBContext context)
        {
            _context = context;
        }


        private async Task<DeliveryMan> DeliveryManEntityAsync(string userId)
        {
            if (userId == null)
                throw new ArgumentException("Invalid user ID format. Expected a valid string.", nameof(userId));

            var deliveryMan = await _context.DeliveryMen
                .FirstOrDefaultAsync(dm => dm.UserId == userId);

            if (deliveryMan == null)
                throw new InvalidOperationException("Delivery man not found.");

            return deliveryMan;
        }

        public async Task<DeliveryMan> ApplyToJoinAsync (DeliveryMan deliveryManEntity)
        {
            if(deliveryManEntity == null) 
                throw new ArgumentNullException(nameof(deliveryManEntity));
            if(deliveryManEntity.User == null)
                throw new ArgumentException("User info must be provided");
            if(string.IsNullOrEmpty(deliveryManEntity.User.Email))
                throw new ArgumentException("User Email must be provided before Save.");

            deliveryManEntity.AvailabilityStatus = true;
            _context.DeliveryMen.Add(deliveryManEntity);
            await _context.SaveChangesAsync();
            return deliveryManEntity;
        }

        public async Task<bool> GetAvailabilityStatusAsync(string userId)
        {
            var deliveryMan = await DeliveryManEntityAsync(userId);

            return deliveryMan?.AvailabilityStatus ?? false; 
        }

        public async Task<bool> UpdateAvailabilityStatusAsync(string userId , bool AvailabilityStatus)
        {

            var deliveryMan = await DeliveryManEntityAsync(userId);

            if (deliveryMan == null)
                return false;

            deliveryMan.AvailabilityStatus = AvailabilityStatus;
            await _context.SaveChangesAsync();
            return true;
        }

        //Profile
        public async Task<DeliveryMan> GetDeliveryManByIdAsync(string userId)
        {
            if (userId == null)
                throw new ArgumentException("Invalid user ID format. Expected a valid string.", nameof(userId));

            var deliveryMan = await _context.DeliveryMen.Include(dm => dm.User)
                                              .FirstOrDefaultAsync(dm => dm.DeliveryManID == userId);

            if (deliveryMan == null)
                throw new InvalidOperationException("Delivery man not found.");

            return deliveryMan;
        }

        public async Task<DeliveryMan> UpdateDeliveryManAsync(DeliveryMan deliveryMan)
        {
            _context.DeliveryMen.Update(deliveryMan);
            await _context.SaveChangesAsync();
            return deliveryMan;
        }

        public async Task<List<DeliveryMan>> GetAvailableDeliveryMenAsync()
        {
           return await _context.DeliveryMen
                .Include(dm => dm.User)
                .Where(dm => dm.AvailabilityStatus
                && dm.User != null 
                && dm.User.Role == RoleEnum.DeliveryMan)
                .ToListAsync();
        }

        public async Task<DeliveryMan?> GetClosestDeliveryManAsync(double orderLatitude, double orderLongitude)
        {
            return await _context.DeliveryMen
                .Where(dm => dm.AvailabilityStatus && dm.User != null && dm.User.Role == RoleEnum.DeliveryMan)
                .OrderBy(dm => CalculateDistance(dm.Latitude, dm.Longitude, orderLatitude, orderLongitude))
                .FirstOrDefaultAsync();
        }

        private double CalculateDistance(double lat1, double lon1, double lat2, double lon2)
        {
            // Haversine formula to calculate the distance between two points on the Earth
            var R = 6371; // Radius of the Earth in kilometers
            var dLat = (lat2 - lat1) * (Math.PI / 180);
            var dLon = (lon2 - lon1) * (Math.PI / 180);
            var a =
                Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                Math.Cos(lat1 * (Math.PI / 180)) * Math.Cos(lat2 * (Math.PI / 180)) *
                Math.Sin(dLon / 2) * Math.Sin(dLon / 2);
            var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
            return R * c; // Distance in kilometers
        }
    }
}
