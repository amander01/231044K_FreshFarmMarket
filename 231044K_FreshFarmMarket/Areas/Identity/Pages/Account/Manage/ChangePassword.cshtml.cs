using System.ComponentModel.DataAnnotations;
using _231044K_FreshFarmMarket.Models;
using _231044K_FreshFarmMarket.Security;
using _231044K_FreshFarmMarket.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Options;

namespace _231044K_FreshFarmMarket.Areas.Identity.Pages.Account.Manage
{
    public class ChangePasswordModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IPasswordHistoryService _passwordHistory;
        private readonly PasswordAgeOptions _pwdAge;

        public ChangePasswordModel(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            IPasswordHistoryService passwordHistory,
            IOptions<PasswordAgeOptions> pwdAgeOptions)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _passwordHistory = passwordHistory;
            _pwdAge = pwdAgeOptions.Value;
        }

        [BindProperty]
        public InputModel Input { get; set; } = new();

        [TempData]
        public string? StatusMessage { get; set; }

        public class InputModel
        {
            [Required]
            [DataType(DataType.Password)]
            [Display(Name = "Current password")]
            public string OldPassword { get; set; } = "";

            [Required]
            [DataType(DataType.Password)]
            [Display(Name = "New password")]
            public string NewPassword { get; set; } = "";

            [Required]
            [DataType(DataType.Password)]
            [Display(Name = "Confirm new password")]
            [Compare("NewPassword", ErrorMessage = "The new password and confirmation password do not match.")]
            public string ConfirmPassword { get; set; } = "";
        }

        public async Task<IActionResult> OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return NotFound();
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
                return Page();

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return NotFound();

            // ✅ MIN password age: cannot change within X minutes (now 2 mins via appsettings)
            var minAge = TimeSpan.FromMinutes(_pwdAge.MinAgeMinutes);
            var elapsed = DateTime.UtcNow - user.PasswordLastChangedUtc;

            if (elapsed < minAge)
            {
                var remaining = minAge - elapsed;
                var secs = (int)Math.Ceiling(remaining.TotalSeconds);

                ModelState.AddModelError(string.Empty,
                    $"You must wait {secs} second(s) before changing your password again.");
                return Page();
            }

            // ✅ Verify current password
            var oldOk = await _userManager.CheckPasswordAsync(user, Input.OldPassword);
            if (!oldOk)
            {
                ModelState.AddModelError(string.Empty, "Incorrect password.");
                return Page();
            }

            // ✅ Password history: block reuse of current + last 2
            var reused = await _passwordHistory.IsReusedAsync(user, Input.NewPassword, maxHistory: 2);
            if (reused)
            {
                ModelState.AddModelError("Input.NewPassword",
                    "You cannot reuse your current password or your last 2 passwords.");
                return Page();
            }

            // ✅ Record CURRENT hash into history before changing
            await _passwordHistory.RecordCurrentPasswordHashAsync(user, maxHistory: 2);

            // ✅ Change password
            var result = await _userManager.ChangePasswordAsync(user, Input.OldPassword, Input.NewPassword);
            if (!result.Succeeded)
            {
                foreach (var err in result.Errors)
                    ModelState.AddModelError(string.Empty, err.Description);

                return Page();
            }

            // ✅ Update password age tracking + clear force-change flag
            user.PasswordLastChangedUtc = DateTime.UtcNow;
            user.MustChangePassword = false;
            await _userManager.UpdateAsync(user);

            await _signInManager.RefreshSignInAsync(user);
            StatusMessage = "Your password has been changed.";
            return RedirectToPage();
        }
    }
}
