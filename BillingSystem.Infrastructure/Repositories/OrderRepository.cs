using System.Data;
using BillingSystem.Domain.Entities;
using BillingSystem.Domain.Interfaces;
using BillingSystem.Infrastructure.Data;
using Dapper;

namespace BillingSystem.Infrastructure.Repositories;

public class OrderRepository : IOrderRepository
{
    private readonly DbConnectionFactory _db;

    public OrderRepository(DbConnectionFactory db)
    {
        _db = db;
    }

    public async Task<Order?> GetByIdAsync(int id)
    {
        using var connection = _db.CreateConnection();
        return await connection.QueryFirstOrDefaultAsync<Order>("SELECT * FROM Orders WHERE Id = @Id AND IsActive = TRUE", new { Id = id });
    }

    public async Task<IEnumerable<Order>> GetAllAsync()
    {
        using var connection = _db.CreateConnection();
        return await connection.QueryAsync<Order>("SELECT * FROM Orders WHERE IsActive = TRUE");
    }

    public async Task<int> AddAsync(Order entity)
    {
        throw new NotImplementedException("Use AddOrderAsync instead");
    }

    public async Task<int> UpdateAsync(Order entity)
    {
        using var connection = _db.CreateConnection();
        var sql = @"UPDATE Orders SET Status = @Status, ReceiverName = @ReceiverName, DeliveredAt = @DeliveredAt, UpdatedAt = CURRENT_TIMESTAMP WHERE Id = @Id";
        return await connection.ExecuteAsync(sql, entity);
    }

    public Task<BillingSystem.Domain.Models.PagedResult<Order>> GetPagedAsync(string search, int page, int pageSize)
    {
        throw new NotImplementedException();
    }

    public async Task<BillingSystem.Domain.Models.PagedResult<dynamic>> GetOrdersPagedAsync(string search, int page, int pageSize)
    {
        using var connection = _db.CreateConnection();
        int offset = (page - 1) * pageSize;
        int limit = pageSize;
        string searchPattern = $"%{search}%";

        var baseSql = @"FROM Orders o 
                        JOIN Customers c ON o.CustomerId = c.Id
                        JOIN Branches b ON o.BranchId = b.Id
                        WHERE o.IsActive = TRUE";

        if (!string.IsNullOrEmpty(search))
        {
            baseSql += " AND (o.OrderNumber ILIKE @Search OR c.Name ILIKE @Search OR o.Status ILIKE @Search)";
        }
        
        var countSql = $"SELECT COUNT(*) {baseSql}";
        var totalCount = await connection.ExecuteScalarAsync<int>(countSql, new { Search = searchPattern });
        
        var dataSql = $@"SELECT o.*, c.Name as CustomerName, b.Name as BranchName 
                         {baseSql} 
                         ORDER BY o.CreatedAt DESC LIMIT @Limit OFFSET @Offset";
        var items = await connection.QueryAsync<dynamic>(dataSql, new { Search = searchPattern, Limit = limit, Offset = offset });
        
        return new BillingSystem.Domain.Models.PagedResult<dynamic>
        {
            Items = items,
            TotalCount = totalCount,
            Page = page,
            PageSize = pageSize
        };
    }

    public async Task<dynamic?> GetOrderWithDetailsAsync(int id)
    {
        using var connection = _db.CreateConnection();
        var orderSql = @"SELECT o.*, c.Name as CustomerName, b.Name as BranchName 
                         FROM Orders o 
                         JOIN Customers c ON o.CustomerId = c.Id
                         JOIN Branches b ON o.BranchId = b.Id
                         WHERE o.Id = @Id";
        var order = await connection.QueryFirstOrDefaultAsync<dynamic>(orderSql, new { Id = id });

        if (order != null)
        {
            var detailsSql = @"SELECT od.*, pr.Name as ProductName, pr.Barcode as ProductCode 
                               FROM OrderDetails od
                               JOIN Products pr ON od.ProductId = pr.Id
                               WHERE od.OrderId = @Id";
            var details = await connection.QueryAsync<dynamic>(detailsSql, new { Id = id });
            
            return new {
                Order = order,
                Details = details
            };
        }
        return null;
    }

    
    public async Task<IEnumerable<Order>> GetByCustomerIdAsync(int customerId)
    {
        using var connection = _db.CreateConnection();
        var sql = @"
            SELECT * FROM orders WHERE CustomerId = @CustomerId ORDER BY CreatedAt DESC;
        ";
        var orders = await connection.QueryAsync<Order>(sql, new { CustomerId = customerId });
        
        foreach(var order in orders)
        {
            var detailSql = @"
                SELECT od.*, p.Name as ProductName 
                FROM orderdetails od
                JOIN products p ON od.ProductId = p.Id
                WHERE od.OrderId = @OrderId;
            ";
            order.Details = (await connection.QueryAsync<OrderDetail>(detailSql, new { OrderId = order.Id })).ToList();
        }
        return orders;
    }

