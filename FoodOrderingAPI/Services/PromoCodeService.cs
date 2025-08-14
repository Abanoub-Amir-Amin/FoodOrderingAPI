using AutoMapper;
using FoodOrderingAPI.Controllers;
using FoodOrderingAPI.DTO;
using FoodOrderingAPI.Models;
using FoodOrderingAPI.Repository;
using Microsoft.AspNetCore.Identity;
using NetTopologySuite.Index.HPRtree;

namespace FoodOrderingAPI.Services
{
    public class PromoCodeService : IPromoCodeService
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
            promoCode.IssuedByID = restaurantId;
            promoCode.IssuedByType = RoleEnum.Restaurant.ToString();
            return await _repository.AddPromoCodeAsync(restaurantId, promoCode);
        }


        public async Task<PromoCode?> UpdatePromoCodeAsync(string restaurantId, Guid promoCodeId, PromoCodeDto dto)
        {
            var existingCode = await _repository.GetPromoCodeByIdAsync(promoCodeId);
            if (existingCode == null)
                return null;


            existingCode.IssuedByID = restaurantId;
            existingCode.IssuedByType = RoleEnum.Restaurant.ToString();

            if (dto.ExpiryDate != null)
            {
                existingCode.ExpiryDate = dto.ExpiryDate.Value;
            }

            if (dto.Code != null)
            {
                existingCode.Code = dto.Code;
            }

            if (dto.DiscountPercentage != null)
            {
                existingCode.DiscountPercentage = dto.DiscountPercentage.Value;
            }

            if (dto.IsFreeDelivery != null)
            {
                existingCode.IsFreeDelivery = dto.IsFreeDelivery.Value;
            }

            if (dto.UsageLimit != null)
            {
                existingCode.UsageLimit = dto.UsageLimit.Value;
            }

            return await _repository.UpdatePromoCodeAsync(existingCode);

        }





        public async Task<bool> DeletePromoCodeAsync(Guid promoCodeId, string restaurantId)
        {
            var existingCode = await _repository.GetPromoCodeByIdAsync(promoCodeId);
            if (existingCode == null)
                return false;

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

        public async Task<PromoCode?> GetPromoCodeByIdAsync(Guid promoCodeId)
        {
            return await _repository.GetPromoCodeByIdAsync(promoCodeId);
        }
    }
}
