using bloggit.Services.Service_Implements;

namespace bloggit.Services.Service_Interfaces
{
    public interface IGmailEmailProvider
    {
        Task SendEmailAsync(EmailMessage emailMessage);
    }
}
