using AutoMapper;
using FoodOrderingAPI.DTO;
using FoodOrderingAPI.Models;
using FoodOrderingAPI.Repository;
using Microsoft.AspNetCore.Identity;

namespace FoodOrderingAPI.Services
{
    public class DeliveryManService : IDeliveryManService
    {
        private readonly IDeliveryManRepository _repository;
        private readonly ApplicationDBContext _context;
        private readonly IMapper _mapper;
        private readonly UserManager<User> _userManager;

        public DeliveryManService( IDeliveryManRepository repository , ApplicationDBContext context , IMapper mapper , UserManager<User> userManager)
        {
            _context = context;
            _repository = repository;
            _mapper = mapper;
            _userManager = userManager;
        }
        public async Task<DeliveryMan> ApplyToJoinAsync(DeliveryManDto dto)
        {
            // 1. Validate DTO and nested user info
            if (dto == null)
                throw new ArgumentNullException(nameof(dto));
            if (dto.User == null)
                throw new ArgumentException("User info must be provided", nameof(dto.User));
            if (string.IsNullOrWhiteSpace(dto.User.Email))
                throw new ArgumentException("Email is required", nameof(dto.User.Email));
            if (string.IsNullOrWhiteSpace(dto.User.Password))
                throw new ArgumentException("Password is required", nameof(dto.User.Password));
            if (string.IsNullOrWhiteSpace(dto.User.UserName))
                throw new ArgumentException("Username is required", nameof(dto.User.UserName));

            // 2. Check for existing users with same email or username
            var existingUserByEmail = await _userManager.FindByEmailAsync(dto.User.Email);
            if (existingUserByEmail != null)
                throw new InvalidOperationException("A user with this email already exists.");

            var existingUserByUsername = await _userManager.FindByNameAsync(dto.User.UserName);
            if (existingUserByUsername != null)
                throw new InvalidOperationException("A user with this username already exists.");

            // 3. Map User DTO to User entity
            var newUser = _mapper.Map<User>(dto.User);

            // 4. set Role and Date manually
            newUser.Role = RoleEnum.DeliveryMan;
            newUser.CreatedAt = DateTime.UtcNow;
            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                // 5. Create user with hashed password via UserManager
                var userCreateResult = await _userManager.CreateAsync(newUser, dto.User.Password);
                if (!userCreateResult.Succeeded)
                {
                    var errors = string.Join("; ", userCreateResult.Errors.Select(e => e.Description));
                    throw new InvalidOperationException($"Failed to create user: {errors}");
                }

                // 6. Assign the "DeliveryMan" role to this user
                var roleAssignResult = await _userManager.AddToRoleAsync(newUser, "DeliveryMan");
                if (!roleAssignResult.Succeeded)
                {
                    var errors = string.Join("; ", roleAssignResult.Errors.Select(e => e.Description));
                    throw new InvalidOperationException($"Failed to assign role: {errors}");
                }

                // 7. Map DeliveryMan DTO to entity and link to created user
                var DeliveryManEntity = _mapper.Map<DeliveryMan>(dto);
                DeliveryManEntity.UserId = newUser.Id;
                DeliveryManEntity.User = newUser;

                // 8. Initialize DeliveryMan AccountStatus pending approval and AvailabilityStatus
                DeliveryManEntity.AccountStatus = AccountStatusEnum.Pending;
                DeliveryManEntity.AvailabilityStatus = true;

                DeliveryManEntity.User.DeliveryMan = null;

                // 9. Save DeliveryMan before commit
                var result = await _repository.ApplyToJoinAsync(DeliveryManEntity);

               // 10. Commit only after saving all entities
                await transaction.CommitAsync();

                return result;


            }

        }

        public async Task<bool> GetAvailabilityStatusAsync(string userId)
        {
            if(string.IsNullOrEmpty(userId))
                throw new ArgumentNullException("Can not find user Id",nameof(userId));

            return await _repository.GetAvailabilityStatusAsync(userId);
        }

        public async Task<bool> UpdateAvailabilityStatusAsync(string userId, bool AvailabilityStatus)
        {
            if (string.IsNullOrEmpty(userId))
                throw new ArgumentNullException("Can not find user Id", nameof(userId));

            return await _repository.UpdateAvailabilityStatusAsync(userId, AvailabilityStatus);
        }

        public async Task<DeliveryManDto> GetProfileAsync(string userId)
        {
            var deliveryMan = await _repository.GetDeliveryManByIdAsync(userId);
            return _mapper.Map<DeliveryManDto>(deliveryMan);
        }

        public async Task<DeliveryManDto> UpdateProfileAsync(string userId, DeliveryManDto dto)
        {
            var deliveryMan = await _repository.GetDeliveryManByIdAsync(userId);
            if (deliveryMan == null)
            {
                throw new KeyNotFoundException("DeliveryMan not found");
            }

            // Update properties
            deliveryMan.Latitude = dto.Latitude;
            deliveryMan.Longitude = dto.Longitude;
            deliveryMan.Rank = dto.Rank;
            deliveryMan.AvailabilityStatus = dto.AvailabilityStatus;

            var updatedDeliveryMan = await _repository.UpdateDeliveryManAsync(deliveryMan);

            return _mapper.Map<DeliveryManDto>(updatedDeliveryMan);
        }

        public async Task<DeliveryMan?> GetBestAvailableDeliveryManAsync()
        {
            var availableDeliveryMen = await _repository.GetAvailableDeliveryMenAsync();

            if (availableDeliveryMen == null || !availableDeliveryMen.Any())
                return null;
            return availableDeliveryMen
                .OrderBy(dm => dm.LastOrderDate ?? DateTime.MinValue)
                .ThenByDescending(dm => dm.Rank)
                .FirstOrDefault();
        }

        public async Task<DeliveryMan?> GetClosestDeliveryManAsync(double orderLatitude, double orderLongitude)
        {
            return await _repository.GetClosestDeliveryManAsync(orderLatitude, orderLongitude);
        }
    }
}
