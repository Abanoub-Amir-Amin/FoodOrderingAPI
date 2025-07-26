using AutoMapper;
using FoodOrderingAPI.DTO;
using FoodOrderingAPI.DTO.FoodOrderingAPI.DTO;
using FoodOrderingAPI.Models;
using FoodOrderingAPI.Repository;
using Microsoft.AspNetCore.Identity;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace FoodOrderingAPI.Services
{
    public class RestaurantService : IRestaurantService
    {
        private readonly ApplicationDBContext _context;       
        private readonly IRestaurantRepository _repository;    
        private readonly IMapper _mapper;                       
        private readonly UserManager<User> _userManager;       

        
        public RestaurantService(IRestaurantRepository repository, ApplicationDBContext context, IMapper mapper, UserManager<User> userManager)
        {
            _repository = repository;
            _context = context;
            _mapper = mapper;
            _userManager = userManager;
        }

        public async Task<Item> AddItemAsync(string restaurantId, ItemDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Category))
                throw new ArgumentException("Category is required.");

            var item = new Item
            {
                RestaurantID = restaurantId,
                Name = dto.Name,
                Description = dto.Description,
                Price = dto.Price,
                Category = dto.Category,
                IsAvailable = dto.IsAvailable,
                ImageUrl = dto.ImgUrl,
            };

            return await _repository.AddItemAsync(restaurantId, item);
        }

        //public async Task<Item> UpdateItemAsync(string restaurantId, ItemDto dto)
        //{
        //    var existingItem = await _repository.GetItemByIdAsync(dto.ItemID.Value, restaurantId);
        //    if (existingItem == null)
        //        return null;

        //    existingItem.Name = dto.Name;
        //    existingItem.Description = dto.Description;
        //    existingItem.Price = dto.Price;
        //    existingItem.Category = dto.Category;
        //    existingItem.IsAvailable = dto.IsAvailable;

        //    return await _repository.UpdateItemAsync(existingItem);
        //}

        public async Task<bool> DeleteItemAsync(Guid itemId, string restaurantId)
        {
            return await _repository.DeleteItemAsync(itemId, restaurantId);
        }

        public async Task<Item> GetItemByIdAsync(string restaurantId, Guid itemId)
        {
            return await _repository.GetItemByIdAsync(itemId, restaurantId);
        }

        public async Task<IEnumerable<Item>> GetItemsByCategoryAsync(string restaurantId, string category)
        {
            return await _repository.GetItemsByCategoryAsync(restaurantId,category);
        }

        public async Task<Discount> AddDiscountAsync(string restaurantId, Discount discount)
        {
            discount.RestaurantID = restaurantId;
            return await _repository.AddDiscountAsync(restaurantId, discount);
        }

        public async Task<Discount> UpdateDiscountAsync(Discount discount)
        {
            return await _repository.UpdateDiscountAsync(discount);
        }

        public async Task<bool> DeleteDiscountAsync(int discountId, string restaurantId)
        {
            return await _repository.DeleteDiscountAsync(discountId, restaurantId);
        }

        public async Task<PromoCode> AddPromoCodeAsync(string restaurantId, PromoCode promoCode)
        {
            promoCode.RestaurantID = restaurantId;
            return await _repository.AddPromoCodeAsync(restaurantId, promoCode);
        }

        public async Task<PromoCode> UpdatePromoCodeAsync(PromoCode promoCode)
        {
            return await _repository.UpdatePromoCodeAsync(promoCode);
        }

        public async Task<bool> DeletePromoCodeAsync(Guid promoCodeId, string restaurantId)
        {
            return await _repository.DeletePromoCodeAsync(promoCodeId, restaurantId);
        }

        public async Task<Order> UpdateOrderStatusAsync(Guid orderId, string status, string restaurantId)
        {
            var allowedStatuses = new[] { "Preparing", "Out for Delivery" };
            if (!allowedStatuses.Contains(status))
                throw new ArgumentException("Invalid order status.");

            return await _repository.UpdateOrderStatusAsync(orderId, status, restaurantId);
        }

        // Create User + Restaurant when applying to join
        public async Task<Restaurant> ApplyToJoinAsync(RestaurantDto dto)
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

            Console.WriteLine($"Applying to join with User: {dto.User.UserName}, Email: {dto.User.Email}");
            // 2. Check for existing users with same email or username
            var existingUserByEmail = await _userManager.FindByEmailAsync(dto.User.Email);
            if (existingUserByEmail != null)
                throw new InvalidOperationException("A user with this email already exists.");

            var existingUserByUsername = await _userManager.FindByNameAsync(dto.User.UserName);
            if (existingUserByUsername != null)
                throw new InvalidOperationException("A user with this username already exists.");

            // 3. Map User DTO to User entity
            var newUser = _mapper.Map<User>(dto.User);

            // 4. Set system properties (do NOT set Id manually unless really needed)
            newUser.Role = RoleEnum.Restaurant;
            newUser.CreatedAt = DateTime.UtcNow;

            // 5. Create user with hashed password via UserManager
            var userCreateResult = await _userManager.CreateAsync(newUser, dto.User.Password);
            if (!userCreateResult.Succeeded)
            {
                var errors = string.Join("; ", userCreateResult.Errors.Select(e => e.Description));
                throw new InvalidOperationException($"Failed to create user: {errors}");
            }

            // 6. Assign the "Restaurant" role to this user
            var roleAssignResult = await _userManager.AddToRoleAsync(newUser, "Restaurant");
            if (!roleAssignResult.Succeeded)
            {
                var errors = string.Join("; ", roleAssignResult.Errors.Select(e => e.Description));
                throw new InvalidOperationException($"Failed to assign role: {errors}");
            }

            // 7. Map Restaurant DTO to entity and link to created user
            var restaurantEntity = _mapper.Map<Restaurant>(dto);
            restaurantEntity.UserId = newUser.Id;
            restaurantEntity.User = newUser;

            // 8. Initialize restaurant inactive pending approval
            restaurantEntity.IsActive = false;

            restaurantEntity.User.Restaurant = null;

            return await _repository.ApplyToJoinAsync(restaurantEntity);
        }

        public async Task<Restaurant> GetRestaurantByIdAsync(string userId)
        {
            if (userId == string.Empty)
                throw new ArgumentException("UserId is invalid", nameof(userId));

            return await _repository.GetRestaurantByIdAsync(userId);
        }

        public async Task<Restaurant> UpdateRestaurantProfileAsync(string restaurantId, RestaurantProfileDto dto)
        {
            var existingRestaurant = await _repository.GetRestaurantByIdAsync(restaurantId);

            if (existingRestaurant == null)
            {
                return null;
            }

            if (!string.IsNullOrWhiteSpace(dto.RestaurantName))
            {
                existingRestaurant.RestaurantName = dto.RestaurantName;
            }
            if (!string.IsNullOrWhiteSpace(dto.Location))
            {
                existingRestaurant.Location = dto.Location;
            }
            if (!string.IsNullOrWhiteSpace(dto.OpenHours))
            {
                existingRestaurant.OpenHours = dto.OpenHours;
            }
            if (!string.IsNullOrWhiteSpace(dto.LogoUrl))
            {
                existingRestaurant.LogoUrl = dto.LogoUrl;
            }
            if (dto.IsAvailable.HasValue)
            {
                existingRestaurant.IsAvailable = dto.IsAvailable.Value;
            }
            if (!string.IsNullOrWhiteSpace(dto.Email))
            {
                existingRestaurant.User.Email = dto.Email;
            }
            if (!string.IsNullOrWhiteSpace(dto.Phone))
            {
                existingRestaurant.User.PhoneNumber = dto.Phone;
            }
            return await _repository.UpdateRestaurantAsync(existingRestaurant);
        }
        public async Task<IEnumerable<Order>> GetAllOrdersByRestaurantAsync(string restaurantId)
        {
            return await _repository.GetAllOrdersByRestaurantAsync(restaurantId);
        }
        public async Task<IEnumerable<OrderDto>> GetOrdersByStatusAsync(string restaurantId, string[] statuses)
        {
            var orders = await _repository.GetAllOrdersByRestaurantAsync(restaurantId);

            // Filter by status using case-insensitive comparison
            var filteredOrders = orders.Where(o => statuses.Contains(o.Status, StringComparer.OrdinalIgnoreCase));

            return _mapper.Map<IEnumerable<OrderDto>>(filteredOrders);
        }

        public async Task<DashboardSummaryDto> GetDashboardSummaryAsync(string restaurantId)
        {
            return await _repository.GetDashboardSummaryAsync(restaurantId);
        }

        public async Task<IEnumerable<ItemDto>> GetMostOrderedItemsAsync(string restaurantId, int topCount = 10)
        {
            // Call repository to get most ordered items with quantities
            var mostOrderedItems = await _repository.GetMostOrderedItemsAsync(restaurantId, topCount);

            // Map each Item entity to ItemDto
            var itemDtos = mostOrderedItems.Select(x =>
            {
                var dto = _mapper.Map<ItemDto>(x.Item);
                return dto;
            });

            return itemDtos;
        }


    }
}
