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

// Dependency Injection - Application Services
builder.Services.AddScoped<BillingSystem.Application.Interfaces.ISaleService, BillingSystem.Application.Services.SaleService>();
builder.Services.AddScoped<BillingSystem.Application.Interfaces.IPurchaseService, BillingSystem.Application.Services.PurchaseService>();
builder.Services.AddScoped<BillingSystem.Application.Interfaces.IKardexService, BillingSystem.Application.Services.KardexService>();
builder.Services.AddScoped<BillingSystem.Application.Interfaces.IReceivableService, BillingSystem.Application.Services.ReceivableService>();
builder.Services.AddScoped<BillingSystem.Application.Interfaces.INotificationService, BillingSystem.API.Services.NotificationService>();
builder.Services.AddScoped<BillingSystem.Application.Interfaces.IReportService, BillingSystem.Application.Services.ReportService>();
builder.Services.AddScoped<BillingSystem.Application.Interfaces.ICashRegisterService, BillingSystem.Application.Services.CashRegisterService>();

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
app.MapControllers();
app.MapHub<BillingSystem.API.Hubs.NotificationHub>("/hubs/notifications");

app.Run();
