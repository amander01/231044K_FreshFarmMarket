using _231044K_FreshFarmMarket.Models;
using _231044K_FreshFarmMarket.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace _231044K_FreshFarmMarket.Areas.Identity.Pages.Account
{
    [Authorize]
    public class LogoutModel : PageModel
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IAuditLogService _audit;

        public LogoutModel(
            SignInManager<ApplicationUser> signInManager,
            UserManager<ApplicationUser> userManager,
            IAuditLogService audit)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _audit = audit;
        }

        public async Task<IActionResult> OnPost(string? returnUrl = null)
        {
            var ip = HttpContext.Connection.RemoteIpAddress?.ToString();
            var user = await _userManager.GetUserAsync(User);

            // ✅ Audit log
            await _audit.WriteAsync(user?.Id, "LOGOUT", $"Email={user?.Email}", ip);

            // ✅ Proper logout (clears auth cookie)
            await _signInManager.SignOutAsync();

            if (!string.IsNullOrWhiteSpace(returnUrl))
                return LocalRedirect(returnUrl);

            return RedirectToPage("/Account/Login", new { area = "Identity" });
        }
    }
}
