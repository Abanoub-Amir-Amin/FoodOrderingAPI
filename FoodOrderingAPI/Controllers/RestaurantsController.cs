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
using System;
using System.Security.Claims;
using System.Threading.Tasks;

namespace FoodOrderingAPI.Controllers
{
    [EnableCors("AllowAngularDevClient")]
    [Authorize(Roles = "Restaurant")]
    [ApiController]
    [Route("api/[controller]")]
    public class RestaurantController : ControllerBase
    {
        private readonly ApplicationDBContext _context;  
        private readonly IRestaurantService _service;    
        private readonly IMapper _mapper;
        private readonly IWebHostEnvironment _environment;
        private readonly IItemRepo itemRepo;

        public RestaurantController(IRestaurantService service, ApplicationDBContext context, IMapper mapper, IWebHostEnvironment environment)
        
        {
            _service = service;
            _context = context;
            _mapper = mapper;
            _environment = environment;
            this.itemRepo = itemRepo;

        }

        // ===== Restaurant Apply to Join =====
        [HttpPost("apply")]
        [Consumes("multipart/form-data")]  // Ensure it accepts multipart/form-data
        [AllowAnonymous]
        public async Task<IActionResult> ApplyToJoin([FromForm] RestaurantUpdateDto dto)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors)
                                              .Select(e => e.ErrorMessage)
                                              .ToList();
                return BadRequest(new { error = "Validation failed", details = errors });
            }
            try
            {
                // Call service to apply and create restaurant user
                var result = await _service.ApplyToJoinAsync(dto);

                // Return 201 Created with route to newly created resource
                return CreatedAtAction(nameof(GetRestaurantById), new { id = result.UserId }, result);
            }
            catch (InvalidOperationException ex) when (ex.Message.Contains("already exists"))
            {
                // Duplicate user detected - return client-friendly 400 Bad Request
                return BadRequest(new { error = ex.Message });
            }
            catch (ArgumentException ex)
            {
                // Input validation error
                return BadRequest(new { error = ex.Message });
            }
            catch (Exception ex)
            {
                // Unexpected error - return 500 Internal Server Error with message
                return StatusCode(500, new { error = ex.Message });
            }
        }

        // ===== Restaurant Profile =====
        [HttpGet("{id}")]
        public async Task<IActionResult> GetRestaurantById(string id)
        {
            try
            {
                var restaurant = await _service.GetRestaurantByIdAsync(id);

                if (restaurant == null)
                    return NotFound();

                var dto = _mapper.Map<RestaurantDto>(restaurant);
                return Ok(dto);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }

        [HttpGet("GetRestaurants")]
        [AllowAnonymous]
        public async Task<IActionResult> GetAllRestaurants()
        {
            try
            {
                var restaurant = await _service.GetAllRestaurantsAsync();

                if (restaurant == null)
                    return NotFound();

                var dto = _mapper.Map<List<AllRestaurantsDTO>>(restaurant);
                return Ok(dto);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }


        [HttpPut("{restaurantId}/update-profile")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> UpdateRestaurantProfile(string restaurantId, [FromForm] RestaurantUpdateDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Authorizing that the authenticated user is the owner of this restaurant ID
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userIdClaim != restaurantId)
            {
                return Forbid("You are not authorized to update this restaurant's profile.");
            }

            try
            {
                var updatedRestaurant = await _service.UpdateRestaurantProfileAsync(restaurantId, dto);

                if (updatedRestaurant == null)
                {
                    return NotFound($"Restaurant with ID '{restaurantId}' not found.");
                }

                // Map the updated entity back to a DTO for the response
                var responseDto = _mapper.Map<RestaurantDto>(updatedRestaurant);
                return Ok(responseDto);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "An error occurred while updating the restaurant profile: " + ex.Message });
            }
        }

        // ===== Image Upload =====

        // POST api/restaurants/logo-upload
        [HttpPost("logo-upload")]
        [AllowAnonymous]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> UploadLogo([FromForm] FileUploadDto fileUpload)
        {
            var file = fileUpload?.File;
            if (file == null || file.Length == 0)
                return BadRequest(new { error = "No file provided" });

            var url = await _service.SaveImageAsync(file);
            return Ok(new { url });
        }


    }
}
