using Microsoft.AspNetCore.Identity.UI.Services;

namespace _231044K_FreshFarmMarket.Services
{
    // DEV ONLY: writes email content to Visual Studio Output window (no SMTP)
    public class DevEmailSender : IEmailSender
    {
        public Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            Console.WriteLine("===== DEV EMAIL SENDER =====");
            Console.WriteLine($"To: {email}");
            Console.WriteLine($"Subject: {subject}");
            Console.WriteLine("Body:");
            Console.WriteLine(htmlMessage);
            Console.WriteLine("============================");
            return Task.CompletedTask;
        }
    }
}
