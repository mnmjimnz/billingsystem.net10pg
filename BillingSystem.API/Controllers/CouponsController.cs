using Microsoft.AspNetCore.Mvc;
using BillingSystem.Domain.Entities;
using BillingSystem.Domain.Interfaces;
using Microsoft.AspNetCore.Authorization;
using System.Threading.Tasks;

namespace BillingSystem.API.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize(Roles = "1,2,3")]
public class CouponsController : ControllerBase
{
    private readonly ICouponRepository _couponRepository;

    public CouponsController(ICouponRepository couponRepository)
    {
        _couponRepository = couponRepository;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var coupons = await _couponRepository.GetAllAsync();
        return Ok(coupons);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var coupon = await _couponRepository.GetByIdAsync(id);
        if (coupon == null) return NotFound();
        return Ok(coupon);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] Coupon coupon)
    {
        coupon.Code = coupon.Code.ToUpperInvariant();
        
        var existing = await _couponRepository.GetByCodeAsync(coupon.Code);
        if (existing != null)
        {
            return BadRequest(new { message = "Un cupón con ese código ya existe." });
        }
        
        var id = await _couponRepository.AddAsync(coupon);
        coupon.Id = id;
        return CreatedAtAction(nameof(GetById), new { id = coupon.Id }, coupon);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] Coupon coupon)
    {
        if (id != coupon.Id) return BadRequest();
        
        var existing = await _couponRepository.GetByIdAsync(id);
        if (existing == null) return NotFound();
        
        coupon.Code = coupon.Code.ToUpperInvariant();
        await _couponRepository.UpdateAsync(coupon);
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        await _couponRepository.DeleteAsync(id);
        return NoContent();
    }
}
