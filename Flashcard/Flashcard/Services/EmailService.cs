using MailKit.Net.Smtp;
using MimeKit;
using System.Threading.Tasks;

namespace Flashcard.Services
{
    /// <summary>
    /// This interface belongs to EmailService.
    /// </summary>
    public interface IEmailService
    {
        Task<int> SendEmail(string from, string email, string subject, string body);
    }

    /// <summary>
    /// This service class handles sending emails.
    /// </summary>
    public class EmailService : IEmailService
    {
        public async Task<int> SendEmail(string from, string email, string subject, string body)
        {
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress(from, "projekt.tanulokartya@gmail.com"));
            message.To.Add(new MailboxAddress(email, email));
            message.Subject = subject;
            var bodyMessage = new BodyBuilder
            {
                HtmlBody = body
            };
            message.Body = bodyMessage.ToMessageBody();

            using (var client = new SmtpClient())
            {
                client.Connect("smtp.gmail.com", 587, false);
                client.Authenticate("projekt.tanulokartya@gmail.com", "tanulokartyakq1w2e3");

                client.Send(message);

                client.Disconnect(true);
            }
            return 1;
        }
    }
}
