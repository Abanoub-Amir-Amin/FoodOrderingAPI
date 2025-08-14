using AutoMapper;
using FoodOrderingAPI.DTO;
using FoodOrderingAPI.Models;
using FoodOrderingAPI.Repository;
using FoodOrderingAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FoodOrderingAPI.Controllers
{
    //[EnableCors("AllowAngularDevClient")]
    //[Authorize(Roles = "Restaurant")]
    [Route("api/[controller]")]
    [ApiController]
    public class PromoCodeController : ControllerBase
    {
        private readonly ApplicationDBContext _context;
        private readonly IPromoCodeService _service;
        private readonly IMapper _mapper;
        private readonly IWebHostEnvironment _environment;
        private readonly IItemRepo itemRepo;

        public PromoCodeController(IPromoCodeService service, ApplicationDBContext context, IMapper mapper, IWebHostEnvironment environment)

        {
            _service = service;
            _context = context;
            _mapper = mapper;
            _environment = environment;
            this.itemRepo = itemRepo;

        }
        // ===== Promo Codes CRUD =====
        [HttpPost("{restaurantId}/promocodes")]
        public async Task<IActionResult> AddPromoCode(string restaurantId, [FromBody] PromoCodeDto dto)
        {
            var promoCode = new PromoCode
            {
                Code = dto.Code!,
                DiscountPercentage = dto.DiscountPercentage!.Value,
                IsFreeDelivery = dto.IsFreeDelivery!.Value,
                ExpiryDate = dto.ExpiryDate!.Value,
                UsageLimit = dto.UsageLimit!.Value
            };

            var result = await _service.AddPromoCodeAsync(restaurantId, promoCode);

            return Ok(result);
        }

        [HttpPut("{restaurantId}/promocodes/{promoCodeId}")]
        public async Task<IActionResult> UpdatePromoCode(string restaurantId, Guid promoCodeId, [FromBody] PromoCodeDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var updatedPromoCode = await _service.UpdatePromoCodeAsync(restaurantId, promoCodeId, dto);

            return Ok(updatedPromoCode);
        }


        [HttpDelete("{restaurantId}/promocodes/{promoCodeId}")]
        public async Task<IActionResult> DeletePromoCode(string restaurantId, Guid promoCodeId)
        {
            var success = await _service.DeletePromoCodeAsync(promoCodeId, restaurantId);
            if (!success) return NotFound();
            return NoContent();
        }

        [HttpGet("{restaurantId}/promocodes")]
        public async Task<IActionResult> GetPromoCodes([FromRoute] string restaurantId)
        {
            var restaurant = await _context.Restaurants
                .FirstOrDefaultAsync(r => r.UserId.ToString() == restaurantId && r.User.Role == RoleEnum.Restaurant);

            if (restaurant == null)
                return NotFound($"Restaurant with ID '{restaurantId}' not found.");

            if (!restaurant.IsActive)
                return Forbid("Your restaurant account is not yet active.");

            IEnumerable<PromoCode> promoCodes;

            promoCodes = await _service.GetAllPromoCodesByRestaurantAsync(restaurantId);

            var dtoList = _mapper.Map<IEnumerable<PromoCodeDto>>(promoCodes);
            return Ok(dtoList);
        }

        // GET: api/PromoCode/{restaurantId}/promocodes/{promoCodeId}
        [HttpGet("{restaurantId}/promocodes/{promoCodeId}")]
        public async Task<IActionResult> GetPromoCodeById(string restaurantId, Guid promoCodeId)
        {
            // Validate restaurant exists and is active
            var restaurant = await _context.Restaurants
                .FirstOrDefaultAsync(r => r.UserId.ToString() == restaurantId);

            if (restaurant == null)
                return NotFound($"Restaurant with ID '{restaurantId}' not found.");

            if (!restaurant.IsActive)
                return Forbid("Your restaurant account is not active.");

            var promoCode = await _service.GetPromoCodeByIdAsync(promoCodeId);

            if (promoCode == null)
                return NotFound($"Promo code with ID '{promoCodeId}' not found.");

            var dto = _mapper.Map<PromoCodeDto>(promoCode);

            return Ok(dto);
        }


    }
}
