using BillingSystem.Domain.Entities;

namespace BillingSystem.Domain.Interfaces;

public interface IUserRepository : IRepository<User>
{
    Task<User?> GetByUsernameAsync(string username);
}
