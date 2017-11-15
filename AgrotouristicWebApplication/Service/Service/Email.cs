using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Service
{
    public abstract class Email
    {
        protected void SendEmail(string userEmail, string subject, string body)
        {
            System.Net.Mail.MailMessage m = new System.Net.Mail.MailMessage(
                         new System.Net.Mail.MailAddress("maciuszka9@gmail.com", "Agroturystyka"),
                         new System.Net.Mail.MailAddress(userEmail));
            m.Subject = subject;
            m.Body = body;
            m.IsBodyHtml = true;
            System.Net.Mail.SmtpClient smtp = new System.Net.Mail.SmtpClient()
            {
                Host = "smtp.gmail.com",
                Port = 587,
                Credentials = new System.Net.NetworkCredential("maciuszka9@gmail.com", "Legolas6#"),
                DeliveryMethod = System.Net.Mail.SmtpDeliveryMethod.Network,
                EnableSsl = true
            };
            smtp.Send(m);
        }
    }
}
