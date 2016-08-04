using MailKit.Net.Smtp;
using MimeKit;
using MimeKit.Text;

namespace openiddict_test.Infrastructure
{
    public class AuthMessageSender
    {
        public void SendEmailAsync(string email, string subject, string body)
        {
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress("Joey Tribbiani", "joey@friends.com"));
            message.To.Add(new MailboxAddress("Mrs. Chanandler Bong", "chandler@friends.com"));
            message.Subject = "How you doin'?";

            message.Body = new TextPart(TextFormat.Text)
            {
                Text = @"

Hey Chandler,

    I just wanted to let you know that Monica and I were going to go play some paintball, you in?

-- Joey

"
            };

            using (var client = new SmtpClient())
            {
                client.Connect("smtp.friends.com", 587, false);

                // Note: since we don't have an OAuth2 token, disable
                // the XOAUTH2 authentication mechanism.
                client.AuthenticationMechanisms.Remove("XOAUTH2");

                // Note: only needed if the SMTP server requires authentication
                client.Authenticate("joey", "password");

                client.Send(message);
                client.Disconnect(true);
            }
        }
    }