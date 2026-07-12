using System.Data;
using BillingSystem.Domain.Entities;
using BillingSystem.Infrastructure.Data;
using Dapper;

namespace BillingSystem.Infrastructure.Repositories;

public class HrRepository : BillingSystem.Domain.Interfaces.IHrRepository
{
    private readonly DbConnectionFactory _db;

    public HrRepository(DbConnectionFactory db)
    {
        _db = db;
    }

    // Attendance
    public async Task<int> AddAttendanceAsync(Attendance entity)
    {
        using var connection = _db.CreateConnection();
        var sql = @"INSERT INTO Attendances (UserId, Date, CheckInTime, CheckOutTime, Status) 
                    VALUES (@UserId, @Date, @CheckInTime, @CheckOutTime, @Status) RETURNING Id;";
        return await connection.ExecuteScalarAsync<int>(sql, entity);
    }

    public async Task<int> UpdateAttendanceAsync(Attendance entity)
    {
        using var connection = _db.CreateConnection();
        var sql = @"UPDATE Attendances SET CheckInTime = @CheckInTime, CheckOutTime = @CheckOutTime, Status = @Status, UpdatedAt = CURRENT_TIMESTAMP WHERE Id = @Id;";
        return await connection.ExecuteAsync(sql, entity);
    }
    
    public async Task<Attendance?> GetAttendanceByDateAsync(int userId, DateTime date)
    {
        using var connection = _db.CreateConnection();
        return await connection.QueryFirstOrDefaultAsync<Attendance>(
            "SELECT * FROM Attendances WHERE UserId = @UserId AND Date = @Date AND IsActive = TRUE LIMIT 1", 
            new { UserId = userId, Date = date });
    }

    public async Task<IEnumerable<Attendance>> GetAttendancesByPeriodAsync(DateTime startDate, DateTime endDate)
    {
        using var connection = _db.CreateConnection();
        return await connection.QueryAsync<Attendance>(
            "SELECT * FROM Attendances WHERE Date >= @StartDate AND Date <= @EndDate AND IsActive = TRUE ORDER BY Date DESC", 
            new { StartDate = startDate, EndDate = endDate });
    }
    
    // Payroll
    public async Task<int> CreatePayrollRunAsync(PayrollRun entity)
    {
        using var connection = _db.CreateConnection();
        var sql = @"INSERT INTO PayrollRuns (PeriodStart, PeriodEnd, ProcessedDate, Notes) 
                    VALUES (@PeriodStart, @PeriodEnd, @ProcessedDate, @Notes) RETURNING Id;";
        return await connection.ExecuteScalarAsync<int>(sql, entity);
    }
    
    public async Task<int> AddPayrollDetailAsync(PayrollDetail entity)
    {
        using var connection = _db.CreateConnection();
        var sql = @"INSERT INTO PayrollDetails (PayrollRunId, UserId, BaseSalary, ExtraHoursAmount, BonusAmount, DeductionsAmount, NetPay, Observations) 
                    VALUES (@PayrollRunId, @UserId, @BaseSalary, @ExtraHoursAmount, @BonusAmount, @DeductionsAmount, @NetPay, @Observations) RETURNING Id;";
        return await connection.ExecuteScalarAsync<int>(sql, entity);
    }
    
    public async Task<IEnumerable<PayrollRun>> GetPayrollRunsAsync()
    {
        using var connection = _db.CreateConnection();
        return await connection.QueryAsync<PayrollRun>("SELECT * FROM PayrollRuns WHERE IsActive = TRUE ORDER BY PeriodStart DESC");
    }
    
    public async Task<IEnumerable<dynamic>> GetPayrollDetailsAsync(int payrollRunId)
    {
        using var connection = _db.CreateConnection();
        var sql = @"
            SELECT pd.*, u.FullName, u.JobTitle, u.DocumentId 
            FROM PayrollDetails pd
            JOIN Users u ON pd.UserId = u.Id
            WHERE pd.PayrollRunId = @PayrollRunId AND pd.IsActive = TRUE
            ORDER BY u.FullName
        ";
        return await connection.QueryAsync<dynamic>(sql, new { PayrollRunId = payrollRunId });
    }
}
