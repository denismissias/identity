using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using System.Web;

namespace ByteBank.Forum.App_Start.Identity
{
    public class EmailService : IIdentityMessageService
    {
        private readonly string EMAIL_FROM = ConfigurationManager.AppSettings["emailService:email_from"];
        private readonly string EMAIL_PASSWORD = ConfigurationManager.AppSettings["emailService:email_password"];


        public async Task SendAsync(IdentityMessage message)
        {
            using (var mailMessage = new MailMessage())
            {
                mailMessage.To.Add(message.Destination);
                mailMessage.Subject = message.Subject;
                mailMessage.Body = message.Body;
                mailMessage.From = new MailAddress(EMAIL_FROM);

                using (var smtpClient = new SmtpClient())
                {
                    smtpClient.UseDefaultCredentials = true;
                    smtpClient.Credentials = new NetworkCredential(EMAIL_FROM, EMAIL_PASSWORD);

                    smtpClient.DeliveryMethod = SmtpDeliveryMethod.Network;
                    smtpClient.Host = "smtp.gmail.com";
                    smtpClient.Port = 587;
                    smtpClient.EnableSsl = true;

                    smtpClient.Timeout = 20000;

                    await smtpClient.SendMailAsync(mailMessage);
                }
            }
        }
    }
}