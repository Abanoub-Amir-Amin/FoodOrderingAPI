using AutoMapper;
using FoodOrderingAPI.DTO;
using FoodOrderingAPI.DTO.FoodOrderingAPI.DTO;
using FoodOrderingAPI.Models;
using FoodOrderingAPI.Repository;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using NetTopologySuite.Index.HPRtree;
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

        // Create User + Restaurant when applying to join
        public async Task<Restaurant> ApplyToJoinAsync(RestaurantUpdateDto dto)
        {
            if (dto == null) throw new ArgumentNullException(nameof(dto));
            if (dto.User == null) throw new ArgumentException("User info must be provided", nameof(dto.User));
            if (string.IsNullOrWhiteSpace(dto.User.Email)) throw new ArgumentException("Email is required", nameof(dto.User.Email));
            if (string.IsNullOrWhiteSpace(dto.User.Password)) throw new ArgumentException("Password is required", nameof(dto.User.Password));
            if (string.IsNullOrWhiteSpace(dto.User.UserName)) throw new ArgumentException("Username is required", nameof(dto.User.UserName));

            var existingUserByEmail = await _userManager.FindByEmailAsync(dto.User.Email);
            if (existingUserByEmail != null)
                throw new InvalidOperationException("A user with this email already exists.");

            var existingUserByUsername = await _userManager.FindByNameAsync(dto.User.UserName);
            if (existingUserByUsername != null)
                throw new InvalidOperationException("A user with this username already exists.");

            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                // Map and create the new user
                var newUser = _mapper.Map<User>(dto.User);
                newUser.Role = RoleEnum.Restaurant;
                newUser.CreatedAt = DateTime.UtcNow;

                var userCreateResult = await _userManager.CreateAsync(newUser, dto.User.Password);
                if (!userCreateResult.Succeeded)
                {
                    var errors = string.Join("; ", userCreateResult.Errors.Select(e => e.Description));
                    throw new InvalidOperationException($"Failed to create user: {errors}");
                }

                var roleAssignResult = await _userManager.AddToRoleAsync(newUser, "Restaurant");
                if (!roleAssignResult.Succeeded)
                {
                    var errors = string.Join("; ", roleAssignResult.Errors.Select(e => e.Description));
                    throw new InvalidOperationException($"Failed to assign role: {errors}");
                }

                //// Save logo file if provided
                //if (logoFile != null && logoFile.Length > 0)
                //{
                //    dto.SavedLogoUrl = await SaveImageAsync(logoFile);
                //}

                // Map restaurant dto and assign references
                var restaurantEntity = _mapper.Map<Restaurant>(dto);
                restaurantEntity.RestaurantID = newUser.Id;
                restaurantEntity.User = newUser;

                if (string.IsNullOrWhiteSpace(restaurantEntity.RestaurantID))
                {
                    restaurantEntity.RestaurantID = Guid.NewGuid().ToString();
                }

                // Save logo file if provided
                if (dto.LogoFile != null && dto.LogoFile.Length > 0)
                {
                    restaurantEntity.ImageFile = await SaveImageAsync(dto.LogoFile);
                }
                else
                {
                    restaurantEntity.ImageFile = "wwwroot/restaurantLogo.jpg"; // default logo
                }

                restaurantEntity.IsActive = false;

                restaurantEntity.User.Restaurant = null;

                // Save restaurant to DB
                await _repository.ApplyToJoinAsync(restaurantEntity);

                // Commit transaction only if all succeeded
                await transaction.CommitAsync();

                return restaurantEntity;
            }
            catch
            {
                // Rollback transaction contacts everything created within this scope
                await transaction.RollbackAsync();

                // delete user if created outside transaction (UserManager may create user outside EF)
                var userToDelete = await _userManager.FindByNameAsync(dto.User.UserName);
                if (userToDelete != null)
                {
                    await _userManager.DeleteAsync(userToDelete);
                }

                throw; // rethrow error
            }
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
                existingRestaurant.ImageFile = await SaveImageAsync(dto.LogoFile);


            if (dto.IsAvailable.HasValue)
                existingRestaurant.IsAvailable = dto.IsAvailable.Value;

            if (!string.IsNullOrWhiteSpace(dto.User.Email))
                existingRestaurant.User.Email = dto.User.Email;

            if (!string.IsNullOrWhiteSpace(dto.User.Phone))
                existingRestaurant.User.PhoneNumber = dto.User.Phone;

            return await _repository.UpdateRestaurantAsync(existingRestaurant);
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