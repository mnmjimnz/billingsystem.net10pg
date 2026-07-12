namespace BillingSystem.Infrastructure.Repositories;

using BillingSystem.Domain.Entities;
using BillingSystem.Domain.Interfaces;
using BillingSystem.Domain.Models;
using BillingSystem.Infrastructure.Data;
using Dapper;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;

public class VehicleRepository : IVehicleRepository
{
    private readonly DbConnectionFactory _db;
    public VehicleRepository(DbConnectionFactory db) => _db = db;

    public async Task<Vehicle?> GetByIdAsync(int id)
    {
        using var connection = _db.CreateConnection();
        return await connection.QueryFirstOrDefaultAsync<Vehicle>("SELECT * FROM vehicles WHERE Id = @Id;", new { Id = id });
    }
    public async Task<IEnumerable<Vehicle>> GetAllAsync()
    {
        using var connection = _db.CreateConnection();
        return await connection.QueryAsync<Vehicle>("SELECT * FROM vehicles ORDER BY PlateNumber;");
    }
    public async Task<int> AddAsync(Vehicle entity)
    {
        using var connection = _db.CreateConnection();
        var sql = "INSERT INTO vehicles (PlateNumber, Model, Capacity, IsActive) VALUES (@PlateNumber, @Model, @Capacity, @IsActive) RETURNING Id;";
        return await connection.ExecuteScalarAsync<int>(sql, entity);
    }
    public async Task<int> UpdateAsync(Vehicle entity)
    {
        using var connection = _db.CreateConnection();
        var sql = "UPDATE vehicles SET PlateNumber = @PlateNumber, Model = @Model, Capacity = @Capacity, IsActive = @IsActive WHERE Id = @Id;";
        return await connection.ExecuteAsync(sql, entity);
    }
    public Task<PagedResult<Vehicle>> GetPagedAsync(string search, int page, int pageSize) => throw new System.NotImplementedException();
}

public class DriverRepository : IDriverRepository
{
    private readonly DbConnectionFactory _db;
    public DriverRepository(DbConnectionFactory db) => _db = db;

    public async Task<Driver?> GetByIdAsync(int id)
    {
        using var connection = _db.CreateConnection();
        return await connection.QueryFirstOrDefaultAsync<Driver>("SELECT * FROM drivers WHERE Id = @Id;", new { Id = id });
    }
    public async Task<IEnumerable<Driver>> GetAllAsync()
    {
        using var connection = _db.CreateConnection();
        return await connection.QueryAsync<Driver>("SELECT * FROM drivers ORDER BY Name;");
    }
    public async Task<int> AddAsync(Driver entity)
    {
        using var connection = _db.CreateConnection();
        var sql = "INSERT INTO drivers (Name, LicenseNumber, Phone, IsActive) VALUES (@Name, @LicenseNumber, @Phone, @IsActive) RETURNING Id;";
        return await connection.ExecuteScalarAsync<int>(sql, entity);
    }
    public async Task<int> UpdateAsync(Driver entity)
    {
        using var connection = _db.CreateConnection();
        var sql = "UPDATE drivers SET Name = @Name, LicenseNumber = @LicenseNumber, Phone = @Phone, IsActive = @IsActive WHERE Id = @Id;";
        return await connection.ExecuteAsync(sql, entity);
    }
    public Task<PagedResult<Driver>> GetPagedAsync(string search, int page, int pageSize) => throw new System.NotImplementedException();
}

public class DeliveryRouteRepository : IDeliveryRouteRepository
{
    private readonly DbConnectionFactory _db;
    public DeliveryRouteRepository(DbConnectionFactory db) => _db = db;

