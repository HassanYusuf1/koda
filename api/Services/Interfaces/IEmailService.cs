namespace api.Services.Interfaces
{
    using api.Models.Email;
    using System.Threading.Tasks;

    public interface IEmailService
    {
        Task SendAsync(EmailMessage message);
    }
}
