using BillingSystem.Domain.Entities;

namespace BillingSystem.Domain.Interfaces;

public interface ICustomerRepository : IRepository<Customer> 
{
    Task<Customer?> GetByUsernameAsync(string username);
}
