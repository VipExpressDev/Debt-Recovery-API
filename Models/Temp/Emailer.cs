using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

namespace DebtRecoveryPlatform.Models.Temp
{
    public class Emailer
    {
        public static void SendMail(string senderAddress, string recipientAddress, string subject, string body, string attachmentPath, bool isHtmlBody = false)
        {
            using (MailMessage mail = new MailMessage())
            {
                using (SmtpClient smtp = new SmtpClient("smtpout.secureserver.net",587))
                {
                    var Credentials = new NetworkCredential("webmaster@vip-portfolio.com", "l3L10iILlp3P");
                    if (!string.IsNullOrEmpty(attachmentPath))
                    {
                        Attachment attachment = new Attachment(attachmentPath);
                        mail.Attachments.Add(attachment);
                    }
                    smtp.UseDefaultCredentials = false;
                    smtp.Credentials = Credentials;
                    mail.From = new MailAddress(string.IsNullOrEmpty(senderAddress) ? "provision@oaks.co.za" : senderAddress, "ProVision");
                    mail.To.Add(new MailAddress(recipientAddress));
                    mail.Bcc.Add("pvmonitor@simsgroup.co.za");
                    mail.Subject = subject;
                    mail.Body = body;
                    mail.IsBodyHtml = isHtmlBody;
                    smtp.Send(mail);
                }
            }
        }
    }
}
