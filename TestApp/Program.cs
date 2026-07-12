using System;
using System.Threading.Tasks;
using Npgsql;
using Dapper;
using System.Collections.Generic;
using System.Linq;

namespace TestApp
{
    class Program
    {
        static async Task Main(string[] args)
        {
            string connString = "Host=dpg-d8vej7p9rddc73c9e6jg-a.oregon-postgres.render.com;Port=5432;Database=billing_system_dtum;Username=admin;Password=3v59bzZTlJeh4Kfc3AohbBcYU63Y6h86";
            using var connection = new NpgsqlConnection(connString);
            await connection.OpenAsync();
            using var transaction = await connection.BeginTransactionAsync();

            try
            {
                int orderId = 4;
                int userId = 1;

                var order = await connection.QueryFirstOrDefaultAsync<dynamic>("SELECT * FROM Orders WHERE Id = @Id", new { Id = orderId }, transaction);
                if (order == null) { Console.WriteLine("Order not found"); return; }
                
                var details = await connection.QueryAsync<dynamic>("SELECT * FROM OrderDetails WHERE OrderId = @Id", new { Id = orderId }, transaction);

                string ticketNumber = $"INV-ORD-{orderId}-{DateTime.Now.Ticks.ToString().Substring(10)}";
                var saleSql = @"
                    INSERT INTO Sales (TicketNumber, Date, CustomerId, UserId, BranchId, Subtotal, Discount, Total, CreatedAt)
                    VALUES (@TicketNumber, CURRENT_TIMESTAMP, @CustomerId, @UserId, @BranchId, @Total, 0, @Total, CURRENT_TIMESTAMP)
                    RETURNING Id;
                ";
                int saleId = await connection.ExecuteScalarAsync<int>(saleSql, new { 
                    TicketNumber = ticketNumber, 
                    CustomerId = order.customerid, 
                    UserId = userId, 
                    BranchId = order.branchid, 
                    Total = order.total 
                }, transaction);

                var saleDetailSql = @"
                    INSERT INTO SaleDetails (SaleId, ProductId, Quantity, UnitPrice, Subtotal, CreatedAt)
                    VALUES (@SaleId, @ProductId, @Quantity, @Price, @Total, CURRENT_TIMESTAMP);
                ";
                foreach (var d in details)
                {
                    await connection.ExecuteAsync(saleDetailSql, new {
                        SaleId = saleId,
                        ProductId = d.productid,
                        Quantity = d.quantity,
                        Price = d.price,
                        Total = d.total
                    }, transaction);
                }

                // Update Order
                var updateSql = @"UPDATE Orders SET Status = @Status, ReceiverName = @ReceiverName, DeliveredAt = @DeliveredAt, UpdatedAt = CURRENT_TIMESTAMP WHERE Id = @Id";
                await connection.ExecuteAsync(updateSql, new { 
                    Status = "DELIVERED", 
                    ReceiverName = "Test", 
                    DeliveredAt = DateTime.Now,
                    Id = orderId 
                }, transaction);

                await transaction.RollbackAsync(); // Rollback so we don't mess up data
                Console.WriteLine("SUCCESS!");
            }
            catch (Exception ex)
            {
                Console.WriteLine("ERROR: " + ex.Message);
            }
        }
    }
}
