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
using System.Security.Claims;

namespace FoodOrderingAPI.Controllers
{
    [EnableCors("AllowAngularDevClient")]
    [Authorize(Roles = "Restaurant")]
    [Route("api/[controller]")]
    [ApiController]
    public class PromoCodeController : ControllerBase
    {
        private readonly ApplicationDBContext _context;
        private readonly IPromoCodeService _service;
        private readonly IShoppingCartRepository _shoppingCartRepository;
        private readonly IMapper _mapper;
        private readonly IWebHostEnvironment _environment;
        private readonly IItemRepo itemRepo;

        public PromoCodeController(IPromoCodeService service,IShoppingCartRepository shoppingCartRepository, ApplicationDBContext context, IMapper mapper, IWebHostEnvironment environment)

        {
            _service = service;
            _shoppingCartRepository = shoppingCartRepository;
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
                Code = dto.Code,
                DiscountPercentage = dto.DiscountPercentage,
                IsFreeDelivery = dto.IsFreeDelivery,
                ExpiryDate = dto.ExpiryDate,
                UsageLimit = dto.UsageLimit
            };

            var result = await _service.AddPromoCodeAsync(restaurantId, promoCode);

            return Ok(result);
        }

        [HttpPut("{restaurantId}/promocodes/{promoCodeId}")]
        public async Task<IActionResult> UpdatePromoCode(string restaurantId, Guid promoCodeId, [FromBody] PromoCodeDto dto)
        {
            var promoCode = _mapper.Map<PromoCode>(dto);
            promoCode.PromoCodeID = promoCodeId;
            promoCode.RestaurantID = restaurantId;

            var updatedPromoCode = await _service.UpdatePromoCodeAsync(promoCode);
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
        public async Task<IActionResult> GetPromoCodes([FromRoute] string restaurantId, [FromQuery] string? code)
        {
            var restaurant = await _context.Restaurants
                .FirstOrDefaultAsync(r => r.UserId.ToString() == restaurantId && r.User.Role == RoleEnum.Restaurant);

            if (restaurant == null)
                return NotFound($"Restaurant with ID '{restaurantId}' not found.");

            if (!restaurant.IsActive)
                return Forbid("Your restaurant account is not yet active.");

            IEnumerable<PromoCode> promoCodes;

            if (string.IsNullOrWhiteSpace(code))
            {
                promoCodes = await _service.GetAllPromoCodesByRestaurantAsync(restaurantId);
            }
            else
            {
                promoCodes = await _service.SearchPromoCodesByCodeAsync(restaurantId, code);
            }

            var dtoList = _mapper.Map<IEnumerable<PromoCodeDto>>(promoCodes);
            return Ok(dtoList);
        }
        [Authorize(Roles = "Customer")]
        [HttpGet("ValidatePromoCode")]
        public async Task<IActionResult> ValidatePromoCode(string restaurantId, string code)
        {
            var userName = User.FindFirstValue(ClaimTypes.Name);

            ShoppingCart shoppingcart = await _shoppingCartRepository.getByCustomer(userName);
            if(shoppingcart == null)
            {
                return BadRequest("customer or shopping cart not found");
            }
            PromoCodeApplyDto pcode = await _service.ValidatePromoCode(restaurantId, code,shoppingcart.SubTotal,shoppingcart.Restaurant.DelivaryPrice);
            if (pcode.IsVaild)
                return Ok(pcode);
            else
                return BadRequest(pcode);
        }


    }
}
