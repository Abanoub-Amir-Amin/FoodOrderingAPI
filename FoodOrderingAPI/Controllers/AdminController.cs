using AutoMapper;
using FoodOrderingAPI.DTO;
using FoodOrderingAPI.Models;
using FoodOrderingAPI.Services;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

[EnableCors("AllowAngularDevClient")]
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

    public async Task<IActionResult> Dashboard(StatusEnum selectedStatus = StatusEnum.All, string activeTab = "restaurant")
    {
        var activeRestaurants = await _adminService.GetRestaurantsByActiveStatusAsync(true);
        var inactiveRestaurants = await _adminService.GetRestaurantsByActiveStatusAsync(false);
        var activeDeliveryMen = await _adminService.GetDeliveryMenByAvailabilityStatusAsync(AccountStatusEnum.Active);
        var inactiveDeliveryMen = await _adminService.GetDeliveryMenByAvailabilityStatusAsync(AccountStatusEnum.Pending);
        var customers = await _adminService.GetCustomersOrderSummaryAsync();
        var admins = await _adminService.GetAllAdminsAsync();
        var allOrders = await _adminService.GetAllOrdersAsync();

        var filterStatus = selectedStatus;

        var filteredOrders = filterStatus == StatusEnum.All
            ? allOrders
            : allOrders.Where(o => o.Status == filterStatus);

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

        ViewData["StatusList"] = new SelectList(Enum.GetValues(typeof(StatusEnum))
                                    .Cast<StatusEnum>()
                                    .Select(s => new { Value = s.ToString(), Text = s.ToString().Replace('_', ' ') }),
                                    "Value", "Text", selectedStatus.ToString());

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
        await _adminService.DeleteDeliveryManAsync(id);
        return RedirectToAction(nameof(Dashboard), new { activeTab });
    }



}
