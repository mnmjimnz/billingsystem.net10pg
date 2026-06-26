using BillingSystem.Domain.Entities;

namespace BillingSystem.Domain.Interfaces;

public interface ICategoryRepository
{
    Task<IEnumerable<Category>> GetAllAsync();
    Task<Category?> GetByIdAsync(int id);
    Task<int> AddAsync(Category entity);
    Task<int> UpdateAsync(Category entity);
}
