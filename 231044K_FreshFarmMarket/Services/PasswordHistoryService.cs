using _231044K_FreshFarmMarket.Data;
using _231044K_FreshFarmMarket.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace _231044K_FreshFarmMarket.Services
{
    public class PasswordHistoryService : IPasswordHistoryService
    {
        private readonly ApplicationDbContext _db;
        private readonly IPasswordHasher<ApplicationUser> _hasher;

        public PasswordHistoryService(ApplicationDbContext db, IPasswordHasher<ApplicationUser> hasher)
        {
            _db = db;
            _hasher = hasher;
        }

        // ✅ Blocks reuse of CURRENT password + last 2 previous passwords
        public async Task<bool> IsReusedAsync(ApplicationUser user, string newPassword, int maxHistory = 2)
        {
            if (user == null) return false;
            if (string.IsNullOrWhiteSpace(newPassword)) return false;

            // 1) Block CURRENT password reuse
            if (!string.IsNullOrWhiteSpace(user.PasswordHash))
            {
                var currentMatch = _hasher.VerifyHashedPassword(user, user.PasswordHash, newPassword);
                if (currentMatch == PasswordVerificationResult.Success)
                    return true;
            }

            // 2) Block last 2 PREVIOUS password reuse
            var history = await _db.PasswordHistories
                .Where(h => h.UserId == user.Id)
                .OrderByDescending(h => h.CreatedAt)
                .Take(maxHistory) // ✅ last TWO
                .Select(h => h.PasswordHash)
                .ToListAsync();

            foreach (var oldHash in history)
            {
                var match = _hasher.VerifyHashedPassword(user, oldHash, newPassword);
                if (match == PasswordVerificationResult.Success)
                    return true;
            }

            return false;
        }

        // ✅ Records the CURRENT PasswordHash into history (call BEFORE ChangePasswordAsync)
        public async Task RecordCurrentPasswordHashAsync(ApplicationUser user, int maxHistory = 2)
        {
            if (user == null) return;
            if (string.IsNullOrWhiteSpace(user.Id)) return;
            if (string.IsNullOrWhiteSpace(user.PasswordHash)) return;

            // Prevent duplicate rows if user submits twice quickly
            var latest = await _db.PasswordHistories
                .Where(h => h.UserId == user.Id)
                .OrderByDescending(h => h.CreatedAt)
                .FirstOrDefaultAsync();

            if (latest != null && latest.PasswordHash == user.PasswordHash)
                return;

            _db.PasswordHistories.Add(new PasswordHistory
            {
                UserId = user.Id,
                PasswordHash = user.PasswordHash,
                CreatedAt = DateTime.UtcNow
            });

            await _db.SaveChangesAsync();

            // ✅ Trim: keep ONLY last 2 (delete oldest beyond last 2)
            var all = await _db.PasswordHistories
                .Where(h => h.UserId == user.Id)
                .OrderByDescending(h => h.CreatedAt)
                .ToListAsync();

            if (all.Count > maxHistory)
            {
                var toDelete = all.Skip(maxHistory).ToList();
                _db.PasswordHistories.RemoveRange(toDelete);
                await _db.SaveChangesAsync();
            }
        }
    }
}
