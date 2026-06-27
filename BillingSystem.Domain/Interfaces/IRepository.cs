namespace BillingSystem.Domain.Interfaces;

public interface IRepository<T> where T : class
{
    Task<T?> GetByIdAsync(int id);
    Task<IEnumerable<T>> GetAllAsync();
    Task<int> AddAsync(T entity);
    Task<int> UpdateAsync(T entity);
    Task<BillingSystem.Domain.Models.PagedResult<T>> GetPagedAsync(string search, int page, int pageSize);
}
