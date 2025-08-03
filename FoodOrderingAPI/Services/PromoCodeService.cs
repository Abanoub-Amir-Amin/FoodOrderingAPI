using AutoMapper;
using FoodOrderingAPI.Models;
using FoodOrderingAPI.Repository;
using Microsoft.AspNetCore.Identity;

namespace FoodOrderingAPI.Services
{
    public class PromoCodeService:IPromoCodeService
    {
        private readonly IWebHostEnvironment _environment;
        private readonly ApplicationDBContext _context;
        private readonly IPromoCodeRepo _repository;
        private readonly IMapper _mapper;
        private readonly UserManager<User> _userManager;


        public PromoCodeService(IPromoCodeRepo repository, ApplicationDBContext context, IMapper mapper, UserManager<User> userManager, IWebHostEnvironment environment)
        {
            _repository = repository;
            _context = context;
            _mapper = mapper;
            _userManager = userManager;
            _environment = environment ?? throw new ArgumentNullException(nameof(environment));
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
    }
}
