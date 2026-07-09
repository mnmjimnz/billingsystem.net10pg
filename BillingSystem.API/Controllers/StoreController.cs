using BillingSystem.Domain.Entities;
using BillingSystem.Domain.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BillingSystem.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class StoreController : ControllerBase
{
    private readonly IProductRepository _productRepository;
    private readonly ICategoryRepository _categoryRepository;
    private readonly IOrderRepository _orderRepository;
    private readonly ICustomerRepository _customerRepository;

    public StoreController(
        IProductRepository productRepository, 
        ICategoryRepository categoryRepository, 
        IOrderRepository orderRepository,
        ICustomerRepository customerRepository)
    {
        _productRepository = productRepository;
        _categoryRepository = categoryRepository;
        _orderRepository = orderRepository;
        _customerRepository = customerRepository;
    }

    [HttpGet("products")]
    public async Task<IActionResult> GetProducts([FromQuery] string search = "", [FromQuery] int categoryId = 0, [FromQuery] int page = 1, [FromQuery] int pageSize = 12)
    {
        // Currently IProductRepository.GetPagedAsync doesn't filter by category directly, but we can do it if needed.
        // For now, let's use GetAllAsync and filter in memory, or if the repository supports it, use that.
        // Wait, does IProductRepository have a category filter? Let's check later. For now, fetch all active.
        
        var allProducts = await _productRepository.GetAllAsync();
        var query = allProducts.Where(p => p.IsActive);
        
        if (!string.IsNullOrWhiteSpace(search))
        {
            query = query.Where(p => p.Name.Contains(search, StringComparison.OrdinalIgnoreCase) || 
                                     p.Description.Contains(search, StringComparison.OrdinalIgnoreCase));
        }

        if (categoryId > 0)
        {
            query = query.Where(p => p.CategoryId == categoryId);
        }

        var totalCount = query.Count();
        var items = query.OrderByDescending(p => p.Id).Skip((page - 1) * pageSize).Take(pageSize).ToList();

        return Ok(new { items, totalCount, page, pageSize });
    }

    [HttpGet("categories")]
    public async Task<IActionResult> GetCategories()
    {
        var cats = await _categoryRepository.GetAllAsync();
        return Ok(cats.Where(c => c.IsActive).OrderBy(c => c.Name));
    }

    public class StoreCheckoutRequest
    {
        public List<StoreCartItem> Items { get; set; } = new();
        public string DeliveryAddress { get; set; } = string.Empty;
        public decimal Latitude { get; set; }
        public decimal Longitude { get; set; }
        public string ReceiverName { get; set; } = string.Empty;
        public string Notes { get; set; } = string.Empty;
        public string PaymentMethod { get; set; } = "EFECTIVO";
    }

    public class StoreCartItem
    {
        public int ProductId { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
    }

    [Authorize]
    [HttpPost("checkout")]
    public async Task<IActionResult> Checkout([FromBody] StoreCheckoutRequest request)
    {
        var customerIdClaim = User.FindFirst("CustomerId")?.Value;
        if (string.IsNullOrEmpty(customerIdClaim) || !int.TryParse(customerIdClaim, out int customerId))
        {
            return Unauthorized(new { message = "Token inválido o expirado." });
        }

        if (request.Items == null || !request.Items.Any())
        {
            return BadRequest(new { message = "El carrito está vacío." });
        }

        var order = new Order
        {
            OrderNumber = $"ONL-{DateTime.Now:yyyyMMddHHmmss}-{new Random().Next(100, 999)}",
            Date = DateTime.Now,
            CustomerId = customerId,
            BranchId = 1, // Default branch for online orders
            Status = "PENDING",
            DeliveryAddress = request.DeliveryAddress,
            Latitude = request.Latitude,
            Longitude = request.Longitude,
            ReceiverName = request.ReceiverName,
            Notes = request.Notes,
            PaymentMethod = request.PaymentMethod,
            Total = request.Items.Sum(i => i.Price * i.Quantity)
        };

        foreach (var item in request.Items)
        {
            order.Details.Add(new OrderDetail
            {
                ProductId = item.ProductId,
                Quantity = item.Quantity,
                Price = item.Price,
                Total = item.Quantity * item.Price
            });
        }

        var orderId = await _orderRepository.AddOrderAsync(order, order.Details);
        
        // Optionally update customer's last known location
        var customer = await _customerRepository.GetByIdAsync(customerId);
        if (customer != null)
        {
            customer.Latitude = request.Latitude;
            customer.Longitude = request.Longitude;
            customer.Address = request.DeliveryAddress;
            await _customerRepository.UpdateAsync(customer);
        }

        return Ok(new { message = "Pedido realizado con éxito", orderId = orderId });
    }
}
