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
                using (var smtpClient = new SmtpClient(_smtpsettings.Host, 465))
                {
                    
                    {
                        smtpClient.Credentials = new System.Net.NetworkCredential(_smtpsettings.Username, _smtpsettings.Password);
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
                    //smtpClient.Send(mailMessage);
                    /*Console.WriteLine(_smtpsettings.Value)*/;
                    await smtpClient.SendMailAsync(mailMessage);
                }
            }
            catch (Exception ex)
            {
                _loggger.LogError(ex, "SMTP error occurred while sending email.");
                throw; // Rethrow so the caller knows it failed.
            }
           



        }
    }
}
