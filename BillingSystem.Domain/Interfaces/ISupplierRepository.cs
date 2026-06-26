using BillingSystem.Domain.Entities;

namespace BillingSystem.Domain.Interfaces;

public interface ISupplierRepository
{
    Task<Supplier?> GetByIdAsync(int id);
    Task<IEnumerable<Supplier>> GetAllAsync();
    Task<int> AddAsync(Supplier entity);
    Task<int> UpdateAsync(Supplier entity);
}
