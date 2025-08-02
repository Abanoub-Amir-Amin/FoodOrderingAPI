using AutoMapper;
using FoodOrderingAPI.DTO;
using FoodOrderingAPI.DTO.FoodOrderingAPI.DTO;
using Microsoft.AspNetCore.Http;
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
        private readonly IWebHostEnvironment _environment;
        private readonly ApplicationDBContext _context;
        private readonly IRestaurantRepository _repository;
        private readonly IMapper _mapper;
        private readonly UserManager<User> _userManager;


        public RestaurantService(IRestaurantRepository repository, ApplicationDBContext context, IMapper mapper, UserManager<User> userManager, IWebHostEnvironment environment)
        {
            _repository = repository;
            _context = context;
            _mapper = mapper;
            _userManager = userManager;
            _environment = environment ?? throw new ArgumentNullException(nameof(environment));
        }

        //Item-CRUD
        public async Task<Item> AddItemAsync(string restaurantId, ItemDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Category))
                throw new ArgumentException("Category is required.");

            // Map ItemDto → Item
            var item = _mapper.Map<Item>(dto);

            // Assign new Guid if RestaurantID is empty
            if (item.ItemID == Guid.Empty)
            {
                item.ItemID = Guid.NewGuid();
            }

            item.RestaurantID = restaurantId;

            if (dto.ImageFile != null && dto.ImageFile.Length > 0)
            {
                item.ImageFile = await SaveImageAsync(dto.ImageFile);
            }

            return await _repository.AddItemAsync(restaurantId, item);
        }

        public async Task<Item> UpdateItemAsync(string restaurantId, Guid itemId, ItemDto dto)
        {
            var existingItem = await _repository.GetItemByIdAsync(itemId, restaurantId);
            if (existingItem == null)
                return null;

            // Map update DTO onto existing entity 
            _mapper.Map(dto, existingItem);

            existingItem.ItemID = itemId;
            existingItem.RestaurantID = restaurantId;

            if (dto.ImageFile != null && dto.ImageFile.Length > 0)
            {
                existingItem.ImageFile = await SaveImageAsync(dto.ImageFile);
            }

            return await _repository.UpdateItemAsync(existingItem);
        }

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
            return await _repository.GetItemsByCategoryAsync(restaurantId, category);
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


        //Discount-CRUD
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


        //PromoCode-CRUD
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

        public async Task<IEnumerable<PromoCode>> GetAllPromoCodesByRestaurantAsync(string restaurantId)
        {
            if (string.IsNullOrWhiteSpace(restaurantId))
                throw new ArgumentException("Restaurant ID must be provided.", nameof(restaurantId));

            if (!Guid.TryParse(restaurantId, out Guid rid))
                throw new ArgumentException("Invalid Restaurant ID format.", nameof(restaurantId));

            return await _repository.GetAllPromoCodesByRestaurantAsync(restaurantId);
        }

        public async Task<IEnumerable<PromoCode>> SearchPromoCodesByCodeAsync(string restaurantId, string code)
        {
            if (string.IsNullOrWhiteSpace(restaurantId))
                throw new ArgumentException("Restaurant ID must be provided.", nameof(restaurantId));

            if (string.IsNullOrWhiteSpace(code))
                return await GetAllPromoCodesByRestaurantAsync(restaurantId); // If no filter, return all

            if (!Guid.TryParse(restaurantId, out Guid rid))
                throw new ArgumentException("Invalid Restaurant ID format.", nameof(restaurantId));

            return await _repository.SearchPromoCodesByCodeAsync(restaurantId, code);
        }


        // Create User + Restaurant when applying to join
        public async Task<Restaurant> ApplyToJoinAsync(RestaurantDto dto, IFormFile logoFile = null)
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
            restaurantEntity.RestaurantID = newUser.Id;
            restaurantEntity.User = newUser;

            // Assign new Guid if RestaurantID is empty
            if (string.IsNullOrWhiteSpace(restaurantEntity.RestaurantID))
            {
                restaurantEntity.RestaurantID = Guid.NewGuid().ToString();
            }

            // 8. Save logo file if provided, assign relative URL to LogoUrl property
            if (logoFile != null && logoFile.Length > 0)
            {
                restaurantEntity.LogoUrl = await SaveImageAsync(logoFile);
            }

            // 8. Initialize restaurant inactive pending approval
            restaurantEntity.IsActive = false;

            restaurantEntity.User.Restaurant = null;

            return await _repository.ApplyToJoinAsync(restaurantEntity);
        }


        //updating restaurant itself
        public async Task<Restaurant> GetRestaurantByIdAsync(string userId)
        {
            if (userId == string.Empty)
                throw new ArgumentException("UserId is invalid", nameof(userId));

            return await _repository.GetRestaurantByIdAsync(userId);
        }

        public async Task<IEnumerable<Restaurant>> GetAllRestaurantsAsync()
        {
            return await _repository.GetAllRestaurantsAsync();
        }

        public async Task<Restaurant> UpdateRestaurantProfileAsync(string restaurantId, RestaurantUpdateDto dto)
        {
            var existingRestaurant = await _repository.GetRestaurantByIdAsync(restaurantId);

            if (existingRestaurant == null)
                return null;

            if (!string.IsNullOrWhiteSpace(dto.RestaurantName))
                existingRestaurant.RestaurantName = dto.RestaurantName;

            if (!string.IsNullOrWhiteSpace(dto.Location))
                existingRestaurant.Location = dto.Location;

            if (!string.IsNullOrWhiteSpace(dto.OpenHours))
                existingRestaurant.OpenHours = dto.OpenHours;

            if (dto.LogoFile != null && dto.LogoFile.Length > 0)
                existingRestaurant.LogoUrl = await SaveImageAsync(dto.LogoFile);

            if (dto.IsAvailable.HasValue)
                existingRestaurant.IsAvailable = dto.IsAvailable.Value;

            if (!string.IsNullOrWhiteSpace(dto.Email))
                existingRestaurant.User.Email = dto.Email;

            if (!string.IsNullOrWhiteSpace(dto.Phone))
                existingRestaurant.User.PhoneNumber = dto.Phone;

            return await _repository.UpdateRestaurantAsync(existingRestaurant);
        }


        // Orders
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
        public async Task<Order> UpdateOrderStatusAsync(Guid orderId, string status, string restaurantId)
        {
            var allowedStatuses = new[] { "Preparing", "Out for Delivery", "Canceled" };
            if (!allowedStatuses.Contains(status))
                throw new ArgumentException("Invalid order status.");

            return await _repository.UpdateOrderStatusAsync(orderId, status, restaurantId);
        }


        //Dashboard Summary
        public async Task<DashboardSummaryDto> GetDashboardSummaryAsync(string restaurantId)
        {
            return await _repository.GetDashboardSummaryAsync(restaurantId);
        }


        //Image Upload
        public async Task<string> SaveImageAsync(IFormFile file)
        {
            if (file == null || file.Length == 0)
                throw new ArgumentException("No file provided");

            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif", ".webp" };
            var ext = Path.GetExtension(file.FileName).ToLowerInvariant();
            if (string.IsNullOrEmpty(ext) || !allowedExtensions.Contains(ext))
                throw new InvalidOperationException("Unsupported file format.");

            var uniqueFileName = Guid.NewGuid().ToString() + ext;
            var uploadsFolder = Path.Combine(_environment.WebRootPath, "uploads");

            if (!Directory.Exists(uploadsFolder))
                Directory.CreateDirectory(uploadsFolder);

            var filePath = Path.Combine(uploadsFolder, uniqueFileName);

            try
            {
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Failed to save uploaded file: {ex.Message}");
            }

            // Return relative URL 
            return $"/uploads/{uniqueFileName}";
        }



    }
}