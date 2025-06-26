using System.Net;
using System.Net.Mail;
using System.Text;

namespace api.Services
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<EmailService> _logger;

        public EmailService(IConfiguration configuration, ILogger<EmailService> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }

        public async Task SendPasswordResetEmailAsync(string email, string fullName, string resetToken, string userId)
        {
            var subject = "Tilbakestill passord - NextPlay";
            
            // URL encode the token for safe URL transmission
            var encodedToken = WebUtility.UrlEncode(resetToken);
            var resetUrl = $"{_configuration["Frontend:BaseUrl"]}/reset-password?token={encodedToken}&userId={userId}";

            var body = $@"
                <html>
                <body style='font-family: Arial, sans-serif; line-height: 1.6; color: #333;'>
                    <div style='max-width: 600px; margin: 0 auto; padding: 20px;'>
                        <h2 style='color: #3b82f6;'>Tilbakestill passord</h2>
                        <p>Hei {fullName},</p>
                        <p>Du har bedt om å tilbakestille passordet ditt for NextPlay.</p>
                        <p>Klikk på lenken nedenfor for å tilbakestille passordet ditt:</p>
                        <p style='margin: 30px 0;'>
                            <a href='{resetUrl}' 
                               style='background-color: #3b82f6; color: white; padding: 12px 24px; text-decoration: none; border-radius: 6px; display: inline-block;'>
                               Tilbakestill passord
                            </a>
                        </p>
                        <p><strong>Viktig:</strong> Denne lenken utløper om 1 time av sikkerhetshensyn.</p>
                        <p>Hvis du ikke ba om denne e-posten, kan du ignorere den trygt.</p>
                        <hr style='margin: 30px 0; border: none; border-top: 1px solid #eee;'>
                        <p style='font-size: 12px; color: #666;'>
                            Denne e-posten ble sendt fra NextPlay treningsplattform.<br>
                            Ikke svar på denne e-posten.
                        </p>
                    </div>
                </body>
                </html>";

            await SendEmailAsync(email, subject, body);
        }

        public async Task SendEmailConfirmationAsync(string email, string fullName, string confirmationToken, string userId)
        {
            var subject = "Bekreft e-postadressen din - NextPlay";
            
            var encodedToken = WebUtility.UrlEncode(confirmationToken);
            var confirmationUrl = $"{_configuration["Frontend:BaseUrl"]}/confirm-email?token={encodedToken}&userId={userId}";

            var body = $@"
                <html>
                <body style='font-family: Arial, sans-serif; line-height: 1.6; color: #333;'>
                    <div style='max-width: 600px; margin: 0 auto; padding: 20px;'>
                        <h2 style='color: #10b981;'>Velkommen til NextPlay!</h2>
                        <p>Hei {fullName},</p>
                        <p>Takk for at du registrerte deg hos NextPlay. For å fullføre registreringen din, må du bekrefte e-postadressen din.</p>
                        <p>Klikk på lenken nedenfor for å bekrefte kontoen din:</p>
                        <p style='margin: 30px 0;'>
                            <a href='{confirmationUrl}' 
                               style='background-color: #10b981; color: white; padding: 12px 24px; text-decoration: none; border-radius: 6px; display: inline-block;'>
                               Bekreft e-postadresse
                            </a>
                        </p>
                        <p>Hvis du ikke opprettet en konto hos oss, kan du ignorere denne e-posten.</p>
                        <hr style='margin: 30px 0; border: none; border-top: 1px solid #eee;'>
                        <p style='font-size: 12px; color: #666;'>
                            Denne e-posten ble sendt fra NextPlay treningsplattform.<br>
                            Ikke svar på denne e-posten.
                        </p>
                    </div>
                </body>
                </html>";

            await SendEmailAsync(email, subject, body);
        }

        public async Task SendWelcomeEmailAsync(string email, string fullName)
        {
            var subject = "Velkommen til NextPlay!";
            
            var body = $@"
                <html>
                <body style='font-family: Arial, sans-serif; line-height: 1.6; color: #333;'>
                    <div style='max-width: 600px; margin: 0 auto; padding: 20px;'>
                        <h2 style='color: #3b82f6;'>Velkommen til NextPlay!</h2>
                        <p>Hei {fullName},</p>
                        <p>Kontoen din er nå aktivert og du kan begynne å bruke NextPlay treningsplattform.</p>
                        <p>Her kan du:</p>
                        <ul>
                            <li>Registrere treningsrapporter</li>
                            <li>Se dine treningsøkter</li>
                            <li>Følge med på din progresjon</li>
                            <li>Kommunisere med trenere og lagkamerater</li>
                        </ul>
                        <p>Vi ønsker deg lykke til med treningen!</p>
                        <p style='margin: 30px 0;'>
                            <a href='{_configuration["Frontend:BaseUrl"]}/login' 
                               style='background-color: #3b82f6; color: white; padding: 12px 24px; text-decoration: none; border-radius: 6px; display: inline-block;'>
                               Logg inn
                            </a>
                        </p>
                        <hr style='margin: 30px 0; border: none; border-top: 1px solid #eee;'>
                        <p style='font-size: 12px; color: #666;'>
                            Med vennlig hilsen,<br>
                            NextPlay teamet
                        </p>
                    </div>
                </body>
                </html>";

            await SendEmailAsync(email, subject, body);
        }

        private async Task SendEmailAsync(string email, string subject, string htmlBody)
        {
            try
            {
                var smtpSettings = _configuration.GetSection("Email");
                
                using var client = new SmtpClient(smtpSettings["SmtpServer"])
                {
                    Port = int.Parse(smtpSettings["Port"] ?? "587"),
                    Credentials = new NetworkCredential(smtpSettings["Username"], smtpSettings["Password"]),
                    EnableSsl = bool.Parse(smtpSettings["EnableSsl"] ?? "true")
                };

                var mailMessage = new MailMessage
                {
                    From = new MailAddress(smtpSettings["FromEmail"]!, smtpSettings["FromName"]),
                    Subject = subject,
                    Body = htmlBody,
                    IsBodyHtml = true
                };

                mailMessage.To.Add(email);

                await client.SendMailAsync(mailMessage);
                _logger.LogInformation("Email sent successfully to {Email}", email);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send email to {Email}", email);
                throw; // Re-throw to let the controller handle the error
            }
        }
    }
}