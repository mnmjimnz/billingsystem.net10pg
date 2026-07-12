using BillingSystem.Domain.Entities;
using BillingSystem.Domain.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using BillingSystem.Application.Interfaces;

namespace BillingSystem.API.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class OrdersController : ControllerBase
{
    private readonly IOrderRepository _repository;
    private readonly IAccountingService _accountingService;

    public OrdersController(IOrderRepository repository, IAccountingService accountingService)
    {
        _repository = repository;
        _accountingService = accountingService;
    }

    [HttpGet]
    public async Task<IActionResult> GetOrders([FromQuery] string search = "", [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
    {
        try
        {
            var result = await _repository.GetOrdersPagedAsync(search, page, pageSize);
            return Ok(result);
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex.Message);
        }
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetOrder(int id)
    {
        var order = await _repository.GetOrderWithDetailsAsync(id);
        if (order == null) return NotFound();
        return Ok(order);
    }

    [HttpPost]
    public async Task<IActionResult> CreateOrder([FromBody] CreateOrderDto dto)
    {
        try
        {
            var orderId = await _repository.AddOrderAsync(dto.Order, dto.Details);
            
            // Calculate total COGS (Costo de ventas)
            decimal totalCogs = 0;
            foreach(var detail in dto.Details) 
            {
                // In a real app we'd fetch the cost from ProductRepository or pass it in DTO.
                // Assuming it's calculated or just left as 0 for now if not available.
            }
            
            // Re-fetch order to get the full properties
            var newOrder = await _repository.GetByIdAsync(orderId);
            if (newOrder != null) 
            {
                await _accountingService.RecordSaleAsync(newOrder, totalCogs);
            }
            
            return Ok(new { Id = orderId });
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex.Message);
        }
    }

    [HttpPut("{id}/status")]
    public async Task<IActionResult> UpdateStatus(int id, [FromBody] UpdateOrderStatusDto dto)
    {
        try
        {
            var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(userIdString, out int userId))
                return Unauthorized();

            var result = await _repository.UpdateOrderStatusAsync(id, dto.Status, userId, dto.ReceiverName);
            if (!result) return NotFound();
            
            return Ok(new { Message = "Status updated successfully" });
        }
        catch (Exception ex)
        {
            return StatusCode(400, ex.Message);
        }
    }
}

public class CreateOrderDto
{
    public Order Order { get; set; } = new();
    public List<OrderDetail> Details { get; set; } = new();
}

public class UpdateOrderStatusDto
{
    public string Status { get; set; } = string.Empty;
    public string? ReceiverName { get; set; }
}
