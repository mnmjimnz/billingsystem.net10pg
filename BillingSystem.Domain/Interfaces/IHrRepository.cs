using BillingSystem.Domain.Entities;

namespace BillingSystem.Domain.Interfaces;

public interface IHrRepository
{
    Task<int> AddAttendanceAsync(Attendance entity);
    Task<int> UpdateAttendanceAsync(Attendance entity);
    Task<Attendance?> GetAttendanceByDateAsync(int userId, DateTime date);
    Task<IEnumerable<Attendance>> GetAttendancesByPeriodAsync(DateTime startDate, DateTime endDate);
    
    Task<int> CreatePayrollRunAsync(PayrollRun entity);
    Task<int> AddPayrollDetailAsync(PayrollDetail entity);
    Task<IEnumerable<PayrollRun>> GetPayrollRunsAsync();
    Task<IEnumerable<dynamic>> GetPayrollDetailsAsync(int payrollRunId);
}
