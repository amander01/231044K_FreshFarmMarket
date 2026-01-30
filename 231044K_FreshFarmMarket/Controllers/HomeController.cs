using _231044K_FreshFarmMarket.Models;
using _231044K_FreshFarmMarket.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace _231044K_FreshFarmMarket.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IDataProtector _protector;

        public HomeController(
            UserManager<ApplicationUser> userManager,
            IDataProtectionProvider dataProtectionProvider)
        {
            _userManager = userManager;
            _protector = dataProtectionProvider.CreateProtector("FreshFarmMarket.CreditCard.v1");
        }

        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                // If somehow not signed in, send to Identity login
                return RedirectToPage("/Account/Login", new { area = "Identity" });
            }

            // ✅ Gender display mapping (store short values, display friendly text)
            var genderDisplay = string.Equals(user.Gender, "NA", StringComparison.OrdinalIgnoreCase)
                ? "Prefer not to say"
                : user.Gender;

            // 🔐 Decrypt credit card (if exists)
            string decryptedCard = "";
            if (!string.IsNullOrWhiteSpace(user.EncryptedCreditCard))
            {
                try
                {
                    decryptedCard = _protector.Unprotect(user.EncryptedCreditCard);
                }
                catch
                {
                    decryptedCard = "";
                }
            }

            // 🔒 Mask credit card (never show full)
            string maskedCard = MaskCreditCard(decryptedCard);

            var vm = new HomepageViewModel
            {
                FullName = user.FullName,
                Email = user.Email ?? "",
                Gender = genderDisplay,
                MobileNo = user.MobileNo,
                DeliveryAddress = user.DeliveryAddress,
                MaskedCreditCard = maskedCard,
                PhotoPath = user.PhotoPath,

                // ✅ XSS safe output (encode)
                AboutMeSafe = string.IsNullOrWhiteSpace(user.AboutMe)
                    ? null
                    : WebUtility.HtmlEncode(user.AboutMe)
            };

            return View(vm);
        }

        private string MaskCreditCard(string cc)
        {
            if (string.IsNullOrWhiteSpace(cc))
                return "**** **** **** ****";

            // Keep only digits just in case user entered spaces/dashes
            var digits = new string(cc.Where(char.IsDigit).ToArray());
            if (digits.Length < 4)
                return "**** **** **** ****";

            var last4 = digits[^4..];
            return $"**** **** **** {last4}";
        }
    }
}
