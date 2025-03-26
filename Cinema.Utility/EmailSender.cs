using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity.UI.Services;

namespace Cinema.Utility
{
    public class EmailSender : IEmailSender
    {
        public Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            var mail = "DE180924ngoanhquan@gmail.com";
            var pass = "uvjs reiv emzl dlsk"; // (Note: It's recommended to use environment variables for sensitive data like passwords)


            var client = new SmtpClient("smtp.gmail.com", 587) // 587 allows secure connection TLS
            {
                Credentials = new NetworkCredential(mail, pass),
                EnableSsl = true
            };
            // the client and server establish a secure encrypted connection.

            var message = new MailMessage
            {
                From = new MailAddress(mail),//sender
                Subject = subject,
                Body = htmlMessage,
                IsBodyHtml = true // HTML body instead of plain text

            };
            message.To.Add(email); // recipient
            return client.SendMailAsync(message);
        }

     

    }
}
