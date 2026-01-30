using _231044K_FreshFarmMarket.Data;
using _231044K_FreshFarmMarket.Models;

namespace _231044K_FreshFarmMarket.Services
{
    public interface IAuditLogService
    {
        Task WriteAsync(string? userId, string action, string? detail, string? ip);
    }

    public class AuditLogService : IAuditLogService
    {
        private readonly ApplicationDbContext _db;

        public AuditLogService(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task WriteAsync(string? userId, string action, string? detail, string? ip)
        {
            _db.AuditLogs.Add(new AuditLog
            {
                UserId = userId,
                Action = action,
                Detail = detail,
                IpAddress = ip
            });

            await _db.SaveChangesAsync();
        }
    }
}
