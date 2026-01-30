using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Configuration;
using System.Net;
using System.Net.Mail;

namespace _231044K_FreshFarmMarket.Services
{
    public class SmtpEmailSender : IEmailSender
    {
        private readonly IConfiguration _config;

        public SmtpEmailSender(IConfiguration config)
        {
            _config = config;
        }

        public async Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            // ✅ Validate TO email
            if (string.IsNullOrWhiteSpace(email))
                throw new ArgumentNullException(nameof(email), "Recipient email cannot be empty.");

            // ✅ Read SMTP settings safely
            var host = _config["Smtp:Host"];
            var portStr = _config["Smtp:Port"];
            var enableSslStr = _config["Smtp:EnableSsl"];

            var userName = _config["Smtp:UserName"];
            var password = _config["Smtp:Password"];

            var fromEmail = _config["Smtp:FromEmail"];
            var fromName = _config["Smtp:FromName"] ?? "FreshFarmMarket";

            // ✅ Validate FROM email (THIS is what caused your crash)
            if (string.IsNullOrWhiteSpace(fromEmail))
                throw new ArgumentNullException("Smtp:FromEmail", "Smtp:FromEmail is missing in appsettings.json.");

            if (string.IsNullOrWhiteSpace(host))
                throw new ArgumentNullException("Smtp:Host", "Smtp:Host is missing in appsettings.json.");

            if (!int.TryParse(portStr, out var port))
                throw new ArgumentNullException("Smtp:Port", "Smtp:Port is missing/invalid in appsettings.json.");

            var enableSsl = true;
            if (!string.IsNullOrWhiteSpace(enableSslStr) && bool.TryParse(enableSslStr, out var parsedSsl))
                enableSsl = parsedSsl;

            using var message = new MailMessage
            {
                From = new MailAddress(fromEmail, fromName),
                Subject = subject ?? "",
                Body = htmlMessage ?? "",
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
    }
}
