using FoodOrderingAPI.DTO;
using FoodOrderingAPI.Models;
using FoodOrderingAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace FoodOrderingAPI.Controllers
{
    [Authorize(Roles = "Customer")]

    [Route("api/[controller]")]
    [ApiController]
    public class CustomerController : ControllerBase
    {
        ICustomerServices customerServices;
        private readonly UserManager<User> userManager;
        private readonly IConfirmationEmail confirmationEmail;

        public CustomerController(ICustomerServices customerServices, UserManager<User> userManager, IConfirmationEmail confirmationEmail) {
            this.customerServices = customerServices;
            this.userManager = userManager;
            this.confirmationEmail = confirmationEmail;
        }
        //[HttpGet("All")]
        //public async Task<IActionResult> GetAll()
        //{
        //    List<CustomerDTO> data = await customerServices.GetAll();
        //    if (data == null) return BadRequest();
        //    return Ok(data);
        //}
        [HttpGet("ByID/{id}")]
        public async Task<IActionResult> GetById(string id)
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userIdClaim != id)
            {
                return Forbid("You are not authorized to View this Customer's profile.");
            }

            CustomerDTO data = await customerServices.GetCusomerDashboardDataById(id);
            if (data == null) return NotFound();
            return Ok(data);
        }
        [HttpGet("ByEmail/{email}")]

        public async Task<IActionResult> GetByEmail(string email)
        {
            var userEmailClaim = User.FindFirstValue(ClaimTypes.Email);
            if (userEmailClaim != email)
            {
                return Forbid("You are not authorized to View this Customer's profile.");
            }

            CustomerDTO data = await customerServices.GetCusomerDashboardDataByEmail(email);
            if (data == null) return BadRequest();
            return Ok(data);
        }
        [HttpGet("ByUserName/{Username}")]
        public async Task<IActionResult> GetByUsername(string Username)
        {
            var userNameClaim = User.FindFirstValue(ClaimTypes.Name);
            if (userNameClaim != Username)
            {
                return Forbid("You are not authorized to View this Customer's profile.");
            }

            CustomerDTO data = await customerServices.GetCusomerDashboardDataByUserName(Username);
            if (data == null) return BadRequest();
            return Ok(data);
        }
        [AllowAnonymous]
        [HttpPost("Register")]
        public async Task<IActionResult> Register(RegisterCustomerDTO customer)
        {
            IdentityResult result = await customerServices.Register(customer);
            if (result.Succeeded)
            {
                await confirmationEmail.SendConfirmationEmail(customer.Email, await userManager.FindByEmailAsync(customer.Email));
                return Created();
            }
            else
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("Creation error", error.Description);
                }
            }
            return BadRequest(ModelState);
        }
        [HttpPut("UpdateCustomer")]
        public async Task<IActionResult> Update(string CustomerId,UpdateCustomerDTO customer)
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userIdClaim != CustomerId)
            {
                return Forbid("You are not authorized to Update this Customer's profile.");
            }
            if (ModelState.IsValid)
            {
                if (await customerServices.UpdateCustomer(CustomerId, customer) == true)
                {
                    await customerServices.Save();
                    return Ok();
                }
                else
                    return NotFound();
            }
            else
                return BadRequest(ModelState);
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
        
        //[HttpDelete]
        //public async Task<IActionResult>Delete(string CustomerId)
        //{
        //    if (await customerServices.DeleteCustomer(CustomerId))
        //    {
        //        await customerServices.Save();
        //        return Ok();
        //    }
        //    else
        //        return BadRequest();

        //}

    }
}