    public async Task<int> AddOrderAsync(Order order, List<OrderDetail> details)
    {
        using var connection = _db.CreateConnection();
        connection.Open();
        using var transaction = connection.BeginTransaction();

        try
        {
            var orderSql = @"
                INSERT INTO orders (OrderNumber, Date, CustomerId, BranchId, Status, DeliveryAddress, Latitude, Longitude, Notes, Total, PaymentMethod, CreatedAt)
                VALUES (@OrderNumber, @Date, @CustomerId, @BranchId, @Status, @DeliveryAddress, @Latitude, @Longitude, @Notes, @Total, @PaymentMethod, CURRENT_TIMESTAMP)
                RETURNING Id;
            ";
            int orderId = await connection.ExecuteScalarAsync<int>(orderSql, order, transaction);

            var detailSql = @"
                INSERT INTO orderdetails (OrderId, ProductId, Quantity, Price, Total, CreatedAt)
                VALUES (@OrderId, @ProductId, @Quantity, @Price, @Total, CURRENT_TIMESTAMP);
            ";
            
            var stockSql = @"
                UPDATE Products 
                SET Stock = Stock - @Quantity, UpdatedAt = CURRENT_TIMESTAMP 
                WHERE Id = @ProductId;
            ";

            foreach (var detail in details)
            {
                detail.OrderId = orderId;
                await connection.ExecuteAsync(detailSql, detail, transaction);
                // Reserve stock
                await connection.ExecuteAsync(stockSql, new { Quantity = detail.Quantity, ProductId = detail.ProductId }, transaction);
            }

            transaction.Commit();
            return orderId;
        }
        catch
        {
            transaction.Rollback();
            throw;
        }
    }

    public async Task<bool> UpdateOrderStatusAsync(int orderId, string status, int userId, string? receiverName = null)
    {
        using var connection = _db.CreateConnection();
        connection.Open();
        using var transaction = connection.BeginTransaction();

        try
        {
            var order = await connection.QueryFirstOrDefaultAsync<Order>("SELECT * FROM Orders WHERE Id = @Id", new { Id = orderId }, transaction);
            if (order == null) return false;

            if (order.Status == "DELIVERED" || order.Status == "CANCELLED")
                throw new Exception($"Cannot change status of a {order.Status} order.");

            var details = await connection.QueryAsync<OrderDetail>("SELECT * FROM OrderDetails WHERE OrderId = @Id", new { Id = orderId }, transaction);

            if (status == "CANCELLED")
            {
                // Restore stock
                var stockSql = "UPDATE Products SET Stock = Stock + @Quantity WHERE Id = @ProductId";
                foreach (var d in details)
                {
                    await connection.ExecuteAsync(stockSql, new { Quantity = d.Quantity, ProductId = d.ProductId }, transaction);
                }
            }
            else if (status == "DELIVERED")
            {
                // Create Sale (Invoice)
                string ticketNumber = $"INV-ORD-{orderId}-{DateTime.Now.Ticks.ToString().Substring(10)}";
                var saleSql = @"
                    INSERT INTO Sales (TicketNumber, Date, CustomerId, UserId, BranchId, Subtotal, Discount, Total, CreatedAt)
                    VALUES (@TicketNumber, CURRENT_TIMESTAMP, @CustomerId, @UserId, @BranchId, @Total, 0, @Total, CURRENT_TIMESTAMP)
                    RETURNING Id;
                ";
                int saleId = await connection.ExecuteScalarAsync<int>(saleSql, new { 
                    TicketNumber = ticketNumber, 
                    CustomerId = order.CustomerId, 
                    UserId = userId, 
                    BranchId = order.BranchId, 
                    Total = order.Total 
                }, transaction);

                var saleDetailSql = @"
                    INSERT INTO SaleDetails (SaleId, ProductId, Quantity, Price, Discount, Total, CreatedAt)
                    VALUES (@SaleId, @ProductId, @Quantity, @Price, 0, @Total, CURRENT_TIMESTAMP);
                ";
                foreach (var d in details)
                {
                    await connection.ExecuteAsync(saleDetailSql, new {
                        SaleId = saleId,
                        ProductId = d.ProductId,
                        Quantity = d.Quantity,
                        Price = d.Price,
                        Total = d.Total
                    }, transaction);
                }
            }

            // Update Order
            var updateSql = @"UPDATE Orders SET Status = @Status, ReceiverName = @ReceiverName, DeliveredAt = @DeliveredAt, UpdatedAt = CURRENT_TIMESTAMP WHERE Id = @Id";
            await connection.ExecuteAsync(updateSql, new { 
                Status = status, 
                ReceiverName = receiverName, 
                DeliveredAt = status == "DELIVERED" ? (DateTime?)DateTime.Now : null,
                Id = orderId 
            }, transaction);

            transaction.Commit();
            return true;
        }
        catch
        {
            transaction.Rollback();
            throw;
        }
    }
}
