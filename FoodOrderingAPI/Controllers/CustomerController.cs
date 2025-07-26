using FoodOrderingAPI.DTO;
using FoodOrderingAPI.Interfaces;
using FoodOrderingAPI.Repository;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace FoodOrderingAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomerController : ControllerBase
    {
        ICustomerRepo customerRepo;
        public CustomerController( ICustomerRepo customerRepo) {
            this.customerRepo = customerRepo;
        }
        [HttpGet("All")]
        public async Task<IActionResult> GetAll()
        {
            List<CustomerDTO> data = await customerRepo.GetAll();
            if (data == null) return BadRequest();
            return Ok(data);
        }
        [HttpGet("ByID/{id}")]
        public async Task<IActionResult> GetById(string id)
        {
            CustomerDTO data = await customerRepo.GetById(id);
            if (data == null) return NotFound();
            return Ok(data);
        }
        [HttpGet("ByEmail/{email}")]

        public async Task<IActionResult> GetByEmail(string email)
        {
            CustomerDTO data = await customerRepo.GetByEmail(email);
            if (data == null) return BadRequest();
            return Ok(data);
        }
        [HttpGet("ByUserName/{Username}")]
        public async Task<IActionResult> GetByUsername(string Username)
        {
            CustomerDTO data = await customerRepo.GetByUsername(Username);
            if (data == null) return BadRequest();
            return Ok(data);
        }
        [HttpPost]
        public async Task<IActionResult> Register(RegisterCustomerDTO customer)
        {
            IdentityResult result = await customerRepo.Register(customer);
            if (result.Succeeded)
            {
                return Ok();
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
        [HttpPut]
        public async Task<IActionResult> Update(string CustomerId,UpdateCustomerDTO customer)
        {
            if (ModelState.IsValid)
            {
                if (await customerRepo.Update(CustomerId, customer) == true)
                {
                    await customerRepo.Save();
                    return Ok();
                }
                else
                    return NotFound();
            }
            else
                return BadRequest(ModelState);
        }
        [HttpDelete]
        public async Task<IActionResult>Delete(string CustomerId)
        {
            if (await customerRepo.Delete(CustomerId))
            {
                await customerRepo.Save();
                return Ok();
            }
            else
                return BadRequest();

        }

    }
}
