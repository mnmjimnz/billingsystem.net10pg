using BillingSystem.Domain.Entities;
using BillingSystem.Domain.Repositories;
using Dapper;
using Npgsql;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BillingSystem.Infrastructure.Repositories;

public class ThemeRepository : IThemeRepository
{
    private readonly string _connectionString;

    public ThemeRepository(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("DefaultConnection") ?? "";
    }

    public async Task<IEnumerable<Theme>> GetAllAsync()
    {
        using var connection = new NpgsqlConnection(_connectionString);
        return await connection.QueryAsync<Theme>("SELECT * FROM \"Themes\" ORDER BY \"Id\"");
    }

    public async Task<Theme?> GetByIdAsync(int id)
    {
        using var connection = new NpgsqlConnection(_connectionString);
        return await connection.QueryFirstOrDefaultAsync<Theme>(
            "SELECT * FROM \"Themes\" WHERE \"Id\" = @Id", new { Id = id });
    }

    public async Task<Theme?> GetByCodeAsync(string code)
    {
        using var connection = new NpgsqlConnection(_connectionString);
        return await connection.QueryFirstOrDefaultAsync<Theme>(
            "SELECT * FROM \"Themes\" WHERE \"Code\" = @Code", new { Code = code });
    }

    public async Task UpdateAsync(Theme theme)
    {
        using var connection = new NpgsqlConnection(_connectionString);
        await connection.ExecuteAsync(@"
            UPDATE ""Themes"" SET 
                ""Name"" = @Name, 
                ""Description"" = @Description, 
                ""PreviewImage"" = @PreviewImage,
                ""IsActive"" = @IsActive,
                ""UpdatedAt"" = CURRENT_TIMESTAMP
            WHERE ""Id"" = @Id", theme);
    }

    public async Task ActivateThemeAsync(int themeId)
    {
        using var connection = new NpgsqlConnection(_connectionString);
        await connection.ExecuteAsync(@"
            UPDATE ""Themes"" SET ""IsActive"" = false;
            UPDATE ""Themes"" SET ""IsActive"" = true WHERE ""Id"" = @Id;
            UPDATE companysettings SET ""ActiveThemeId"" = @Id;
            UPDATE companysettings SET storetheme = (SELECT ""Code"" FROM ""Themes"" WHERE ""Id"" = @Id);
        ", new { Id = themeId });
    }
}
