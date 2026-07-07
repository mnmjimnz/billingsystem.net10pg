using Dapper;
using System.Text;
using BillingSystem.Domain.Interfaces;
using BillingSystem.Application.Interfaces;
using BillingSystem.Infrastructure.Data;
using BillingSystem.Infrastructure.Repositories;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        policy => policy.WithOrigins("http://127.0.0.1:5500", "http://localhost:5500", "https://billingsystem-front-js.onrender.com")
                        .AllowAnyMethod()
                        .AllowAnyHeader()
                        .AllowCredentials());
});

// Dependency Injection - Factories
builder.Services.AddSingleton<DbConnectionFactory>();

// Dependency Injection - Repositories
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<ICustomerRepository, CustomerRepository>();
builder.Services.AddScoped<ISupplierRepository, SupplierRepository>();
builder.Services.AddScoped<IPurchaseRepository, PurchaseRepository>();
builder.Services.AddScoped<ISaleRepository, SaleRepository>();
builder.Services.AddScoped<IReportRepository, ReportRepository>();
builder.Services.AddScoped<IKardexRepository, KardexRepository>();
builder.Services.AddScoped<IReceivableRepository, ReceivableRepository>();
builder.Services.AddScoped<INotificationRepository, NotificationRepository>();
builder.Services.AddScoped<IRoleRepository, RoleRepository>();
builder.Services.AddScoped<IBranchRepository, BranchRepository>();
builder.Services.AddScoped<IPayableRepository, PayableRepository>();
builder.Services.AddScoped<ICashRegisterRepository, CashRegisterRepository>();
builder.Services.AddScoped<ISettingsRepository, SettingsRepository>();
builder.Services.AddScoped<IBranchMovementRepository, BranchMovementRepository>();
builder.Services.AddScoped<IStockTransferRepository, StockTransferRepository>();
builder.Services.AddScoped<IOrderRepository, OrderRepository>();

// Dependency Injection - Application Services
builder.Services.AddScoped<BillingSystem.Application.Interfaces.ISaleService, BillingSystem.Application.Services.SaleService>();
builder.Services.AddScoped<BillingSystem.Application.Interfaces.IPurchaseService, BillingSystem.Application.Services.PurchaseService>();
builder.Services.AddScoped<BillingSystem.Application.Interfaces.IKardexService, BillingSystem.Application.Services.KardexService>();
builder.Services.AddScoped<BillingSystem.Application.Interfaces.IReceivableService, BillingSystem.Application.Services.ReceivableService>();
builder.Services.AddScoped<BillingSystem.Application.Interfaces.INotificationService, BillingSystem.API.Services.NotificationService>();
builder.Services.AddScoped<BillingSystem.Application.Interfaces.IReportService, BillingSystem.Application.Services.ReportService>();
builder.Services.AddScoped<BillingSystem.Application.Interfaces.ICashRegisterService, BillingSystem.Application.Services.CashRegisterService>();
builder.Services.AddScoped<BillingSystem.Application.Interfaces.IBranchMovementService, BillingSystem.Application.Services.BranchMovementService>();
builder.Services.AddScoped<BillingSystem.Application.Interfaces.IStockTransferService, BillingSystem.Application.Services.StockTransferService>();

// JWT Authentication
var jwtSettings = builder.Configuration.GetSection("JwtSettings");
var secretKey = jwtSettings["Secret"] ?? "SuperSecretKeyForJwtAuthenticationInBillingSystem!2026";

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtSettings["Issuer"],
            ValidAudience = jwtSettings["Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey))
        };
    });

// SignalR
builder.Services.AddSignalR();

var app = builder.Build();

app.UseCors("AllowAll");
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapGet("/add-permission-transfers", async (DbConnectionFactory factory) => {
    try {
        using var connection = factory.CreateConnection();
        // Insert permission if it doesn't exist
        var sql = @"
            INSERT INTO Permissions (SystemName, DisplayName, Module, Description) 
            SELECT 'MANAGE_TRANSFERS', 'Gestionar Traslados', 'Inventario', 'Permite gestionar traslados de sucursal'
            WHERE NOT EXISTS (SELECT 1 FROM Permissions WHERE SystemName = 'MANAGE_TRANSFERS');
        ";
        await Dapper.SqlMapper.ExecuteAsync(connection, sql);
        return Results.Ok("Permission added");
    } catch (Exception ex) {
        return Results.Problem(ex.ToString());
    }
});

app.MapControllers();
app.MapHub<BillingSystem.API.Hubs.NotificationHub>("/hubs/notifications");


