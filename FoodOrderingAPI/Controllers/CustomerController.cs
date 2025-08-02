using FoodOrderingAPI.DTO;
using FoodOrderingAPI.Interfaces;
using FoodOrderingAPI.Models;
using FoodOrderingAPI.Repository;
using FoodOrderingAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
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
        public CustomerController(ICustomerServices customerServices) {
            this.customerServices = customerServices;
        }
        [HttpGet("All")]
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
