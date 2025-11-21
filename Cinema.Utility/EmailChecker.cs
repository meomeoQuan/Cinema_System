using System;
using System.Net.Mail;
using System.Threading.Tasks;


namespace Cinema.Utility
{
    public class EmailChecker
    {
        public static async Task<bool> CheckEmailAsync(string email)
        {
            try
            {
                var mailAddress = new MailAddress(email);
                using (var smtpClient = new SmtpClient("smtp.your-email-server.com"))
                {
                    smtpClient.Timeout = 10000; // Set timeout to 10 seconds
                    smtpClient.DeliveryMethod = SmtpDeliveryMethod.Network;
                    smtpClient.UseDefaultCredentials = false;
                    smtpClient.Credentials = new System.Net.NetworkCredential("your-email@example.com", "your-email-password");

                    var mailMessage = new MailMessage("your-email@example.com", email)
                    {
                        Subject = "Email Verification",
                        Body = "This is a test email for verification purposes.",
                        IsBodyHtml = false
                    };

                    await smtpClient.SendMailAsync(mailMessage);
                    return true;
                }
            }
            catch (FormatException)
            {
                return false; // Invalid email format
            }
            catch (SmtpException)
            {
                return false; // Email server error or email does not exist
            }
        }
    }
}

