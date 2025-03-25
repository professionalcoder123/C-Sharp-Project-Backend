using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Logging;

namespace BusinessLayer
{
    public class EmailService : IEmailService
    {
        public bool SendEmail(string toEmail, string subject, string body)
        {
            try
            {
                string fromEmail = "aditya.turwatkar@gmail.com";
                MailMessage message = new MailMessage(fromEmail, toEmail);
                message.Subject = subject;
                message.Body = body;
                message.BodyEncoding = Encoding.UTF8;
                message.IsBodyHtml = true;

                SmtpClient client = new SmtpClient("smtp.gmail.com", 587);
                NetworkCredential credential = new NetworkCredential(fromEmail, "vmod gdkr bvac xbqb");
                client.EnableSsl = true;
                client.UseDefaultCredentials = false;
                client.Credentials = credential;
                client.Send(message);

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Email sending failed: " + ex.Message);
                return false;
            }
        }
    }
}