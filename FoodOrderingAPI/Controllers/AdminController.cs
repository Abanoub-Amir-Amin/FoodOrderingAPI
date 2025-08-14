using AutoMapper;
using FoodOrderingAPI.DTO;
using FoodOrderingAPI.Models;
using FoodOrderingAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;


[EnableCors("AllowAngularDevClient")]
//[Authorize(Roles = "Admin")]
public class AdminController : Controller
{
    private readonly IAdminService _adminService;
    private readonly IMapper _mapper;
    private readonly SignInManager<User> _signInManager;

    public AdminController(IAdminService adminService, IMapper mapper, SignInManager<User> signInManager)
    {
        _adminService = adminService;
        _mapper = mapper;
        _signInManager = signInManager;
    }


    public async Task<IActionResult> Dashboard()
    {
        var activeRestaurants = await _adminService.GetRestaurantsByActiveStatusAsync(true);
        var inactiveRestaurants = await _adminService.GetRestaurantsByActiveStatusAsync(false);
        var activeDeliveryMen = await _adminService.GetDeliveryMenByAvailabilityStatusAsync(AccountStatusEnum.Active);
        var inactiveDeliveryMen = await _adminService.GetDeliveryMenByAvailabilityStatusAsync(AccountStatusEnum.Pending);
        //var deliveryMen = await _adminService.GetAllDeliveryMenAsync();
        //var customers = await _adminService.GetAllCustomerAsync();
        var customers = await _adminService.GetCustomersOrderSummaryAsync();
        var admins = await _adminService.GetAllAdminsAsync();
        var orders = await _adminService.GetAllOrdersAsync();

        var model = new DashboardDto
        {
            ActiveRestaurants = _mapper.Map<List<RestaurantDto>>(activeRestaurants),
            InactiveRestaurants = _mapper.Map<List<RestaurantDto>>(inactiveRestaurants),
            ActiveDeliveryMen = _mapper.Map<List<DeliveryManDto>>(activeDeliveryMen),
            InactiveDeliveryMen = _mapper.Map<List<DeliveryManDto>>(inactiveDeliveryMen),
            //DeliveryMen = _mapper.Map<List<DeliveryManDto>>(deliveryMen),
            Customers = _mapper.Map<List<CustomerDTO>>(customers),
            Admins = _mapper.Map<List<AdminDto>>(admins),
            Orders = _mapper.Map<List<OrderDto>>(orders),
        };

        model.StatusList = new SelectList(new[]
        {
            new { Value = "", Text = "All" },
            new { Value = "In Process", Text = "In Process" },
            new { Value = "Delivered", Text = "Delivered" },
            new { Value = "Cancelled", Text = "Cancelled" },
        }, "Value", "Text", "");

        return View(model);

    }


    [HttpPost]
    public async Task<IActionResult> ActivateRestaurant(string id)
    {
        try
        {
            await _adminService.ActivateRestaurantAsync(id);
            return RedirectToAction(nameof(Dashboard));
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }


    [HttpPost]
    public async Task<IActionResult> DeactivateRestaurant(string id)
    {
        try
        {
            await _adminService.DeactivateRestaurantAsync(id);
            return RedirectToAction(nameof(Dashboard));
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }


    [HttpPost]
    public async Task<IActionResult> DeleteRestaurant(string id)
    {
        await _adminService.DeleteRestaurantAsync(id);
        return RedirectToAction(nameof(Dashboard));
    }


    [HttpPost]
    public async Task<IActionResult> ActivateDeliveryMen(string id)
    {
        try
        {
            await _adminService.ActivateDeliveryMenAsync(id);
            return RedirectToAction(nameof(Dashboard));
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }


    [HttpPost]
    public async Task<IActionResult> DeactivateDeliveryMen(string id)
    {
        try
        {
            await _adminService.DeactivateDeliveryMenAsync(id);
            return RedirectToAction(nameof(Dashboard));
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }


    [HttpPost]
    public async Task<IActionResult> DeleteDeliveryMan(string id)
    {
        await _adminService.DeleteDeliveryManAsync(id);
        return RedirectToAction(nameof(Dashboard));
    }


    // GET: Show edit form with current admin data
    [HttpGet]
    public async Task<IActionResult> EditAdmin(string userId)
    {
        if (string.IsNullOrEmpty(userId))
            return BadRequest();

        var admin = await _adminService.GetAdminByUserNameAsync(userId);
        if (admin == null)
            return NotFound();

        var model = _mapper.Map<AdminDto>(admin);

        if (model.User == null)
        {
            model.User = new UserDto();
        }

        return View(model);
    }


    // POST: Save admin update changes
    [HttpPost]
    public async Task<IActionResult> EditAdmin(AdminDto model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }
        if (model.User == null || string.IsNullOrEmpty(model.User.UserName))
        {
            ModelState.AddModelError("", "User information is missing.");
            return View(model);
        }
        try
        {
            await _adminService.UpdateAdminAsync(model);
            return RedirectToAction(nameof(Dashboard));
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
        catch (Exception ex)
        {
            ModelState.AddModelError("", "Error updating admin: " + ex.Message);
            return View(model);
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout()
    {
        await _signInManager.SignOutAsync();
        return Ok();
    }

    // GET: /Admin/AdminChat
    public IActionResult AdminChat()
    {
        return View("AdminChat");
    }

    // GET: /Admin/AdminChat
    public IActionResult AiChatBot()
    {
        return View("AiChatBot");
    }


}
