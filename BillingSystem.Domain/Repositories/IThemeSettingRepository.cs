using BillingSystem.Domain.Entities;
using System.Threading.Tasks;

namespace BillingSystem.Domain.Repositories;

public interface IThemeSettingRepository
{
    Task<ThemeSetting?> GetByThemeIdAsync(int themeId);
    Task UpdateAsync(ThemeSetting themeSetting);
}
