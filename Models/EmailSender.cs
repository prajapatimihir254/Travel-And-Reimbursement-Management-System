using System.Net;
using System.Net.Mail;

namespace BizTravel.Models
{
    public class EmailSender
    {
        public async Task SendEmailAsync(string toEmail, string subject, string message)
        {
            var mail = "prajapatimihir254@gmail.com";
            var pw = "pmsm uozm htue jkon";

            //smtp client configration
            using (var client = new SmtpClient("smtp.gmail.com",587))
            {
                client.EnableSsl = true;
                client.Credentials = new NetworkCredential(mail, pw);

                var mailMessage = new MailMessage(from: mail, to: toEmail, subject, message)
                {
                    IsBodyHtml = true
                };
                await client.SendMailAsync(mailMessage);
            }
        }
    }
}

