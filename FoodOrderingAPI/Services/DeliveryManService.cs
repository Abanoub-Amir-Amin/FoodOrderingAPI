using AutoMapper;
using FoodOrderingAPI.DTO;
using FoodOrderingAPI.Models;
using FoodOrderingAPI.Repository;
using Microsoft.AspNetCore.Identity;
using NetTopologySuite.Geometries;


namespace FoodOrderingAPI.Services
{
    public class DeliveryManService : IDeliveryManService
    {
        private readonly IDeliveryManRepository _repository;
        private readonly ApplicationDBContext _context;
        private readonly IMapper _mapper;
        private readonly UserManager<User> _userManager;

        public DeliveryManService(IDeliveryManRepository repository, ApplicationDBContext context, IMapper mapper, UserManager<User> userManager)
        {
            _context = context;
            _repository = repository;
            _mapper = mapper;
            _userManager = userManager;
        }

        public async Task<DeliveryMan> GetDeliveryManByIdAsync(string userId)
        {
            if (string.IsNullOrEmpty(userId))
                throw new ArgumentNullException("Can not find user Id", nameof(userId));
            return await _repository.GetDeliveryManByIdAsync(userId);
        }
        public async Task<DeliveryMan> ApplyToJoinAsync(DeliveryManApplyDto dto)
        {
            // 1. Validate DTO and nested user info
            if (dto == null)
                throw new ArgumentNullException(nameof(dto));

            if (string.IsNullOrWhiteSpace(dto.Email))
                throw new ArgumentException("Email is required", nameof(dto.Email));

            if (string.IsNullOrWhiteSpace(dto.Password))
                throw new ArgumentException("Password is required", nameof(dto.Password));

            if (string.IsNullOrWhiteSpace(dto.UserName))
                throw new ArgumentException("Username is required", nameof(dto.UserName));

            // 2. Check for existing users with same email or username
            var existingUserByEmail = await _userManager.FindByEmailAsync(dto.Email);
            if (existingUserByEmail != null)
                throw new InvalidOperationException("A user with this email already exists.");

            var existingUserByUsername = await _userManager.FindByNameAsync(dto.UserName);
            if (existingUserByUsername != null)
                throw new InvalidOperationException("A user with this username already exists.");

            // 3. Map User DTO to User entity
            var newUser = _mapper.Map<User>(dto);

            // 4. set Role and Date manually
            newUser.Role = RoleEnum.DeliveryMan;
            newUser.CreatedAt = DateTime.UtcNow;
            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    var userCreateResult = await _userManager.CreateAsync(newUser, dto.Password);
                    if (!userCreateResult.Succeeded)
                    {
                        var errors = string.Join("; ", userCreateResult.Errors.Select(e => e.Description));
                        throw new InvalidOperationException($"Failed to create user: {errors}");
                    }

                    var roleAssignResult = await _userManager.AddToRoleAsync(newUser, "DeliveryMan");
                    if (!roleAssignResult.Succeeded)
                    {
                        var errors = string.Join("; ", roleAssignResult.Errors.Select(e => e.Description));
                        throw new InvalidOperationException($"Failed to assign role: {errors}");
                    }

                    var DeliveryManEntity = _mapper.Map<DeliveryMan>(dto);
                    DeliveryManEntity.UserId = newUser.Id;
                    DeliveryManEntity.DeliveryManID = newUser.Id;
                    DeliveryManEntity.User = newUser;

                    DeliveryManEntity.AccountStatus = AccountStatusEnum.Pending;
                    DeliveryManEntity.AvailabilityStatus = true;
                    DeliveryManEntity.Location = new Point(DeliveryManEntity.Longitude, DeliveryManEntity.Latitude) { SRID = 4326 };

                    DeliveryManEntity.User.DeliveryMan = DeliveryManEntity;

                    var result = await _repository.ApplyToJoinAsync(DeliveryManEntity);

                    await transaction.CommitAsync();

                    return result;
                }
                catch
                {
                    // لو حصل أي مشكلة نعمل rollback
                    await transaction.RollbackAsync();
                    throw;
                }
            }


        }

        public async Task<bool> GetAvailabilityStatusAsync(string userId)
        {
            if (string.IsNullOrEmpty(userId))
                throw new ArgumentNullException("Can not find user Id", nameof(userId));

            return await _repository.GetAvailabilityStatusAsync(userId);
        }

        public async Task<bool> UpdateAvailabilityStatusAsync(string userId, bool AvailabilityStatus)
        {
            if (string.IsNullOrEmpty(userId))
                throw new ArgumentNullException("Can not find user Id", nameof(userId));

            return await _repository.UpdateAvailabilityStatusAsync(userId, AvailabilityStatus);
        }

        public async Task<DeliveryManProfileDto> GetProfileAsync(string userId)
        {
            var deliveryMan = await _repository.GetDeliveryManByIdAsync(userId);
            return _mapper.Map<DeliveryManProfileDto>(deliveryMan);
        }

        public async Task<DeliveryMan> UpdateProfileAsync(string userId, DeliveryManProfileUpdateDTO dto)
        {
            var existingDeliveryMan = await _repository.GetDeliveryManByIdAsync(userId);

            if (existingDeliveryMan == null)
            {
                return null;
            }

            if (!string.IsNullOrWhiteSpace(dto.UserName))
            {
                existingDeliveryMan.User.UserName = dto.UserName;
            }
            if (!string.IsNullOrWhiteSpace(dto.PhoneNumber))
            {
                existingDeliveryMan.User.PhoneNumber = dto.PhoneNumber;
            }
            if (!string.IsNullOrWhiteSpace(dto.Email))
            {
                existingDeliveryMan.User.Email = dto.Email;
            }
            if (dto.Latitude != 0)
            {
                existingDeliveryMan.Latitude = dto.Latitude;
            }
            if (dto.Longitude != 0)
            {
                existingDeliveryMan.Longitude = dto.Longitude;
            }
            if (dto.Longitude != 0 && dto.Latitude != 0)
            {
                // Update the Location based on Longitude and Latitude
                existingDeliveryMan.Location = new Point(existingDeliveryMan.Longitude, existingDeliveryMan.Latitude) { SRID = 4326 };
            }

            var updatedDeliveryMan = await _repository.UpdateDeliveryManAsync(existingDeliveryMan);

            return updatedDeliveryMan;
        }

        public async Task<DeliveryMan?> GetBestAvailableDeliveryManAsync()
        {
            var availableDeliveryMen = await _repository.GetAvailableDeliveryMenAsync();

            if (availableDeliveryMen == null || !availableDeliveryMen.Any())
                return null;
            return availableDeliveryMen
                .OrderBy(dm => dm.LastOrderDate ?? DateTime.MinValue)
                .FirstOrDefault();

            //.ToList();
            //.ThenByDescending(dm => dm.Rank)
        }

        public async Task<DeliveryMan?> GetClosestDeliveryManAsync(double orderLatitude, double orderLongitude)
        {
            return await _repository.GetClosestDeliveryManAsync(orderLatitude, orderLongitude);
        }

        public async Task<DeliveryManUpdateOrderStatusDTO> UpdateOrderStatusAsync(Guid OrderId, StatusEnum newStatus, string deliveryManId)
        {
            return await _repository.UpdateOrderStatusAsync(OrderId, newStatus, deliveryManId);

        }
    }
}
