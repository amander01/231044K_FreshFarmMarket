using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace _231044K_FreshFarmMarket.Controllers
{
    [AllowAnonymous]
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public class ErrorController : Controller
    {
        // Handles status codes like 404, 403, etc
        [Route("Error/{statusCode:int}")]
        public IActionResult Status(int statusCode)
        {
            ViewData["StatusCode"] = statusCode;
            return View("Status");
        }

        // Handles unhandled exceptions (500)
        [Route("Error/500")]
        public IActionResult Error500()
        {
            // You can log details if needed (avoid showing stack traces to user)
            var feature = HttpContext.Features.Get<IExceptionHandlerFeature>();
            ViewData["Message"] = "Something went wrong. Please try again later.";

            // Optional: you can store feature?.Error.Message in logs only
            return View("Status");
        }

        // ✅ For testing 500 easily (Phase 6 proof)
        [Route("Error/Test500")]
        public IActionResult Test500()
        {
            throw new Exception("Intentional test exception for Phase 6 (500).");
        }
    }
}