app.MapGet("/migrate-stocks", async (DbConnectionFactory factory) => {
    try {
        using var connection = factory.CreateConnection();
        var sql = @"
            CREATE TABLE IF NOT EXISTS ProductStocks (
                Id SERIAL PRIMARY KEY,
                ProductId INT NOT NULL REFERENCES Products(Id),
                BranchId INT NOT NULL REFERENCES Branches(Id),
                Stock INT NOT NULL DEFAULT 0,
                CreatedAt TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
                UpdatedAt TIMESTAMP NULL,
                UNIQUE(ProductId, BranchId)
            );

            ALTER TABLE InventoryMovements ADD COLUMN IF NOT EXISTS BranchId INT NULL REFERENCES Branches(Id);

            CREATE TABLE IF NOT EXISTS StockTransfers (
                Id SERIAL PRIMARY KEY,
                ProductId INT NOT NULL REFERENCES Products(Id),
                FromBranchId INT NOT NULL REFERENCES Branches(Id),
                ToBranchId INT NOT NULL REFERENCES Branches(Id),
                Quantity INT NOT NULL,
                UserId INT NOT NULL REFERENCES Users(Id),
                CreatedAt TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
                Notes TEXT NULL
            );

            INSERT INTO ProductStocks (ProductId, BranchId, Stock, CreatedAt, UpdatedAt)
            SELECT p.Id, (SELECT MIN(Id) FROM Branches), p.Stock, CURRENT_TIMESTAMP, CURRENT_TIMESTAMP
            FROM Products p
            WHERE NOT EXISTS (
                SELECT 1 FROM ProductStocks ps WHERE ps.ProductId = p.Id
            );

            UPDATE InventoryMovements 
            SET BranchId = (SELECT MIN(Id) FROM Branches) 
            WHERE BranchId IS NULL;
        ";
        await Dapper.SqlMapper.ExecuteAsync(connection, sql);
        return Results.Ok("Migration successful");
    } catch (Exception ex) {
        return Results.Problem(ex.ToString());
    }
});


app.MapGet("/migrate-orders", async (DbConnectionFactory factory) => {
    try {
        using var connection = factory.CreateConnection();
        var sql = @"-- Migration for Orders Module

-- 1. Add Coordinates to Branches
ALTER TABLE Branches ADD COLUMN IF NOT EXISTS Latitude DECIMAL(10, 8);
ALTER TABLE Branches ADD COLUMN IF NOT EXISTS Longitude DECIMAL(11, 8);

-- 2. Create Orders Table
CREATE TABLE IF NOT EXISTS Orders (
    Id SERIAL PRIMARY KEY,
    OrderNumber VARCHAR(50) NOT NULL UNIQUE,
    Date TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    CustomerId INT REFERENCES Customers(Id),
    BranchId INT REFERENCES Branches(Id),
    Status VARCHAR(50) NOT NULL DEFAULT 'PENDING', -- PENDING, IN_TRANSIT, DELIVERED, CANCELLED
    DeliveryAddress VARCHAR(255) NOT NULL,
    Latitude DECIMAL(10, 8) NOT NULL,
    Longitude DECIMAL(11, 8) NOT NULL,
    ReceiverName VARCHAR(100),
    DeliveredAt TIMESTAMP,
    Notes TEXT,
    Total DECIMAL(12,2) NOT NULL DEFAULT 0,
    CreatedAt TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    UpdatedAt TIMESTAMP,
    IsActive BOOLEAN NOT NULL DEFAULT TRUE
);

-- 3. Create OrderDetails Table
CREATE TABLE IF NOT EXISTS OrderDetails (
    Id SERIAL PRIMARY KEY,
    OrderId INT REFERENCES Orders(Id) ON DELETE CASCADE,
    ProductId INT REFERENCES Products(Id),
    Quantity INT NOT NULL,
    Price DECIMAL(12,2) NOT NULL,
    Total DECIMAL(12,2) NOT NULL,
    CreatedAt TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    UpdatedAt TIMESTAMP,
    IsActive BOOLEAN NOT NULL DEFAULT TRUE
);

-- 4. Add Permission
INSERT INTO Permissions (SystemName, DisplayName, Module, Description) 
VALUES ('MANAGE_ORDERS', 'Gestionar Pedidos', 'Ventas', 'Permite crear pedidos, ver rutas y confirmar entregas')
ON CONFLICT (SystemName) DO NOTHING;

-- 5. Assign to Admin Role (RoleId = 1)
INSERT INTO RolePermissions (RoleId, PermissionId)
SELECT 1, Id FROM Permissions WHERE SystemName = 'MANAGE_ORDERS'
ON CONFLICT DO NOTHING;
";
        await Dapper.SqlMapper.ExecuteAsync(connection, sql);
        return Results.Ok("Orders Migration successful");
    } catch (Exception ex) {
        return Results.Problem(ex.ToString());
    }
});

app.Run();
