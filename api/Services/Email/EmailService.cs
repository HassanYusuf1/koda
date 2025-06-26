using System.Net;
using System.Net.Mail;
using api.Models.Email;
using api.Services.Interfaces;

namespace api.Services.Email;

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

        var emailConfig = _config.GetSection("Email");
        var smtpServer = emailConfig["SmtpServer"] ?? string.Empty;
        var port = int.TryParse(emailConfig["Port"], out var p) ? p : 25;
        var sender = emailConfig["Sender"] ?? string.Empty;
        var username = emailConfig["Username"] ?? string.Empty;
        var password = emailConfig["Password"] ?? string.Empty;

        using var client = new SmtpClient(smtpServer, port)
        {
            Credentials = new NetworkCredential(username, password),
            EnableSsl = true
        };

        using var mailMessage = new MailMessage(sender, message.To)
        {
            Subject = message.Subject,
            Body = message.Body,
            IsBodyHtml = true
        };

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
