using BillingSystem.Domain.Entities;
using BillingSystem.Domain.Interfaces;
using BillingSystem.Infrastructure.Data;
using Dapper;

namespace BillingSystem.Infrastructure.Repositories;

public class SettingsRepository : ISettingsRepository
{
    private readonly DbConnectionFactory _db;

    public SettingsRepository(DbConnectionFactory db)
    {
        _db = db;
    }

    public async Task<CompanySetting> GetSettingsAsync()
    {
        using var connection = _db.CreateConnection();
        return await connection.QueryFirstOrDefaultAsync<CompanySetting>("SELECT * FROM CompanySettings LIMIT 1");
    }

    public async Task UpdateSettingsAsync(CompanySetting settings)
    {
        using var connection = _db.CreateConnection();
        var exists = await connection.ExecuteScalarAsync<bool>("SELECT EXISTS(SELECT 1 FROM CompanySettings)");
        if (exists)
        {
            var sql = @"
                UPDATE CompanySettings 
                SET CompanyName = @CompanyName,
                    Address = @Address,
                    Phone = @Phone,
                    Email = @Email,
                    TaxPercentage = @TaxPercentage,
                    StoreTheme = @StoreTheme,
                    ShowStoreSlider = @ShowStoreSlider,
                    StoreProductsPerPage = @StoreProductsPerPage,
                    SliderImage1 = @SliderImage1,
                    SliderImage2 = @SliderImage2,
                    SliderImage3 = @SliderImage3,
                    SocialSecurityPercentage = @SocialSecurityPercentage,
                    AfpPercentage = @AfpPercentage,
                    UpdatedAt = CURRENT_TIMESTAMP
                WHERE Id = (SELECT Id FROM CompanySettings LIMIT 1);";
            await connection.ExecuteAsync(sql, settings);
        }
        else
        {
            var sql = @"
                INSERT INTO CompanySettings (
                    CompanyName, Address, Phone, Email, TaxPercentage, 
                    StoreTheme, ShowStoreSlider, StoreProductsPerPage, 
                    SliderImage1, SliderImage2, SliderImage3, 
                    SocialSecurityPercentage, AfpPercentage, UpdatedAt)
                VALUES (
                    @CompanyName, @Address, @Phone, @Email, @TaxPercentage, 
                    @StoreTheme, @ShowStoreSlider, @StoreProductsPerPage, 
                    @SliderImage1, @SliderImage2, @SliderImage3, 
                    @SocialSecurityPercentage, @AfpPercentage, CURRENT_TIMESTAMP);";
            await connection.ExecuteAsync(sql, settings);
        }
    }
}
