using BillingSystem.Domain.Entities;

namespace BillingSystem.Domain.Interfaces;

public interface ISettingsRepository
{
    Task<CompanySetting> GetSettingsAsync();
    Task UpdateSettingsAsync(CompanySetting settings);
}
