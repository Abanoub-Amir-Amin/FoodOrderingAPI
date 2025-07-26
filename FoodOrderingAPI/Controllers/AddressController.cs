using FoodOrderingAPI.DTO;
using FoodOrderingAPI.Interfaces;
using FoodOrderingAPI.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FoodOrderingAPI.Controllers
{
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
        public async Task<IActionResult> Update(Guid AddressId, AddressDTO address)
        {
            if (ModelState.IsValid)
            {
                if (await addressRepo.Update(AddressId, address))
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
