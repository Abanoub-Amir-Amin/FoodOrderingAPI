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
using System.Linq; 
using System.Threading.Tasks;

[EnableCors("AllowAngularDevClient")]
//[Authorize(Roles = "Admin")] // Uncomment if Admin-only access is intended
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

    // Dashboard GET: Load all main admin info filtered by StatusEnum
    public async Task<IActionResult> Dashboard(StatusEnum selectedStatus = StatusEnum.All, string activeTab = "restaurant")
    {
        var activeRestaurants = await _adminService.GetRestaurantsByActiveStatusAsync(true);
        var inactiveRestaurants = await _adminService.GetRestaurantsByActiveStatusAsync(false);
        var activeDeliveryMen = await _adminService.GetDeliveryMenByAvailabilityStatusAsync(AccountStatusEnum.Active);
        var inactiveDeliveryMen = await _adminService.GetDeliveryMenByAvailabilityStatusAsync(AccountStatusEnum.Pending);
        var customers = await _adminService.GetCustomersOrderSummaryAsync();
        var admins = await _adminService.GetAllAdminsAsync();
        var allOrders = await _adminService.GetAllOrdersAsync();

        var filteredOrders = selectedStatus == StatusEnum.All
            ? allOrders
            : allOrders.Where(o => o.Status == selectedStatus);

        var model = new DashboardDto
        {
            ActiveRestaurants = _mapper.Map<List<RestaurantDto>>(activeRestaurants),
            InactiveRestaurants = _mapper.Map<List<RestaurantDto>>(inactiveRestaurants),
            ActiveDeliveryMen = _mapper.Map<List<DeliveryManDto>>(activeDeliveryMen),
            InactiveDeliveryMen = _mapper.Map<List<DeliveryManDto>>(inactiveDeliveryMen),
            Customers = _mapper.Map<List<CustomerDTO>>(customers),
            Admins = _mapper.Map<List<AdminDto>>(admins),
            Orders = _mapper.Map<List<OrderDto>>(filteredOrders),
            SelectedStatus = selectedStatus.ToString()
        };

        ViewData["StatusList"] = new SelectList(
            Enum.GetValues(typeof(FoodOrderingAPI.Models.StatusEnum))
                .Cast<FoodOrderingAPI.Models.StatusEnum>()
                .Select(s => new SelectListItem
                {
                    Value = s.ToString(),
                    Text = s.ToString().Replace('_', ' '),
                    Selected = s.ToString() == model.SelectedStatus
                }),
            "Value", "Text", model.SelectedStatus);

        ViewData["ActiveTab"] = activeTab;

        return View(model);
    }



    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ActivateRestaurant(string id, string activeTab = "restaurant")
    {
        try
        {
            await _adminService.ActivateRestaurantAsync(id);
            return RedirectToAction(nameof(Dashboard), new { activeTab });
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeactivateRestaurant(string id, string activeTab = "restaurant")
    {
        try
        {
            await _adminService.DeactivateRestaurantAsync(id);
            return RedirectToAction(nameof(Dashboard), new { activeTab });
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteRestaurant(string id, string activeTab = "restaurant")
    {
        await _adminService.DeleteRestaurantAsync(id);
        return RedirectToAction(nameof(Dashboard), new { activeTab });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ActivateDeliveryMen(string id, string activeTab = "deliveryman")
    {
        try
        {
            await _adminService.ActivateDeliveryMenAsync(id);
            return RedirectToAction(nameof(Dashboard), new { activeTab });
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeactivateDeliveryMen(string id, string activeTab = "deliveryman")
    {
        try
        {
            await _adminService.DeactivateDeliveryMenAsync(id);
            return RedirectToAction(nameof(Dashboard), new { activeTab });
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteDeliveryMan(string id, string activeTab = "deliveryman")
    {
        try
        {
            await _adminService.DeleteDeliveryManAsync(id);
        }
        catch (Exception ex)
        {
            // Log error here and optionally show a friendly message
            ModelState.AddModelError("", "Delete failed: " + ex.Message);
            // Optionally re-display the same view or redirect with error info
        }
        return RedirectToAction(nameof(Dashboard), new { activeTab });
    }


    // GET: Show edit form with current admin data by UserName
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
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> EditAdmin(AdminDto model, string activeTab = "admin")
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
            return RedirectToAction(nameof(Dashboard), new { activeTab });
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
    [HttpGet]
    public IActionResult AdminChat()
    {
        return View("AdminChat");
    }

    // GET: /Admin/AiChatBot
    [HttpGet]
    public IActionResult AiChatBot()
    {
        return View("AiChatBot");
    }
}

