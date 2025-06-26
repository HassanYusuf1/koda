namespace api.Services
{
    public interface IEmailService
    {
        Task SendPasswordResetEmailAsync(string email, string fullName, string resetToken, string userId);
        Task SendEmailConfirmationAsync(string email, string fullName, string confirmationToken, string userId);
        Task SendWelcomeEmailAsync(string email, string fullName);
    }
}