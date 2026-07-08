using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using BillingSystem.Domain.Entities;
using BillingSystem.Domain.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace BillingSystem.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class StoreAuthController : ControllerBase
{
    private readonly ICustomerRepository _customerRepository;
    private readonly IConfiguration _configuration;

    public StoreAuthController(ICustomerRepository customerRepository, IConfiguration configuration)
    {
        _customerRepository = customerRepository;
        _configuration = configuration;
    }

    public class StoreLoginRequest
    {
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }

    public class StoreRegisterRequest
    {
        public string Name { get; set; } = string.Empty;
        public string DocumentNumber { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public decimal? Latitude { get; set; }
        public decimal? Longitude { get; set; }
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] StoreLoginRequest request)
    {
        var customer = await _customerRepository.GetByUsernameAsync(request.Username);

        if (customer == null || customer.PasswordHash != request.Password)
        {
            return Unauthorized(new { message = "Usuario o contraseña incorrectos" });
        }

        var token = GenerateJwtToken(customer);
        return Ok(new { token, customer = new { customer.Id, customer.Username, customer.Name, customer.Email } });
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] StoreRegisterRequest request)
    {
        var existing = await _customerRepository.GetByUsernameAsync(request.Username);
        if (existing != null)
        {
            return BadRequest(new { message = "El nombre de usuario ya está en uso" });
        }

        var newCustomer = new Customer
        {
            Name = request.Name,
            DocumentNumber = request.DocumentNumber,
            Email = request.Email,
            Phone = request.Phone,
            Address = request.Address,
            Username = request.Username,
            PasswordHash = request.Password,
            Latitude = request.Latitude,
            Longitude = request.Longitude
        };

        var id = await _customerRepository.AddAsync(newCustomer);
        newCustomer.Id = id;

        var token = GenerateJwtToken(newCustomer);
        return Ok(new { token, customer = new { newCustomer.Id, newCustomer.Username, newCustomer.Name, newCustomer.Email } });
    }

    private string GenerateJwtToken(Customer customer)
    {
        var jwtSettings = _configuration.GetSection("JwtSettings");
        var secretKey = jwtSettings["Secret"];
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey!));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, customer.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim("CustomerId", customer.Id.ToString()),
            new Claim(ClaimTypes.Name, customer.Username ?? ""),
            new Claim("IsCustomer", "true")
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
}
