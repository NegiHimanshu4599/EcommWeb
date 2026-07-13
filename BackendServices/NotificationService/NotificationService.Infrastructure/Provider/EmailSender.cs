using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NotificationService.Application.Interface.Provider;
using NotificationService.Domain.Configuration;
using System.Net;
using System.Net.Mail;

namespace NotificationService.Infrastructure.Provider
{
    public class EmailSender : IEmailSender
    {
        private readonly ILogger<EmailSender> _logger;
        private readonly EmailSettings _emailSettings;
        public EmailSender(ILogger<EmailSender> logger, IOptions<EmailSettings> settings)
        {
            _logger = logger;
            _emailSettings = settings.Value;
        }
        public async Task SendAsync(string to, string subject, string body)
        {
            if (string.IsNullOrWhiteSpace(to))
                throw new ArgumentException("Recipient email is required.", nameof(to));
            if (string.IsNullOrWhiteSpace(subject))
                throw new ArgumentException("Email subject is required.", nameof(subject));
            if (string.IsNullOrWhiteSpace(body))
                throw new ArgumentException("Email body is required.", nameof(body));
            var host = _emailSettings.Host;
            int port = _emailSettings.Port;
            var username = _emailSettings.Username;
            var password = _emailSettings.Password;
            var from = _emailSettings.From;
            if (string.IsNullOrWhiteSpace(host))
                throw new InvalidOperationException("Email host is not configured.");
            if (string.IsNullOrWhiteSpace(username))
                throw new InvalidOperationException("Email username is not configured.");
            if (string.IsNullOrWhiteSpace(password))
                throw new InvalidOperationException("Email password is not configured.");
            if (string.IsNullOrWhiteSpace(from))
                from = username;
            try
            {
                _logger.LogInformation("Sending email to {Recipient} with subject '{Subject}'.", to, subject);
                using var smtpClient = new SmtpClient(host,(int)port)
                {
                    Credentials = new NetworkCredential(username, password),
                    EnableSsl = true,
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                    UseDefaultCredentials = false
                };
                using var mailMessage = new MailMessage
                {
                    From = new MailAddress(from),
                    Subject = subject,
                    Body = body,
                    IsBodyHtml = true
                };
                mailMessage.To.Add(to);
                await smtpClient.SendMailAsync(mailMessage);
                _logger.LogInformation("Email successfully sent to {Recipient}.", to);
            }
            catch (SmtpException ex)
            {
                _logger.LogError(ex, "SMTP error while sending email to {Recipient}.", to);
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error while sending email to {Recipient}.", to);
                throw;
            }
        }
    }
}
