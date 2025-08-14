using AutoMapper;
using FoodOrderingAPI.DTO;
using FoodOrderingAPI.Models;
using FoodOrderingAPI.Repository;
using FoodOrderingAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Index.HPRtree;

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

            var discount = _mapper.Map<Discount>(dto);
            discount.ItemID = itemId;

            var result = await _service.AddDiscountAsync(restaurantId, discount);

            return Ok(result);
        }

        [HttpPut("discounts/{discountId}")]
        public async Task<IActionResult> UpdateDiscount(int discountId, [FromBody] DiscountDto dto)
        {

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var updatedDiscount = await _service.UpdateDiscountAsync(discountId, dto);

                if (updatedDiscount == null)
                {
                    return NotFound($"Discount with ID '{discountId}' not found.");
                }


            var updatedDiscount = await _service.UpdateDiscountAsync(discount);
            return Ok(updatedDiscount);
        }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "An error occurred while updating the discount info: " + ex.ToString() });
            }
        }


        [HttpDelete("discounts/{discountId}")]
        public async Task<IActionResult> DeleteDiscount(int discountId)
        {
            var success = await _service.DeleteDiscountAsync(discountId);
            if (!success)
                return NotFound();

            return NoContent();
        }

        [HttpGet("{restaurantId}/discounts")]
        public async Task<IActionResult> GetDiscountsByRestaurant(string restaurantId)
        {
            var restaurant = await _context.Restaurants
                .FirstOrDefaultAsync(r => r.UserId.ToString() == restaurantId);

            if (restaurant == null)
                return NotFound($"Restaurant with ID '{restaurantId}' not found.");

            if (!restaurant.IsActive)
                return Forbid("Your restaurant account is not yet active.");

            var discounts = await _service.GetDiscountsByRestaurantAsync(restaurantId);

            var dtoList = _mapper.Map<IEnumerable<DiscountDto>>(discounts);
            return Ok(dtoList);
        }
    }
}
