using api.Models.Email;
using api.Services.Interfaces;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Hosting;
using System.Net;
using System.Net.Mail;

namespace api.Services.Email
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _config;
        private readonly ILogger<EmailService> _logger;
        private readonly IWebHostEnvironment _env;

        public EmailService(IConfiguration config, ILogger<EmailService> logger, IWebHostEnvironment env)
        {
            _config = config;
            _logger = logger;
            _env = env;
        }

        public async Task SendAsync(EmailMessage message)
        {
            if (_env.IsDevelopment())
            {
                _logger.LogInformation("Sending email (mock) to {To}: {Subject}\n{Body}", message.To, message.Subject, message.Body);
                return;
            }

            var emailConfig = _config.GetSection("EmailSettings");
            var smtpServer = emailConfig["SmtpServer"] ?? string.Empty;
            var port = int.TryParse(emailConfig["SmtpPort"], out var p) ? p : 587;
            var senderName = emailConfig["SenderName"] ?? string.Empty;
            var senderEmail = emailConfig["SenderEmail"] ?? string.Empty;
            var password = emailConfig["SenderPassword"] ?? string.Empty;

            using var client = new SmtpClient(smtpServer, port)
            {
                Credentials = new NetworkCredential(senderEmail, password),
                EnableSsl = true
            };

            using var mailMessage = new MailMessage()
            {
                From = new MailAddress(senderEmail, senderName),
                Subject = message.Subject,
                Body = message.Body,
                IsBodyHtml = true
            };
            mailMessage.To.Add(message.To);

            try
            {
                await client.SendMailAsync(mailMessage);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send email to {To}", message.To);
                throw;
            }
        }
    }
}