    public async Task<DeliveryRoute?> GetByIdAsync(int id)
    {
        using var connection = _db.CreateConnection();
        return await connection.QueryFirstOrDefaultAsync<DeliveryRoute>("SELECT * FROM delivery_routes WHERE Id = @Id;", new { Id = id });
    }
    public async Task<DeliveryRoute?> GetWithDetailsAsync(int id)
    {
        using var connection = _db.CreateConnection();
        var route = await connection.QueryFirstOrDefaultAsync<DeliveryRoute>("SELECT * FROM delivery_routes WHERE Id = @Id;", new { Id = id });
        if (route != null)
        {
            route.Driver = await connection.QueryFirstOrDefaultAsync<Driver>("SELECT * FROM drivers WHERE Id = @Id;", new { Id = route.DriverId });
            route.Vehicle = await connection.QueryFirstOrDefaultAsync<Vehicle>("SELECT * FROM vehicles WHERE Id = @Id;", new { Id = route.VehicleId });
            var stops = await connection.QueryAsync<RouteStop>("SELECT * FROM route_stops WHERE DeliveryRouteId = @Id ORDER BY StopOrder;", new { Id = route.Id });
            route.Stops = stops.ToList();
            
            // Fetch order details for stops
            foreach (var stop in route.Stops)
            {
                stop.Order = await connection.QueryFirstOrDefaultAsync<Order>("SELECT * FROM orders WHERE Id = @Id;", new { Id = stop.OrderId });
            }
        }
        return route;
    }
    public async Task<IEnumerable<DeliveryRoute>> GetAllAsync()
    {
        using var connection = _db.CreateConnection();
        var routes = (await connection.QueryAsync<DeliveryRoute>("SELECT * FROM delivery_routes ORDER BY Date DESC;")).ToList();
        var allStops = (await connection.QueryAsync<RouteStop>("SELECT * FROM route_stops;")).ToList();
        foreach (var route in routes)
        {
            route.Stops = allStops.Where(s => s.DeliveryRouteId == route.Id).OrderBy(s => s.StopOrder).ToList();
        }
        return routes;
    }
    public async Task<IEnumerable<DeliveryRoute>> GetByStatusAsync(string status)
    {
        using var connection = _db.CreateConnection();
        return await connection.QueryAsync<DeliveryRoute>("SELECT * FROM delivery_routes WHERE Status = @Status ORDER BY Date DESC;", new { Status = status });
    }
    public async Task<int> AddAsync(DeliveryRoute entity)
    {
        using var connection = _db.CreateConnection();
        var sql = "INSERT INTO delivery_routes (Date, DriverId, VehicleId, BranchId, Status, CreatedAt) VALUES (@Date, @DriverId, @VehicleId, @BranchId, @Status, CURRENT_TIMESTAMP) RETURNING Id;";
        var id = await connection.ExecuteScalarAsync<int>(sql, entity);
        
        foreach (var stop in entity.Stops)
        {
            stop.DeliveryRouteId = id;
            var stopSql = "INSERT INTO route_stops (DeliveryRouteId, OrderId, StopOrder, Status, EstimatedTime) VALUES (@DeliveryRouteId, @OrderId, @StopOrder, @Status, @EstimatedTime);";
            await connection.ExecuteAsync(stopSql, stop);
            
            // Auto-update order status
            await connection.ExecuteAsync("UPDATE Orders SET Status = 'SHIPPED', UpdatedAt = CURRENT_TIMESTAMP WHERE Id = @OrderId AND Status = 'PENDING';", new { OrderId = stop.OrderId });
        }
        return id;
    }
    public async Task<int> UpdateAsync(DeliveryRoute entity)
    {
        using var connection = _db.CreateConnection();
        var sql = "UPDATE delivery_routes SET Date = @Date, DriverId = @DriverId, VehicleId = @VehicleId, BranchId = @BranchId, Status = @Status WHERE Id = @Id;";
        var res = await connection.ExecuteAsync(sql, entity);
        
        // Revert orders back to PENDING before deleting them from the route, unless they are already DELIVERED
        await connection.ExecuteAsync("UPDATE Orders SET Status = 'PENDING', UpdatedAt = CURRENT_TIMESTAMP WHERE Status != 'DELIVERED' AND Id IN (SELECT OrderId FROM route_stops WHERE DeliveryRouteId = @Id);", new { Id = entity.Id });
        
        // Simplified stop update: Delete all and reinsert (in a real scenario we'd diff them)
        await connection.ExecuteAsync("DELETE FROM route_stops WHERE DeliveryRouteId = @Id;", new { Id = entity.Id });
        foreach (var stop in entity.Stops)
        {
            stop.DeliveryRouteId = entity.Id;
            var stopSql = "INSERT INTO route_stops (DeliveryRouteId, OrderId, StopOrder, Status, EstimatedTime) VALUES (@DeliveryRouteId, @OrderId, @StopOrder, @Status, @EstimatedTime);";
            await connection.ExecuteAsync(stopSql, stop);
            
            // Auto-update order status
            await connection.ExecuteAsync("UPDATE Orders SET Status = 'SHIPPED', UpdatedAt = CURRENT_TIMESTAMP WHERE Id = @OrderId AND Status = 'PENDING';", new { OrderId = stop.OrderId });
        }
        return res;
    }
    public Task<PagedResult<DeliveryRoute>> GetPagedAsync(string search, int page, int pageSize) => throw new System.NotImplementedException();
}
