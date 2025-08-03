using FoodOrderingAPI.DTO;
using FoodOrderingAPI.Interfaces;
using FoodOrderingAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace FoodOrderingAPI.Controllers
{
    [Authorize(Roles = "Customer")]
    [Route("api/[controller]")]
    [ApiController]
    public class AddressController : ControllerBase
    {
        public IAddressRepo addressRepo;
        public AddressController(IAddressRepo addressRepo) {
            this.addressRepo = addressRepo;
        }
        [HttpGet]
        public async Task<IActionResult> GetAll(string UserName)
        {
            var userNameClaim = User.FindFirstValue(ClaimTypes.Name);
            if (userNameClaim != UserName)
            {
                return Forbid("You are not authorized to View this Customer's Address.");
            }
            return Ok(await addressRepo.GetAllAddresses(UserName));
        }
        [HttpGet("ById")]
        public async Task<IActionResult> GetById(Guid AddressId)
        {
            Address address = await addressRepo.GetAddress(AddressId);
            if (address ==null) 
                return NotFound();
            return Ok();
        }
        [HttpPost]
        public async Task<IActionResult> Add(string userName, AddressDTO address)
        {
            var userNameClaim = User.FindFirstValue(ClaimTypes.Name);
            if (userNameClaim != userName)
            {
                return Forbid("You are not authorized to add adddress to this Customer's Address.");
            }
            if (ModelState.IsValid)
            {
                await addressRepo.Add(userName, address);
                await addressRepo.Save();
                return Ok("add Address Successfully");
            }
            else
            {
                return BadRequest(ModelState);
            }

        }
        [HttpPut]
        public async Task<IActionResult> Update(Guid AddressId, AddressDTO addressdto)
        {
            Address address= await addressRepo.GetAddress(AddressId);
            if (address ==null)
                return NotFound();
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userIdClaim != address.CustomerID)
            {
                return Forbid("You are not authorized to Update this Customer's Address.");
            }
            if (ModelState.IsValid)
            {
                if (await addressRepo.Update(AddressId, addressdto))
                {
                    await addressRepo.Save();
                    return Ok("add Address Successfully");
                }
                else return NotFound();
            }

            else
            {
                return BadRequest(ModelState);
            }

        }
        [HttpDelete]
        public async Task<IActionResult> Delete(Guid AddressId)
        {
            Address address = await addressRepo.GetAddress(AddressId);
            if (address == null)
                return NotFound();
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userIdClaim != address.CustomerID)
            {
                return Forbid("You are not authorized to Delete this Customer's Address.");
            }
            if (await addressRepo.Delete(AddressId))
            {
                await addressRepo.Save();
                return Ok("Address Deleted");
            }
            else
                return
                    NotFound();
        }
    }
}
