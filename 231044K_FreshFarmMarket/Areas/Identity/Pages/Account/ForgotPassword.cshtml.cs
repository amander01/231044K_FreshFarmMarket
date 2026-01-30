using _231044K_FreshFarmMarket.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using System.Text.Encodings.Web;

namespace _231044K_FreshFarmMarket.Areas.Identity.Pages.Account
{
    public class ForgotPasswordModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IEmailSender _emailSender;

        public ForgotPasswordModel(
            UserManager<ApplicationUser> userManager,
            IEmailSender emailSender)
        {
            _userManager = userManager;
            _emailSender = emailSender;
        }

        [BindProperty]
        public InputModel Input { get; set; } = new();

        public class InputModel
        {
            [Required, EmailAddress]
            public string Email { get; set; } = string.Empty;
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
                return Page();

            var user = await _userManager.FindByEmailAsync(Input.Email);

            // ✅ SECURITY: never reveal whether user exists
            if (user == null)
                return RedirectToPage("./ForgotPasswordConfirmation");

            if (string.IsNullOrWhiteSpace(user.Email))
                return RedirectToPage("./ForgotPasswordConfirmation");

            // Generate reset token
            var token = await _userManager.GeneratePasswordResetTokenAsync(user);

            // Generate reset link
            var resetLink = Url.Page(
                "/Account/ResetPassword",
                pageHandler: null,
                values: new { area = "Identity", token = token, email = user.Email },
                protocol: Request.Scheme
            );

            if (string.IsNullOrWhiteSpace(resetLink))
                return RedirectToPage("./ForgotPasswordConfirmation");

            try
            {
                await _emailSender.SendEmailAsync(
                    user.Email,
                    "Reset Your Password",
                    $"""
                    <p>You requested a password reset.</p>
                    <p>
                        <a href="{HtmlEncoder.Default.Encode(resetLink)}">
                            Click here to reset your password
                        </a>
                    </p>
                    <p>If you did not request this, please ignore this email.</p>
                    """
                );
            }
            catch (Exception ex)
            {
                // ✅ Log error but DO NOT show user
                Console.WriteLine("ForgotPassword error:");
                Console.WriteLine(ex);
            }

            return RedirectToPage("./ForgotPasswordConfirmation");
        }
    }
}
