using FoodOrderingAPI.DTO;
using FoodOrderingAPI.Models;

namespace FoodOrderingAPI.Services
{
    public interface IPromoCodeService
    {
        //PromoCode-CRUD
        public Task<PromoCode> GetPromoCodesByCodeAsync(string restaurantId, string code);
        Task<PromoCode> AddPromoCodeAsync(string restaurantId, PromoCode promoCode);
        Task<PromoCode> UpdatePromoCodeAsync(PromoCode promoCode);
        Task<bool> DeletePromoCodeAsync(Guid promoCodeId, string restaurantId);
        Task<IEnumerable<PromoCode>> GetAllPromoCodesByRestaurantAsync(string restaurantId);
        Task<IEnumerable<PromoCode>> SearchPromoCodesByCodeAsync(string restaurantId, string code);
        public Task<PromoCodeApplyDto> ValidatePromoCode(string restaurantId, string code, decimal subtotal,decimal delivaryPrice);
        public Task<bool> ApplyPromoCode(Order order);


    }
}
