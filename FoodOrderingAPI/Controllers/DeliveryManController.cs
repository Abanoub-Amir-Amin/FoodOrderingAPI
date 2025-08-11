using AutoMapper;
using FoodOrderingAPI.DTO;
using FoodOrderingAPI.Models;
using FoodOrderingAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace FoodOrderingAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DeliveryManController : ControllerBase
    {
        private readonly ApplicationDBContext _context;
        private readonly IDeliveryManService _service;
        private readonly IMapper _mapper;
        private readonly UserManager<User> userManager;
        private readonly IConfirmationEmail confirmationEmail;

        public DeliveryManController(ApplicationDBContext context, IDeliveryManService service, IMapper mapper, UserManager<User> userManager, IConfirmationEmail confirmationEmail)
        {
            _context = context;
            _service = service;
            _mapper = mapper;
            this.userManager = userManager;
            this.confirmationEmail = confirmationEmail;
        }

        [HttpPost("apply")]
        [AllowAnonymous]
        public async Task<IActionResult> ApplyToJoin([FromBody] DeliveryManApplyDto dto)
        {
            try
            {
                // Call service to apply and create DeliveryMan user
                var result = await _service.ApplyToJoinAsync(dto);

                // Return 201 Created with route to newly created resource
                // return CreatedAtAction(nameof(GetRestaurantById), new { id = result.UserId }, result);
                await confirmationEmail.SendConfirmationEmail(dto.Email, await userManager.FindByEmailAsync(dto.Email));
                return Created();
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

        [Authorize(Roles = "DeliveryMan")]
        [HttpGet("availability")]
        public async Task<IActionResult> GetAvailabilityStatus()
        {

            try
            {
                // Get deliveryMan Id from token payLoad
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

                var IsAvailable = await _service.GetAvailabilityStatusAsync(userId);

                return Ok(IsAvailable);
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

        [Authorize(Roles = "DeliveryMan")]
        [HttpPatch("UpdateAvailability")]
        public async Task<IActionResult> UpdateAvailabilityStatus([FromBody] bool availability)
        {

            try
            {
                // Get deliveryMan Id from token payLoad
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

                var IsUpdated = await _service.UpdateAvailabilityStatusAsync(userId, availability);

                if (IsUpdated)
                    return NoContent();
                else
                    return NotFound("Delivery man not found.");
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

        [Authorize(Roles = "DeliveryMan")]
        [HttpGet("profile")]
        public async Task<IActionResult> GetProfile()
        {
            try
            {
                // Get deliveryMan Id from token payLoad
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var profile = await _service.GetProfileAsync(userId);
                return Ok(profile);
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

        [Authorize(Roles = "DeliveryMan")]
        [HttpPut("profile")]
        public async Task<IActionResult> UpdateProfile([FromBody] DeliveryManProfileUpdateDTO deliveryManDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                // Get deliveryMan Id from token payLoad
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

                var updatedProfile = await _service.UpdateProfileAsync(userId, deliveryManDto);

                // Map the updated entity back to a DTO for the response
                var responseDto = _mapper.Map<DeliveryManProfileUpdateDTO>(updatedProfile);

                return Ok(responseDto);
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

        [HttpGet("claims")]
        public IActionResult ShowClaims()
        {
            var claims = User.Claims.Select(c => new { c.Type, c.Value }).ToList();
            return Ok(claims);
        }

        [HttpGet("best-delivery-man")]
        public async Task<ActionResult<DeliveryManDto>> GetBestAvailableDeliveryMan()
        {
            var deliveryMan = await _service.GetBestAvailableDeliveryManAsync();

            if (deliveryMan == null)
            {
                return NotFound("No available delivery men found");
            }
            return Ok(MapToDto(deliveryMan));
        }

        [HttpGet("closest-delivery-man")]
        public async Task<ActionResult<DeliveryManDto>> GetClosestDeliveryMan(double orderLatitude, double orderLongitude)
        {
            var deliveryMan = await _service.GetClosestDeliveryManAsync(orderLatitude, orderLongitude);

            if (deliveryMan == null)
            {
                return NotFound("No available delivery men found near the specified location");
            }
            return Ok(MapToDto(deliveryMan));
        }

        private DeliveryManDto MapToDto(DeliveryMan deliveryMan)
        {
            return new DeliveryManDto
            {
                DeliveryManId = deliveryMan.DeliveryManID,
                Latitude = deliveryMan.Latitude,
                Longitude = deliveryMan.Longitude,
                Rank = deliveryMan.Rank,
                AvailabilityStatus = deliveryMan.AvailabilityStatus,
                User = new UserDto
                {
                    Email = deliveryMan.User.Email,
                    UserName = deliveryMan.User.UserName
                }
            };
        }

        [Authorize(Roles = "DeliveryMan")]
        [HttpPatch("UpdateOrderStatus")]
        public async Task<IActionResult> UpdateOrderStatus([FromBody] DeliveryManOrderStatusDto dto)
        {
            // Get deliveryMan Id from token payLoad
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId != dto.DeliveryManId)
            {
                return Forbid("You are not authorized to update this order.");
            }
            try
            {
                var order = await _service.UpdateOrderStatusAsync(dto.OrderID, dto.Status, dto.DeliveryManId);

                if (order == null)
                    return NotFound();

                return Ok(order);// Have to be NoContent() or Ok(order) based on the requirement
            }
            catch (InvalidOperationException ex) // Catches the IsAvailable check from service
            {
                return Forbid(ex.Message);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "An error occurred while updating order status: " + ex.Message });
            }

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
    }
}
