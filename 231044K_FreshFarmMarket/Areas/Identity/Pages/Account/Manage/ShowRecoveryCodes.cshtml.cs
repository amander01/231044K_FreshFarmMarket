using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace _231044K_FreshFarmMarket.Areas.Identity.Pages.Account.Manage
{
    [Authorize]
    public class ShowRecoveryCodesModel : PageModel
    {
        public string[] RecoveryCodes { get; set; } = System.Array.Empty<string>();

        [TempData]
        public string StatusMessage { get; set; } = "";

        public IActionResult OnGet()
        {
            // ✅ Read codes passed from TwoFactorAuthentication handler
            if (TempData["RecoveryCodes"] is string[] codes && codes.Length > 0)
            {
                RecoveryCodes = codes;
                return Page();
            }

            // ✅ Prevent 500 if user opens URL directly
            StatusMessage = "Error: No recovery codes to display. Please generate them again.";
            return RedirectToPage("./TwoFactorAuthentication");
        }
    }
}
