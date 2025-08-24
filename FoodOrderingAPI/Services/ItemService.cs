using AutoMapper;
using FoodOrderingAPI.DTO;
using FoodOrderingAPI.Hubs;
using FoodOrderingAPI.Models;
using FoodOrderingAPI.Repository;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;

namespace FoodOrderingAPI.Services
{
    public class ItemService : IItemService
    {
        private readonly IWebHostEnvironment _environment;
        private readonly ApplicationDBContext _context;
        private readonly IItemRepo _repository;
        private readonly IMapper _mapper;
        private readonly UserManager<User> _userManager;

        public IHubContext<ItemHub> HubContext { get; }

        public ItemService(IHubContext<ItemHub> hubContext, IItemRepo repository, ApplicationDBContext context, IMapper mapper, UserManager<User> userManager, IWebHostEnvironment environment)
        {
            HubContext = hubContext;
            _repository = repository;
            _context = context;
            _mapper = mapper;
            _userManager = userManager;
            _environment = environment ?? throw new ArgumentNullException(nameof(environment));
        }
        //Item-CRUD
        public async Task<Item> AddItemAsync(string restaurantId, ItemUpdateDto dto)
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
                dto.ImageUrl = await SaveImageAsync(dto.ImageFile);
            }

            item.ImageFile = dto.ImageUrl;

            return await _repository.AddItemAsync(restaurantId, item);
        }

        public async Task CreateItemAsync(string restaurantId, ItemUpdateDto item)
        {
            await HubContext.Clients.All.SendAsync("ReceiveItem", item);
        }

        public async Task<Item> UpdateItemAsync(Guid itemId, ItemUpdateDto dto)
        {
            var existingItem = await _repository.GetItemByIdAsync(itemId);
            if (existingItem == null)
                return null;

            // Map update DTO onto existing entity 
            _mapper.Map(dto, existingItem);


            if (dto.ImageFile != null && dto.ImageFile.Length > 0)
            {
                dto.ImageUrl = await SaveImageAsync(dto.ImageFile);
            }
            existingItem.ImageFile = dto.ImageUrl;
            return await _repository.UpdateItemAsync(existingItem);
        }

        public async Task<List<Item>> GetAllItemsAsync()
        {
            return await _repository.GetAllItemsAsync();
        }

        public async Task<IEnumerable<ItemDto>> GetItemsByRestaurantNameAsync(string restaurantName)
        {
            return await _repository.GetItemsByRestaurantNameAsync(restaurantName);
        }
        public async Task<bool> DeleteItemAsync(Guid itemId)
        {
            return await _repository.DeleteItemAsync(itemId);
        }

        public async Task<Item> GetItemByIdAsync(Guid itemId)
        {

            if (itemId.ToString() == string.Empty)
                throw new ArgumentException("UserId is invalid", nameof(itemId));

            return await _repository.GetItemByIdAsync(itemId);
        }

        public async Task<IEnumerable<Item>> GetItemsByCategoryAsync(string restaurantId, string category)
        {
            if (string.IsNullOrWhiteSpace(category))
                throw new ArgumentException("Category must be provided.", nameof(category));
            if (string.IsNullOrWhiteSpace(restaurantId))
                throw new ArgumentException("Restaurant ID must be provided.", nameof(restaurantId));
            return await _repository.GetItemsByCategoryAsync(category);
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

        public async Task<List<string>> GetAllCategoriesAsync()
        {
            // Get all distinct categories from the Items table
            return await _repository.GetAllCategoriesAsync();
        }

        public async Task<IEnumerable<ItemDto>> GetItemsByIDRestaurantAsync(string restaurantId)
        {
            return await _repository.GetItemsByRestaurantIdAsync(restaurantId);
        }



    }
}
