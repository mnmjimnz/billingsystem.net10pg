using BillingSystem.Domain.Entities;
using BillingSystem.Domain.Repositories;
using Dapper;
using Npgsql;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;

namespace BillingSystem.Infrastructure.Repositories;

public class ThemeSettingRepository : IThemeSettingRepository
{
    private readonly string _connectionString;

    public ThemeSettingRepository(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("DefaultConnection") ?? "";
    }

    public async Task<ThemeSetting?> GetByThemeIdAsync(int themeId)
    {
        using var connection = new NpgsqlConnection(_connectionString);
        return await connection.QueryFirstOrDefaultAsync<ThemeSetting>(
            "SELECT * FROM \"ThemeSettings\" WHERE \"ThemeId\" = @ThemeId", new { ThemeId = themeId });
    }

    public async Task UpdateAsync(ThemeSetting themeSetting)
    {
        using var connection = new NpgsqlConnection(_connectionString);
        await connection.ExecuteAsync(@"
            UPDATE ""ThemeSettings"" SET 
                ""PrimaryColor"" = @PrimaryColor, 
                ""SecondaryColor"" = @SecondaryColor, 
                ""FontFamily"" = @FontFamily,
                ""BorderRadius"" = @BorderRadius,
                ""MainBannerUrl"" = @MainBannerUrl,
                ""LogoUrl"" = @LogoUrl,
                ""ButtonStyle"" = @ButtonStyle,
                ""ProductsPerRow"" = @ProductsPerRow,
                ""ProductCardStyle"" = @ProductCardStyle,
                ""UpdatedAt"" = CURRENT_TIMESTAMP
            WHERE ""ThemeId"" = @ThemeId", themeSetting);
    }
}
