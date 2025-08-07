using AutoMapper;
using FoodOrderingAPI.DTO;
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
        public async Task<PromoCode> GetPromoCodesByCodeAsync(string restaurantId, string code)
        {
            if (string.IsNullOrWhiteSpace(restaurantId))
                throw new ArgumentException("Restaurant ID must be provided.", nameof(restaurantId));

            if (string.IsNullOrWhiteSpace(code))
                throw new ArgumentException("Promocode must be provided.", nameof(code));

            if (!Guid.TryParse(restaurantId, out Guid rid))
                throw new ArgumentException("Invalid Restaurant ID format.", nameof(restaurantId));

            return await _repository.GetPromoCodesByCodeAsync(restaurantId, code);
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
        public async Task<PromoCodeApplyDto> ValidatePromoCode(string restaurantId, string code,decimal subtotal,decimal delivaryPrice)
        {
            PromoCode pcode;
            try
            {
                pcode = await GetPromoCodesByCodeAsync(restaurantId, code);
            }
            catch(Exception ex)
            {
                return new PromoCodeApplyDto
                {
                    IsVaild = false,
                    message = ex.Message
                };
            }
            if (pcode == null)
                return new PromoCodeApplyDto
                {
                    IsVaild = false,
                    message = "this promocode Not Found"
                };
            if(DateTime.Now> pcode.ExpiryDate)
                return new PromoCodeApplyDto
                {
                    IsVaild = false,
                    message = "promocode expired cannot be applied"
                };
            if(await _repository.getPromoCodeUses(code)>= pcode.UsageLimit && pcode.UsageLimit>0)
                return new PromoCodeApplyDto
                {
                    IsVaild = false,
                    message = "This promo code has reached its maximum usage limit. "
                };
            return new PromoCodeApplyDto
            {
                IsVaild = true,
                PromoCodeID = pcode.PromoCodeID,
                SubTotal = subtotal,
                DiscountAmount = ((decimal)pcode.DiscountPercentage * subtotal) / 100,
                DelivaryPrice = pcode.IsFreeDelivery ? delivaryPrice : 0,
                message="PromoCode applied successfully"
            };
        }
        public async Task<bool> ApplyPromoCode(Order order)
        {
            PromoCode pcode = await _repository.GetPromoCodesById((Guid)order.PromoCodeID);
        if (pcode == null)
            {
                return false;
            }
            PromoCodeApplyDto result = await ValidatePromoCode(order.RestaurantID, pcode.Code, order.SubTotal, order.DelivaryPrice);
            if (result.IsVaild)
            {
                order.SubTotal = result.SubTotal;
                order.DiscountAmount = result.DiscountAmount;
                order.DelivaryPrice = result.DelivaryPrice;
                return true;
            }
            return false;
        }

    }
}
