using System.ComponentModel.DataAnnotations;
using _231044K_FreshFarmMarket.Models;
using _231044K_FreshFarmMarket.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Options;

namespace _231044K_FreshFarmMarket.Areas.Identity.Pages.Account
{
    [AllowAnonymous]
    public class LoginModel : PageModel
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IReCaptchaService _captcha;
        private readonly GoogleReCaptchaOptions _captchaOptions;

        public LoginModel(
            SignInManager<ApplicationUser> signInManager,
            UserManager<ApplicationUser> userManager,
            IReCaptchaService captcha,
            IOptions<GoogleReCaptchaOptions> captchaOptions)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _captcha = captcha;
            _captchaOptions = captchaOptions.Value;
        }

        // ✅ Used by Login.cshtml
        public string SiteKey => _captchaOptions.SiteKey ?? "";

        [BindProperty]
        public InputModel Input { get; set; } = new();

        // ✅ Phase 5: posted from hidden input (NOT inside InputModel)
        [BindProperty]
        public string RecaptchaToken { get; set; } = "";

        public string ReturnUrl { get; set; } = "~/";

        public class InputModel
        {
            [Required, EmailAddress]
            public string Email { get; set; } = "";

            [Required, DataType(DataType.Password)]
            public string Password { get; set; } = "";

            public bool RememberMe { get; set; }
        }

        public void OnGet(string? returnUrl = null)
        {
            ReturnUrl = returnUrl ?? "~/";

            // ✅ Redirect signed-in users away from Login
            if (User.Identity != null && User.Identity.IsAuthenticated)
            {
                Response.Redirect(Url.Content("~/"));
                return;
            }
        }

        public async Task<IActionResult> OnPostAsync(string? returnUrl = null)
        {
            ReturnUrl = returnUrl ?? "~/";

            if (!ModelState.IsValid)
                return Page();

            // ✅ Guard: if JS disabled, token missing => show message (NO 500)
            if (string.IsNullOrWhiteSpace(RecaptchaToken))
            {
                ModelState.AddModelError(string.Empty,
                    "Anti-bot verification failed. JavaScript is required to submit this form.");
                return Page();
            }

            // ✅ Verify reCAPTCHA (your VerifyAsync returns bool)
            bool captchaOk;
            try
            {
                captchaOk = await _captcha.VerifyAsync(RecaptchaToken, "login");
            }
            catch
            {
                captchaOk = false;
            }

            if (!captchaOk)
            {
                ModelState.AddModelError(string.Empty,
                    "Anti-bot verification failed. Please refresh the page and try again.");
                return Page();
            }

            // ✅ Find user by email
            var user = await _userManager.FindByEmailAsync(Input.Email);
            if (user == null)
            {
                ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                return Page();
            }

            // ✅ Password sign in
            var result = await _signInManager.PasswordSignInAsync(
                user.UserName!,
                Input.Password,
                Input.RememberMe,
                lockoutOnFailure: true);

            if (result.Succeeded)
                return LocalRedirect(ReturnUrl);

            // ✅ If 2FA enabled => redirect to LoginWith2fa
            if (result.RequiresTwoFactor)
            {
                return RedirectToPage("./LoginWith2fa", new
                {
                    ReturnUrl,
                    RememberMe = Input.RememberMe
                });
            }

            if (result.IsLockedOut)
            {
                ModelState.AddModelError(string.Empty, "Account locked due to too many failed attempts.");
                return Page();
            }

            ModelState.AddModelError(string.Empty, "Invalid login attempt.");
            return Page();
        }
    }
}
