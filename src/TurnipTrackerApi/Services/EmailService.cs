using System.Net;
using System.Net.Mail;
using Microsoft.Extensions.Options;
using TurnipTallyApi.Helpers.Settings;

namespace TurnipTallyApi.Services
{
    public interface IEmailService
    {
        void SendEmail(string to, string body, string subject, bool isHtml);
    }

    public class EmailService : IEmailService
    {
        private readonly EmailSettings _emailSettings;

        public EmailService(IOptions<EmailSettings> emailSettings)
        {
            _emailSettings = emailSettings.Value;
        }

        public void SendEmail(string to, string body, string subject, bool isHtml)
        {
            using var msg = new MailMessage();
            msg.To.Add(to);
            msg.From = new MailAddress(_emailSettings.From);
            msg.Subject = subject;
            msg.Body = body;
            msg.IsBodyHtml = isHtml;

            using var client = new SmtpClient(_emailSettings.Smtp.Server)
            {
                Port = _emailSettings.Smtp.Port,
                Credentials = new NetworkCredential(_emailSettings.From, "75GsfptiksHP"),
                EnableSsl = _emailSettings.Smtp.Ssl
            };

            client.Send(msg);
        }
    }
}