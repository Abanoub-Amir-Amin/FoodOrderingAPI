using FoodOrderingAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace FoodOrderingAPI.Repository
{
    public class PromoCodeRepo : IPromoCodeRepo
    {
        private readonly ApplicationDBContext _context;

        public PromoCodeRepo(ApplicationDBContext context)
        {
            _context = context;
        }
        // ===== Promo Codes CRUD =====
        public async Task<PromoCode> AddPromoCodeAsync(string restaurantId, PromoCode promoCode)
        {
            promoCode.IssuedByID = restaurantId;
            _context.PromoCodes.Add(promoCode);
            await _context.SaveChangesAsync();
            return promoCode;
        }

        public async Task<PromoCode> UpdatePromoCodeAsync(PromoCode promoCode)
        {
            await _context.SaveChangesAsync();
            return promoCode;
        }


        public async Task<bool> DeletePromoCodeAsync(Guid promoCodeId, string restaurantId)
        {
            var promoCode = await _context.PromoCodes
                .FirstOrDefaultAsync(p => p.PromoCodeID == promoCodeId);
            if (promoCode == null) return false;

            _context.PromoCodes.Remove(promoCode);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<PromoCode>> GetAllPromoCodesByRestaurantAsync(string restaurantId)
        {
            //return await _context.PromoCodes
            //    .Where(p => p.IssuedByID == restaurantId)
            //    .ToListAsync();
            return new List<PromoCode>();
        }

        public async Task<IEnumerable<PromoCode>> SearchPromoCodesByCodeAsync(string restaurantId, string code)
        {
            return await _context.PromoCodes
                .Where(p => p.IssuedByID == restaurantId && p.Code.Contains(code))
                .ToListAsync();
        }

        public async Task<PromoCode?> GetPromoCodeByIdAsync(Guid promoCodeId)
        {
            return await _context.PromoCodes
                .FirstOrDefaultAsync(p => p.PromoCodeID == promoCodeId);
        }
    }
}
