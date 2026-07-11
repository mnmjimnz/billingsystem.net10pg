namespace BillingSystem.Domain.Interfaces;
using BillingSystem.Domain.Entities;
using System.Threading.Tasks;
using System.Collections.Generic;

public interface IVehicleRepository : IRepository<Vehicle> {}
public interface IDriverRepository : IRepository<Driver> {}

public interface IDeliveryRouteRepository : IRepository<DeliveryRoute>
{
    Task<DeliveryRoute?> GetWithDetailsAsync(int id);
    Task<IEnumerable<DeliveryRoute>> GetByStatusAsync(string status);
}
