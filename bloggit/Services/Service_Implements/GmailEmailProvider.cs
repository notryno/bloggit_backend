using System.Net.Mail;
using System.Net;
using bloggit.Services.Service_Interfaces;

namespace bloggit.Services.Service_Implements
{
    public class GmailEmailProvider : IGmailEmailProvider
    {
        private readonly string _from;
        private readonly SmtpClient _client;

        public GmailEmailProvider(IConfiguration configuration)
        {
            var userName = configuration.GetSection("GmailCredentials:UserName").Value!;
            var password = configuration.GetSection("GmailCredentials:Password").Value!;

            _from = userName;
            _client = new SmtpClient("smtp.gmail.com", 587)
            {
                Credentials = new NetworkCredential(userName, password),
                UseDefaultCredentials = false,
                EnableSsl = true
            };
        }

        public async Task SendEmailAsync(EmailMessage message)
        {
            var mailMessage = new MailMessage(_from, message.To, message.Subject, message.Body);
            if (message.AttachmentPaths != null)
            {
                foreach (var attachment in message.AttachmentPaths.Select(a => new Attachment(a)))
                {
                    mailMessage.Attachments.Add(attachment);
                }
            }

            await _client.SendMailAsync(mailMessage);
        }
    }
}
