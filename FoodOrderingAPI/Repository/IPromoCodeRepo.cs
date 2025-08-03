using FoodOrderingAPI.Models;

namespace FoodOrderingAPI.Repository
{
    public interface IPromoCodeRepo
    {
        //PromoCode-CRUD
        Task<PromoCode> AddPromoCodeAsync(string restaurantId, PromoCode promoCode);
        Task<PromoCode> UpdatePromoCodeAsync(PromoCode promoCode);
        Task<bool> DeletePromoCodeAsync(Guid promoCodeId, string restaurantId);
        Task<IEnumerable<PromoCode>> GetAllPromoCodesByRestaurantAsync(string restaurantId);
        Task<IEnumerable<PromoCode>> SearchPromoCodesByCodeAsync(string restaurantId, string code);
    }
}
