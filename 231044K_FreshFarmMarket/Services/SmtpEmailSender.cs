using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Net;
using System.Net.Mail;

namespace _231044K_FreshFarmMarket.Services
{
    public class SmtpEmailSender : IEmailSender
    {
        private readonly IConfiguration _config;
        private readonly ILogger<SmtpEmailSender> _logger;

        public SmtpEmailSender(IConfiguration config, ILogger<SmtpEmailSender> logger)
        {
            _config = config;
            _logger = logger;
        }

        public async Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            if (string.IsNullOrWhiteSpace(email))
                throw new ArgumentNullException(nameof(email), "Recipient email cannot be empty.");

            var host = _config["Smtp:Host"];
            var portStr = _config["Smtp:Port"];
            var enableSslStr = _config["Smtp:EnableSsl"];
            var userName = _config["Smtp:UserName"];
            var password = _config["Smtp:Password"];
            var fromEmail = _config["Smtp:FromEmail"];
            var fromName = _config["Smtp:FromName"] ?? "FreshFarmMarket";

            if (string.IsNullOrWhiteSpace(fromEmail))
                throw new ArgumentNullException("Smtp:FromEmail", "Smtp:FromEmail is missing in appsettings.json.");

            if (string.IsNullOrWhiteSpace(host))
                throw new ArgumentNullException("Smtp:Host", "Smtp:Host is missing in appsettings.json.");

            if (!int.TryParse(portStr, out var port))
                throw new ArgumentNullException("Smtp:Port", "Smtp:Port is missing/invalid in appsettings.json.");

            var enableSsl = true;
            if (!string.IsNullOrWhiteSpace(enableSslStr) && bool.TryParse(enableSslStr, out var parsedSsl))
                enableSsl = parsedSsl;

            try
            {
                using var message = new MailMessage
                {
                    From = new MailAddress(fromEmail, fromName),
                    Subject = subject ?? string.Empty,
                    Body = htmlMessage ?? string.Empty,
                    IsBodyHtml = true
                };

                message.To.Add(new MailAddress(email));

                using var client = new SmtpClient(host, port)
                {
                    EnableSsl = enableSsl,
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                    UseDefaultCredentials = false,
                    Credentials = new NetworkCredential(userName, password)
                };

                await client.SendMailAsync(message);
            }
            catch (Exception ex)
            {
                // ✅ Log technical details (do NOT expose to user)
                _logger.LogError(ex, "Failed to send email to {Recipient}. Subject: {Subject}", email, subject);

                // ✅ Generic message only
                throw new InvalidOperationException("Unable to send email at the moment. Please try again later.");
            }
        }
    }
}
