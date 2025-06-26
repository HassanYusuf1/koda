namespace api.Services.Interfaces;

using api.Models.Email;

public interface IEmailService
{
    Task SendAsync(EmailMessage message);
}
