using FoodOrderingAPI.DTO;
using FoodOrderingAPI.Interfaces;
using FoodOrderingAPI.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Geometries;

namespace FoodOrderingAPI.Repository
{
    public class DeliveryManRepository : IDeliveryManRepository
    {
        private readonly ApplicationDBContext _context;

        private readonly INotificationRepo notificationRepo;
        public DeliveryManRepository(ApplicationDBContext context,INotificationRepo notificationRepo)
        {
            _context = context;
            notificationRepo = notificationRepo;
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

        public async Task<DeliveryMan> ApplyToJoinAsync(DeliveryMan deliveryManEntity)
        {
            if (deliveryManEntity == null)
                throw new ArgumentNullException(nameof(deliveryManEntity));
            if (deliveryManEntity.User == null)
                throw new ArgumentException("User info must be provided");
            if (string.IsNullOrEmpty(deliveryManEntity.User.Email))
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

        public async Task<bool> UpdateAvailabilityStatusAsync(string userId, bool AvailabilityStatus)
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
                                              .FirstOrDefaultAsync(dm => dm.UserId == userId);

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
            var orderLocation = new Point(orderLongitude, orderLatitude) { SRID = 4326 };

            var closestDeliveryMan = await _context.DeliveryMen
                .Include(dm => dm.User)
                .Where(dm => dm.AvailabilityStatus && dm.User != null && dm.User.Role == RoleEnum.DeliveryMan && dm.AccountStatus == AccountStatusEnum.Active  )
                .OrderBy(dm => dm.Location.Distance(orderLocation))
                .ThenBy(dm => dm.LastOrderDate ?? DateTime.MinValue)
                //.ToListAsync();
                .FirstOrDefaultAsync();
            return closestDeliveryMan;
        }

        private async Task UpdateDeliveryManAfterDeliveryAsync(string deliveryManId)
        {
            var deliveryMan = await DeliveryManEntityAsync(deliveryManId);
            if (deliveryMan != null)
            {
                deliveryMan.LastOrderDate = DateTime.UtcNow;
                deliveryMan.AvailabilityStatus = true;
            }
        }

        public async Task<DeliveryManUpdateOrderStatusDTO> UpdateOrderStatusAsync(Guid OrderId, StatusEnum newStatus, string deliveryManId)
        {

            var UpdateOrder = await _context.Orders.Include(d => d.Address).Include(o => o.Customer)
                .ThenInclude(C => C.User)
                .FirstOrDefaultAsync(or => or.OrderID == OrderId && or.DeliveryManID == deliveryManId);

            if (UpdateOrder == null)
                return null;

            //var validTransitions = new Dictionary<string, List<string>>(StringComparer.OrdinalIgnoreCase)
            //{
            //    {"Preparing"  , new List<string> { "PickedUp"} },
            //    {"PickedUp"   , new List<string> {"InRoute"} },
            //    {"InRoute"    , new List<string> {"Delivered" } }
            //};
            var validTransitions = new Dictionary<StatusEnum, List<StatusEnum>>
            {
                {StatusEnum.Preparing  , new List<StatusEnum> { StatusEnum.Out_for_Delivery} },
                {StatusEnum.Out_for_Delivery    , new List < StatusEnum > { StatusEnum.Delivered } }
            };
            var CurrentStatus = UpdateOrder.Status;

            // Check if the transition from current to new status is allowed
            if (!validTransitions.TryGetValue(CurrentStatus, out var allowedStatus) ||
                !allowedStatus.Contains(newStatus))
            {
                throw new InvalidOperationException(
                $"Invalid status transition from '{CurrentStatus}' to '{newStatus}'");
            }

            if (newStatus == StatusEnum.Delivered)
            {
                DeliveryMan deliveryMan = await DeliveryManEntityAsync(deliveryManId);

                if (deliveryMan != null)
                {
                    deliveryMan.LastOrderDate = DateTime.UtcNow;
                    await UpdateDeliveryManAfterDeliveryAsync(deliveryManId);
                }
                UpdateOrder.DeliveredAt = DateTime.Now;
                
            }

            UpdateOrder.Status = newStatus;
            await _context.SaveChangesAsync();

                var updated = new DeliveryManUpdateOrderStatusDTO()
                   {
                       OrderNumber = UpdateOrder.OrderNumber,
                        Address = UpdateOrder.Address.Street,
                        DeliveredAt = UpdateOrder.DeliveredAt,
                        TotalPrice = UpdateOrder.TotalPrice,
                        UserName = UpdateOrder.Customer.FirstName + UpdateOrder.Customer.LastName,
                        
                };
            return updated;
        }
    }
}
