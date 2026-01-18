using System.Threading.Tasks;

namespace ResturantBusinessLayer.Services.Interfaces
{
    public interface IEmailService
    {
        Task<bool> SendEmailAsync(string to, string subject, string body);
        Task<bool> SendEmailConfirmationAsync(string email, string confirmationLink);
        Task<bool> SendPasswordResetAsync(string email, string resetLink);
    }
}
