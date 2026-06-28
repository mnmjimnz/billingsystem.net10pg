using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using BillingSystem.Application.DTOs;
using BillingSystem.Domain.Entities;
using BillingSystem.Domain.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace BillingSystem.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IUserRepository _userRepository;
    private readonly IRoleRepository _roleRepository;
    private readonly IConfiguration _configuration;

    public AuthController(IUserRepository userRepository, IRoleRepository roleRepository, IConfiguration configuration)
    {
        _userRepository = userRepository;
        _roleRepository = roleRepository;
        _configuration = configuration;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        var user = await _userRepository.GetByUsernameAsync(request.Username);

        // Simple password check (in a real app, use BCrypt)
        if (user == null || user.PasswordHash != request.Password)
        {
            return Unauthorized(new { message = "Usuario o contraseña incorrectos" });
        }

        var token = GenerateJwtToken(user);
        var perms = await _roleRepository.GetPermissionsByRoleIdAsync(user.RoleId);
        var permissions = perms.Select(p => p.SystemName).ToList();
        return Ok(new { token, user = new { user.Id, user.Username, user.FullName, user.RoleId, user.BranchId }, permissions });
    }

    private string GenerateJwtToken(User user)
    {
        var jwtSettings = _configuration.GetSection("JwtSettings");
        var secretKey = jwtSettings["Secret"];
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey!));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim("UserId", user.Id.ToString()),
            new Claim(ClaimTypes.Name, user.Username),
            new Claim(ClaimTypes.Role, user.RoleId.ToString()),
            new Claim("BranchId", user.BranchId?.ToString() ?? "0")
        };

        var token = new JwtSecurityToken(
            issuer: jwtSettings["Issuer"],
            audience: jwtSettings["Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(double.Parse(jwtSettings["ExpiryMinutes"]!)),
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    [HttpPost("seed-permissions")]
    public async Task<IActionResult> SeedPermissions([FromServices] BillingSystem.Infrastructure.Data.DbConnectionFactory connectionFactory)
    {
        var sql = @"
            INSERT INTO Permissions (SystemName, DisplayName, Module, Description) VALUES 
            ('VIEW_REPORTS', 'Ver Reportes', 'Reportes', 'Permite acceder a los reportes contables'),
            ('MANAGE_PAYABLES', 'Cuentas por Pagar', 'Pagos', 'Permite gestionar deudas a proveedores'),
            ('MANAGE_BRANCHES', 'Gestionar Sucursales', 'Configuración', 'Permite crear y editar sucursales'),
            ('MANAGE_SETTINGS', 'Configuraciones', 'Configuración', 'Permite modificar los ajustes de la empresa'),
            ('MANAGE_MOVEMENTS', 'Movimientos de Sucursal', 'Inventario', 'Permite trasladar inventario entre sucursales')
            ON CONFLICT (SystemName) DO NOTHING;

            INSERT INTO RolePermissions (RoleId, PermissionId)
            SELECT 1, Id FROM Permissions WHERE SystemName IN ('VIEW_REPORTS', 'MANAGE_PAYABLES', 'MANAGE_BRANCHES', 'MANAGE_SETTINGS', 'MANAGE_MOVEMENTS')
            ON CONFLICT DO NOTHING;
        ";
        using var connection = connectionFactory.CreateConnection();
        await Dapper.SqlMapper.ExecuteAsync(connection, sql);
        return Ok(new { message = "Permissions seeded successfully" });
    }
}
