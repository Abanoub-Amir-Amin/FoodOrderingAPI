using AutoMapper;
using FoodOrderingAPI.DTO;
using FoodOrderingAPI.Models;
using FoodOrderingAPI.Repository;
using FoodOrderingAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Security.Claims;
using System.Threading.Tasks;

namespace FoodOrderingAPI.Controllers
{
    //[EnableCors("AllowAngularDevClient")]
    //[Authorize(Roles = "Restaurant")]
    [ApiController]
    [Route("api/[controller]")]
    public class RestaurantController : ControllerBase
    { 
        private readonly ApplicationDBContext _context;
        private readonly IRestaurantService _service;    
        private readonly IMapper _mapper;
        private readonly IWebHostEnvironment _environment;
        private readonly UserManager<User> userManager;
        private readonly IConfirmationEmail confirmationEmail;

        public RestaurantController(IRestaurantService service, ApplicationDBContext context, IMapper mapper, IWebHostEnvironment environment, IConfirmationEmail confirmationEmail, UserManager<User> userManager)
        
        {
            _service = service;
            _context = context;
            _mapper = mapper;
            _environment = environment;
            this.confirmationEmail = confirmationEmail;
            this.userManager = userManager;

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
                await confirmationEmail.SendConfirmationEmail(dto.User.Email, await userManager.FindByEmailAsync(dto.User.Email));
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
                dto.ImageFile = restaurant.ImageFile;

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
                var errors = ModelState.Values.SelectMany(v => v.Errors)
                                              .Select(e => e.ErrorMessage)
                                              .ToList();
                return BadRequest(new { errors });
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

                return Ok(updatedRestaurant);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "An error occurred while updating the restaurant profile: " + ex.ToString() });
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

        [AllowAnonymous]
        [HttpGet("confirm-email")]
        public async Task<IActionResult> ConfirmEmail(string UserId, string Token)
        {
            if (string.IsNullOrEmpty(UserId) || string.IsNullOrEmpty(Token))
            {
                // Provide a descriptive error message for the view
                return BadRequest("The link is invalid or has expired. Please request a new one if needed.");
            }
            //Find the User by Id
            var user = await userManager.FindByIdAsync(UserId);
            if (user == null)
            {
                // Provide a descriptive error for a missing user scenario
                return NotFound("We could not find a user associated with the given link.");

            }
            // Attempt to confirm the email
            var result = await userManager.ConfirmEmailAsync(user, Token);
            if (result.Succeeded)
            {
                return Ok("Thank you for confirming your email address. Your account is now verified!");
            }
            // If confirmation fails
            return BadRequest("We were unable to confirm your email address. Please try again or request a new link.");

        }

        //===== Loction =====
        [HttpPost]
        public async Task<IActionResult> UpdateLocation(string restaurantId, [FromBody] RestaurantDto restaurant)
        {
            try
            {
                await _service.SetRestaurantLocation(restaurantId, restaurant.Latitude, restaurant.Longitude);
                return Ok("Location updated successfully.");
        }
            catch (Exception ex)
            {
                // Optionally log the exception here
                return BadRequest(ex.Message);
            }
        }


    }
}
