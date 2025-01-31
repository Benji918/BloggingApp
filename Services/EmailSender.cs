using Microsoft.AspNetCore.Identity.UI.Services;
using System.Threading.Tasks;
using System.Net;
using System.Net.Mail;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Logging;
using System.Net.Sockets;


namespace BloggingApp.Services
{
    public class EmailSender : IEmailSender
    {
        private readonly SmtpSettings _smtpsettings;
        private readonly ILogger<EmailSender> _loggger;

        public EmailSender(IOptions<SmtpSettings> smtpSettings, ILogger<EmailSender> loggger)
        {
            _smtpsettings = smtpSettings.Value;
            _loggger = loggger;
        }

        public async Task SendEmailAsync(string email, string subject, string message)
        {
            try
            {
                using (var smtpClient = new SmtpClient(_smtpsettings.Host, _smtpsettings.Port))
                {
                    {
                        smtpClient.Credentials = new NetworkCredential(_smtpsettings.Username, _smtpsettings.Password);
                        smtpClient.UseDefaultCredentials = false;
                        smtpClient.EnableSsl = true;
                    };

                    var mailMessage = new MailMessage
                    {
                        From = new MailAddress(_smtpsettings.FromEmail),
                        Subject = subject,
                        Body = message,
                        IsBodyHtml = true
                    };
                    mailMessage.To.Add(email);
                    //Console.WriteLine(_smtpsettings.Username);
                    await smtpClient.SendMailAsync(mailMessage);
                }
            }
            catch (SmtpException ex)
            {
                _loggger.LogError(ex, "SMTP error occurred while sending email.");
            }
            catch (SocketException ex)
            {
                _loggger.LogError(ex, "Network error occurred while connecting to SMTP server.");
            }
            catch (Exception ex)
            {
                _loggger.LogError(ex, "Error sending email");
            }



        }
    }
}
