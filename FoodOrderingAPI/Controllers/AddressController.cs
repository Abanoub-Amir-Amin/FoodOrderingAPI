using AutoMapper;
using FoodOrderingAPI.DTO;
using FoodOrderingAPI.Interfaces;
using FoodOrderingAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Stripe;
using System.Security.Claims;

namespace FoodOrderingAPI.Controllers
{
    [Authorize(Roles = "Customer")]
    [Route("api/[controller]")]
    [ApiController]
    public class AddressController : ControllerBase
    {
        public IAddressRepo addressRepo;
        public IMapper mapper;
        public AddressController(IAddressRepo addressRepo, IMapper mapper) {
            this.addressRepo = addressRepo;
            this.mapper = mapper;
        }
        [HttpGet]
        public async Task<IActionResult> GetAll(string UserId)
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userIdClaim != UserId)
            {
                return Forbid("You are not authorized to View this Customer's Address.");
            }
            var address = await addressRepo.GetAllAddresses(UserId);
            var add = mapper.Map<List<AddressViewDto>>(address);
            return Ok(add);
        }
        [HttpGet("ById")]
        public async Task<IActionResult> GetById(Guid AddressId)
        {
            var address = await addressRepo.GetAddress(AddressId);
            if (address ==null) 
                return NotFound();
            var add = mapper.Map<AddressViewDto>(address);
            return Ok(add);
        }
        [HttpGet("DefaultAddress")]
        public async Task<IActionResult> GetById(string customerId)
        {
            var address = await addressRepo.getDafaultAddress(customerId);
            if (address == null)
                return NotFound();
            var add = mapper.Map<AddressViewDto>(address);
            return Ok(address);
        }
        [HttpPut("makeAddressDefault")]
        public async Task<IActionResult> MakeDefault(Guid AddressId)
        {
            var address = await addressRepo.MakeDefault(AddressId);
            if(address == null)
                return NotFound();
            var add = mapper.Map<AddressViewDto>(address);
            return Ok(add);
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
                var add=await addressRepo.Add(userName, address);
                await addressRepo.Save();
                var addDto = mapper.Map<AddressViewDto>(address);
                return Ok(new
                {
                    Message = "Address added successfully",
                    Data = addDto
                });
            }
            else
            {
                return BadRequest(ModelState);
            }

        }
        [HttpPut]
        public async Task<IActionResult> Update(Guid AddressId, AddressDTO addressdto)
        {
            var address= await addressRepo.GetAddress(AddressId);
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
                   var addDto = mapper.Map<AddressViewDto>(address);
                    return Ok(new
                    {
                        Message = "Address added successfully",
                        Data = addDto
                    });
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
            var address = await addressRepo.GetAddress(AddressId);
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
