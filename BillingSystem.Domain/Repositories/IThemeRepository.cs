using BillingSystem.Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BillingSystem.Domain.Repositories;

public interface IThemeRepository
{
    Task<IEnumerable<Theme>> GetAllAsync();
    Task<Theme?> GetByIdAsync(int id);
    Task<Theme?> GetByCodeAsync(string code);
    Task UpdateAsync(Theme theme);
    Task ActivateThemeAsync(int themeId);
}
