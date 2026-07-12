using BillingSystem.Domain.Entities;
using BillingSystem.Domain.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BillingSystem.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class HrController : ControllerBase
{
    private readonly IHrRepository _hrRepo;
    private readonly IUserRepository _userRepo;
    private readonly ISettingsRepository _settingsRepo;

    public HrController(IHrRepository hrRepo, IUserRepository userRepo, ISettingsRepository settingsRepo)
    {
        _hrRepo = hrRepo;
        _userRepo = userRepo;
        _settingsRepo = settingsRepo;
    }

    [HttpGet("users")]
    public async Task<IActionResult> GetHrUsers()
    {
        var users = await _userRepo.GetAllAsync();
        return Ok(users);
    }
    
    [HttpGet("users/{id}")]
    public async Task<IActionResult> GetUserById(int id)
    {
        var user = await _userRepo.GetByIdAsync(id);
        if (user == null) return NotFound();
        return Ok(user);
    }
    
    [HttpPut("users/{id}/hr-details")]
    public async Task<IActionResult> UpdateUserHrDetails(int id, [FromBody] User dto)
    {
        var existing = await _userRepo.GetByIdAsync(id);
        if (existing == null) return NotFound();
        
        existing.Salary = dto.Salary;
        existing.HireDate = dto.HireDate;
        existing.JobTitle = dto.JobTitle;
        existing.DocumentId = dto.DocumentId;
        existing.BaseBonus = dto.BaseBonus;
        
        await _userRepo.UpdateAsync(existing);
        return Ok(existing);
    }

    [HttpPost("attendance/check")]
    public async Task<IActionResult> CheckAttendance([FromBody] dynamic request)
    {
        int userId = request.GetProperty("userId").GetInt32();
        var date = DateTime.UtcNow.Date;
        var existing = await _hrRepo.GetAttendanceByDateAsync(userId, date);
        var timeNow = DateTime.UtcNow.TimeOfDay;

        if (existing == null)
        {
            // Check In
            var attendance = new Attendance
            {
                UserId = userId,
                Date = date,
                CheckInTime = timeNow,
                Status = "Present"
            };
            await _hrRepo.AddAttendanceAsync(attendance);
            return Ok(new { message = "Check-in successful", time = timeNow });
        }
        else if (existing.CheckOutTime == null)
        {
            // Check Out
            existing.CheckOutTime = timeNow;
            await _hrRepo.UpdateAttendanceAsync(existing);
            return Ok(new { message = "Check-out successful", time = timeNow });
        }
        
        return BadRequest(new { message = "Already checked out for today" });
    }

    [HttpGet("attendance")]
    public async Task<IActionResult> GetAttendances([FromQuery] DateTime startDate, [FromQuery] DateTime endDate)
    {
        var records = await _hrRepo.GetAttendancesByPeriodAsync(startDate, endDate);
        return Ok(records);
    }

    [HttpPost("payroll/calculate")]
    public async Task<IActionResult> CalculatePayroll([FromBody] PayrollRun request)
    {
        // 1. Get all active users
        var users = await _userRepo.GetAllAsync();
        
        // 2. Get global settings for deductions
        var settings = await _settingsRepo.GetSettingsAsync();
        decimal igssPercentage = settings.SocialSecurityPercentage / 100m;
        decimal isrPercentage = settings.IncomeTaxPercentage / 100m;
        
        // 3. Create the Run
        request.ProcessedDate = DateTime.UtcNow;
        int runId = await _hrRepo.CreatePayrollRunAsync(request);
        
        // 4. Calculate for each user
        foreach(var user in users)
        {
            decimal baseSalary = user.Salary ?? 0;
            decimal bonus = user.BaseBonus ?? 0;
            decimal extraHours = 0; // In a full implementation we would sum hours from Attendances > 8 hrs
            
            decimal grossPay = baseSalary + extraHours + bonus;
            decimal deductions = (grossPay * igssPercentage) + (grossPay * isrPercentage);
            decimal netPay = grossPay - deductions;
            
            var detail = new PayrollDetail
            {
                PayrollRunId = runId,
                UserId = user.Id,
                BaseSalary = baseSalary,
                ExtraHoursAmount = extraHours,
                BonusAmount = bonus,
                DeductionsAmount = deductions,
                NetPay = netPay
            };
            await _hrRepo.AddPayrollDetailAsync(detail);
        }

        return Ok(new { message = "Payroll calculated successfully", runId = runId });
    }
    
    [HttpGet("payroll/runs")]
    public async Task<IActionResult> GetPayrollRuns()
    {
        var runs = await _hrRepo.GetPayrollRunsAsync();
        return Ok(runs);
    }
    
    [HttpGet("payroll/runs/{id}/details")]
    public async Task<IActionResult> GetPayrollDetails(int id)
    {
        var details = await _hrRepo.GetPayrollDetailsAsync(id);
        return Ok(details);
    }
}
