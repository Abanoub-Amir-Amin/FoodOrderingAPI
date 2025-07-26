using AutoMapper;
using FoodOrderingAPI.DTO;
using FoodOrderingAPI.Models;
using FoodOrderingAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
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

        public DeliveryManController(ApplicationDBContext context, IDeliveryManService service, IMapper mapper)
        {
            _context = context;
            _service = service;
            _mapper = mapper;
        }

        [HttpPost("apply")]
        [AllowAnonymous]
        public async Task<IActionResult> ApplyToJoin([FromBody] DeliveryManDto dto)
        {
            try
            {
                // Call service to apply and create DeliveryMan user
                var result = await _service.ApplyToJoinAsync(dto);

                // Return 201 Created with route to newly created resource
                // return CreatedAtAction(nameof(GetRestaurantById), new { id = result.UserId }, result);
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
        public async Task<IActionResult> UpdateProfile([FromBody] DeliveryManDto deliveryManDto)
        {
            try
            {
                // Get deliveryMan Id from token payLoad
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

                var updatedProfile = await _service.UpdateProfileAsync(userId, deliveryManDto);
                return Ok(updatedProfile);
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
        [ProducesResponseType(typeof(DeliveryManDto), 200)]
        [ProducesResponseType(404)]
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
        [ProducesResponseType(typeof(DeliveryManDto), 200)]
        [ProducesResponseType(404)]
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
                    UserID = deliveryMan.User.Id,
                    Email = deliveryMan.User.Email,
                    UserName = deliveryMan.User.UserName
                }
            };
        }
    }
}
