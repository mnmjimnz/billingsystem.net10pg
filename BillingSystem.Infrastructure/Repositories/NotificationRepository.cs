using BillingSystem.Domain.Entities;
using BillingSystem.Domain.Interfaces;
using BillingSystem.Infrastructure.Data;
using Dapper;

namespace BillingSystem.Infrastructure.Repositories;

public class NotificationRepository : INotificationRepository
{
    private readonly DbConnectionFactory _db;
    public NotificationRepository(DbConnectionFactory db) => _db = db;

    public async Task<IEnumerable<Notification>> GetAllAsync()
    {
        using var connection = _db.CreateConnection();
        var sql = "SELECT * FROM Notifications ORDER BY CreatedAt DESC LIMIT 100";
        return await connection.QueryAsync<Notification>(sql);
    }

    public async Task<IEnumerable<Notification>> GetUnreadAsync()
    {
        using var connection = _db.CreateConnection();
        var sql = "SELECT * FROM Notifications WHERE IsResolved = FALSE ORDER BY CreatedAt DESC";
        return await connection.QueryAsync<Notification>(sql);
    }

    public async Task MarkAsReadAsync(int id)
    {
        using var connection = _db.CreateConnection();
        var sql = "UPDATE Notifications SET IsResolved = TRUE WHERE Id = @Id";
        await connection.ExecuteAsync(sql, new { Id = id });
    }

    public async Task MarkResolvedAsync(int refId, string type)
    {
        using var connection = _db.CreateConnection();
        var sql = "UPDATE Notifications SET IsResolved = TRUE WHERE ReferenceId = @RefId AND Type = @Type";
        await connection.ExecuteAsync(sql, new { RefId = refId, Type = type });
    }

    public async Task AddAsync(Notification notification)
    {
        using var connection = _db.CreateConnection();
        var sql = @"INSERT INTO Notifications (Title, Message, Type, ReferenceId) 
                    VALUES (@Title, @Message, @Type, @ReferenceId)";
        await connection.ExecuteAsync(sql, notification);
    }
}
