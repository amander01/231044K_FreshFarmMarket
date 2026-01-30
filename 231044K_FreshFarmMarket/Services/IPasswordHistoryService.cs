using _231044K_FreshFarmMarket.Models;

namespace _231044K_FreshFarmMarket.Services
{
    public interface IPasswordHistoryService
    {
        Task RecordCurrentPasswordHashAsync(ApplicationUser user, int maxHistory = 2);
        Task<bool> IsReusedAsync(ApplicationUser user, string newPassword, int maxHistory = 2);
    }
}
