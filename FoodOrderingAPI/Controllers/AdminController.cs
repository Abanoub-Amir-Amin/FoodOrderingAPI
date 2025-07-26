using AutoMapper;
using FoodOrderingAPI.DTO;
using FoodOrderingAPI.Models;
using FoodOrderingAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

//[Authorize(Roles = "Admin")]
public class AdminController : Controller
{
    private readonly IAdminService _adminService;
    private readonly IMapper _mapper;

    public AdminController(IAdminService adminService, IMapper mapper)
    {
        _adminService = adminService;
        _mapper = mapper;
    }

    public async Task<IActionResult> Dashboard()
    {
        var activeRestaurants = await _adminService.GetRestaurantsByActiveStatusAsync(true);
        var inactiveRestaurants = await _adminService.GetRestaurantsByActiveStatusAsync(false);
        var deliveryMen = await _adminService.GetAllDeliveryMenAsync();
        var customers = await _adminService.GetAllCustomerAsync();

        var model = new DashboardDto
        {
            ActiveRestaurants = _mapper.Map<List<RestaurantDto>>(activeRestaurants),
            InactiveRestaurants = _mapper.Map<List<RestaurantDto>>(inactiveRestaurants),
            DeliveryMen = _mapper.Map<List<DeliveryManDto>>(deliveryMen),
            Customers = _mapper.Map<List<CustomerDTO>>(customers)
        };

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
    public async Task<IActionResult> DeleteDeliveryMan(string id)
    {
        await _adminService.DeleteDeliveryManAsync(id);
        return RedirectToAction(nameof(Dashboard));
    }


}
