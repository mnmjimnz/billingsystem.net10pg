using BillingSystem.Domain.Entities;
using BillingSystem.Domain.Models;

namespace BillingSystem.Domain.Interfaces;

public interface IOrderRepository : IRepository<Order>
{
    Task<PagedResult<dynamic>> GetOrdersPagedAsync(string search, int page, int pageSize);
    Task<dynamic?> GetOrderWithDetailsAsync(int id);
    Task<int> AddOrderAsync(Order order, List<OrderDetail> details);
    Task<bool> UpdateOrderStatusAsync(int orderId, string status, int userId, string? receiverName = null);
}
