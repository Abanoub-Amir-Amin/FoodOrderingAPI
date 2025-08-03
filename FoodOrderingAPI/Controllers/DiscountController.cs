using AutoMapper;
using FoodOrderingAPI.DTO;
using FoodOrderingAPI.Models;
using FoodOrderingAPI.Repository;
using FoodOrderingAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;

namespace FoodOrderingAPI.Controllers
{
    [EnableCors("AllowAngularDevClient")]
    [Authorize(Roles = "Restaurant")]
    [Route("api/[controller]")]
    [ApiController]
    public class DiscountController : ControllerBase
    {
        private readonly ApplicationDBContext _context;
        private readonly IDiscountService _service;
        private readonly IMapper _mapper;
        private readonly IWebHostEnvironment _environment;
        private readonly IItemRepo itemRepo;

        public DiscountController(IDiscountService service, ApplicationDBContext context, IMapper mapper, IWebHostEnvironment environment)

        {
            _service = service;
            _context = context;
            _mapper = mapper;
            _environment = environment;
            this.itemRepo = itemRepo;

        }
        // ===== Discounts CRUD =====
        [HttpPost("{restaurantId}/discounts/{itemId}")]
        public async Task<IActionResult> AddDiscount(string restaurantId, Guid itemId, [FromBody] DiscountDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var discount = new Discount
            {
                ItemID = itemId,
                Percentage = dto.Percentage,
                StartDate = dto.StartDate,
                EndDate = dto.EndDate,
            };

            var result = await _service.AddDiscountAsync(restaurantId, discount);

            return Ok(result);
        }

        [HttpPut("{restaurantId}/discounts/{discountId}")]
        public async Task<IActionResult> UpdateDiscount(string restaurantId, int discountId, [FromBody] DiscountDto dto)
        {
            var discount = _mapper.Map<Discount>(dto);
            discount.DiscountID = discountId;
            discount.RestaurantID = restaurantId;

            var updatedDiscount = await _service.UpdateDiscountAsync(discount);
            return Ok(updatedDiscount);
        }

        [HttpDelete("{restaurantId}/discounts/{discountId}")]
        public async Task<IActionResult> DeleteDiscount(string restaurantId, int discountId)
        {
            var success = await _service.DeleteDiscountAsync(discountId, restaurantId);
            if (!success) return NotFound();
            return NoContent();
        }
    }
}
