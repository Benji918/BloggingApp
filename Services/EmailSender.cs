using Microsoft.AspNetCore.Identity.UI.Services;
using System.Threading.Tasks;
using System.Net;
using System.Net.Mail;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Logging;
using System.Net.Sockets;
using MailKit.Security;
using MimeKit.Text;
using MimeKit;
using Azure.Core;


namespace BloggingApp.Services
{
    public class EmailSender : IEmailSender
    {
        private readonly SmtpSettings _smtpsettings;
        private readonly ILogger<EmailSender> _logger;

        public EmailSender(IOptions<SmtpSettings> smtpSettings, ILogger<EmailSender> loggger)
        {
            _smtpsettings = smtpSettings.Value;
            _logger = loggger;
        }

        public async Task SendEmailAsync(string email, string subject, string message)
        { 

            // Create the email message
            var msg = new MimeMessage();

            // Set the From address using your SMTP settings.
            // You might include a display name if desired.
            msg.From.Add(new MailboxAddress("Your App Name", _smtpsettings.FromEmail));

            // Set the To address using the provided email parameter.
            // Optionally, you can also include a name if you have one.
            msg.To.Add(new MailboxAddress("", email));

            // Set the subject.
            msg.Subject = subject;

            // Set the body as HTML.
            msg.Body = new TextPart(TextFormat.Html)
            {
                Text = message
            };

            try
            {
                // Be sure to use the MailKit SMTP client (not System.Net.Mail.SmtpClient)
                using (var emailClient = new MailKit.Net.Smtp.SmtpClient())
                {
                    emailClient.Timeout = 20000; // 20 seconds

                    _logger.LogInformation("Connecting to SMTP host: {Host} on port: {Port}", _smtpsettings.Host, _smtpsettings.Port);

                    // This callback bypasses certificate validation. Use caution in production.
                    emailClient.ServerCertificateValidationCallback = (s, c, h, e) => true;

                    // Connect to the SMTP server using the settings.
                    await emailClient.ConnectAsync(_smtpsettings.Host, _smtpsettings.Port, SecureSocketOptions.SslOnConnect);

                    // Remove any OAuth functionality if not needed.
                    emailClient.AuthenticationMechanisms.Remove("XOAUTH2");

                    // Authenticate using your SMTP username and password.
                    await emailClient.AuthenticateAsync(_smtpsettings.Username, _smtpsettings.Password);

                    // Send the email.
                    await emailClient.SendAsync(msg);

                    // Disconnect from the SMTP server.
                    await emailClient.DisconnectAsync(true);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "SMTP error occurred while sending email.");
                throw; // Rethrow so that the caller is aware of the failure.
            }





        }
    }
}
