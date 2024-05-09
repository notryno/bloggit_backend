using bloggit.Services.Service_Interfaces;

namespace bloggit.Services.Service_Implements
{
    public class EmailService : IEmailService
    {
        private readonly IGmailEmailProvider _emailProvider;
        private readonly string _webAppBaseUrl;

        public EmailService(IGmailEmailProvider emailProvider, IConfiguration configuration)
        {
            _emailProvider = emailProvider;
            _webAppBaseUrl = configuration.GetSection("App:WebAppBaseUrl").Value!;
        }

        public async Task SendForgotPasswordEmailAsync(string firstName, string lastName, string toEmail, string passwordResetToken)
        {
            var passwordRestUrl = $"{_webAppBaseUrl}/reset-password?token={passwordResetToken}";
            var message = new EmailMessage
            {
                Subject = "Password Reset Request",
                To = toEmail,
                Body = @$"Dear {firstName} {lastName},
                        To reset your password, please click on the following link:
                        {passwordRestUrl}"
            };

            await _emailProvider.SendEmailAsync(message);
        }

        public async Task SendEmailConfirmationEmailAsync(string firstName, string lastName, string userId, string email, string token)
        {
            var confirmationLink = $"{_webAppBaseUrl}/confirm-email?token={token}&userId={userId}";
            var message = new EmailMessage
            {
                Subject = "Confirm Your Email",
                To = email,
                Body = @$"Dear {firstName} {lastName},
            Thank you for registering with us! Please click the link below to confirm your email address:
            {confirmationLink}
        "
            };

            await _emailProvider.SendEmailAsync(message);
        }


    }
}
