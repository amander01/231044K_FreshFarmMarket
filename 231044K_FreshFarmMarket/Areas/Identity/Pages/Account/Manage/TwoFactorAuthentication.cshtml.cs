using _231044K_FreshFarmMarket.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace _231044K_FreshFarmMarket.Areas.Identity.Pages.Account.Manage
{
    [Authorize]
    public class TwoFactorAuthenticationModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ILogger<TwoFactorAuthenticationModel> _logger;

        public TwoFactorAuthenticationModel(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            ILogger<TwoFactorAuthenticationModel> logger)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
        }

        public bool Is2faEnabled { get; set; }
        public int RecoveryCodesLeft { get; set; }

        [TempData]
        public string StatusMessage { get; set; } = "";

        public async Task<IActionResult> OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return NotFound("Unable to load user.");

            Is2faEnabled = await _userManager.GetTwoFactorEnabledAsync(user);
            RecoveryCodesLeft = await _userManager.CountRecoveryCodesAsync(user);

            return Page();
        }

        // ✅ Generates new recovery codes and sends them to ShowRecoveryCodes via TempData
        public async Task<IActionResult> OnPostGenerateRecoveryCodesAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return NotFound("Unable to load user.");

            if (!await _userManager.GetTwoFactorEnabledAsync(user))
            {
                StatusMessage = "Error: You must enable 2FA before generating recovery codes.";
                return RedirectToPage();
            }

            // Generates 10 new recovery codes
            var recoveryCodesEnumerable = await _userManager.GenerateNewTwoFactorRecoveryCodesAsync(user, 10);
            var recoveryCodes = recoveryCodesEnumerable is null ? Array.Empty<string>() : System.Linq.Enumerable.ToArray(recoveryCodesEnumerable);

            // ✅ CRITICAL: Put into TempData so ShowRecoveryCodes page can display it
            TempData["RecoveryCodes"] = recoveryCodes;

            _logger.LogInformation("User with ID '{UserId}' generated new 2FA recovery codes.", await _userManager.GetUserIdAsync(user));

            // Refresh cookie
            await _signInManager.RefreshSignInAsync(user);

            StatusMessage = "New recovery codes generated.";
            return RedirectToPage("./ShowRecoveryCodes");
        }

        public async Task<IActionResult> OnPostDisable2faAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return NotFound("Unable to load user.");

            var disable2faResult = await _userManager.SetTwoFactorEnabledAsync(user, false);
            if (!disable2faResult.Succeeded)
            {
                StatusMessage = "Error: Unexpected error occurred disabling 2FA.";
                return RedirectToPage();
            }

            _logger.LogInformation("User with ID '{UserId}' disabled 2FA.", await _userManager.GetUserIdAsync(user));

            await _signInManager.RefreshSignInAsync(user);

            StatusMessage = "2FA has been disabled.";
            return RedirectToPage();
        }
    }
}
