namespace BillingSystem.API.Controllers;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using BillingSystem.Domain.Entities;
using BillingSystem.Domain.Interfaces;
using System.Threading.Tasks;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Admin,Manager,Dispatcher")]
public class DeliveryController : ControllerBase
{
    private readonly IVehicleRepository _vehicleRepo;
    private readonly IDriverRepository _driverRepo;
    private readonly IDeliveryRouteRepository _routeRepo;

    public DeliveryController(
        IVehicleRepository vehicleRepo, 
        IDriverRepository driverRepo, 
        IDeliveryRouteRepository routeRepo)
    {
        _vehicleRepo = vehicleRepo;
        _driverRepo = driverRepo;
        _routeRepo = routeRepo;
    }

    [HttpGet("vehicles")]
    public async Task<IActionResult> GetVehicles()
    {
        var data = await _vehicleRepo.GetAllAsync();
        return Ok(data);
    }

    [HttpPost("vehicles")]
    public async Task<IActionResult> CreateVehicle(Vehicle v)
    {
        var id = await _vehicleRepo.AddAsync(v);
        return Ok(new { id });
    }
    
    [HttpPut("vehicles/{id}")]
    public async Task<IActionResult> UpdateVehicle(int id, Vehicle v)
    {
        v.Id = id;
        await _vehicleRepo.UpdateAsync(v);
        return Ok();
    }

    [HttpGet("drivers")]
    public async Task<IActionResult> GetDrivers()
    {
        var data = await _driverRepo.GetAllAsync();
        return Ok(data);
    }

    [HttpPost("drivers")]
    public async Task<IActionResult> CreateDriver(Driver d)
    {
        var id = await _driverRepo.AddAsync(d);
        return Ok(new { id });
    }
    
    [HttpPut("drivers/{id}")]
    public async Task<IActionResult> UpdateDriver(int id, Driver d)
    {
        d.Id = id;
        await _driverRepo.UpdateAsync(d);
        return Ok();
    }

    [HttpGet("routes")]
    public async Task<IActionResult> GetRoutes()
    {
        var data = await _routeRepo.GetAllAsync();
        return Ok(data);
    }

    [HttpGet("routes/{id}")]
    public async Task<IActionResult> GetRoute(int id)
    {
        var data = await _routeRepo.GetWithDetailsAsync(id);
        if (data == null) return NotFound();
        return Ok(data);
    }

    [HttpPost("routes")]
    public async Task<IActionResult> CreateRoute(DeliveryRoute r)
    {
        var id = await _routeRepo.AddAsync(r);
        return Ok(new { id });
    }

    [HttpPut("routes/{id}")]
    public async Task<IActionResult> UpdateRoute(int id, DeliveryRoute r)
    {
        r.Id = id;
        await _routeRepo.UpdateAsync(r);
        return Ok();
    }
}
