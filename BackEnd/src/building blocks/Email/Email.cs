using Microsoft.Extensions.Options;
using System;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

namespace Email
{
    public class Email
    {
        public string de { get; set; }
        public string para { get; set; }
        public string assunto { get; set; }
        public string corpo { get; set; }

        public Email()
        {

        }
    }

    public class EmailSender : IEmailSender
    {
        public EmailSetting _emailSettings { get; }

        public EmailSender(IOptions<EmailSetting> emailSettings)
        {
            _emailSettings = emailSettings.Value;
        }

        public Task SendEmailAsync(string email, string subject, string message)
        {
            try
            {
                Execute(email, subject, message).Wait();
                return Task.FromResult(0);
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        private async Task Execute(string email, string subject, string message)
        {
            try
            {
                string toEmail = string.IsNullOrEmpty(email) ? _emailSettings.ToEmail : email;

                MailMessage mail = new MailMessage()
                {
                    From = new MailAddress(_emailSettings.UsernameEmail, "Sinaf")
                };

                mail.To.Add(new MailAddress(toEmail));
                //    mail.CC.Add(new MailAddress(_emailSettings.CcEmail));

                mail.Subject = subject;
                mail.Body = message;
                mail.IsBodyHtml = true;
                mail.Priority = MailPriority.High;

                //outras opções
                //mail.Attachments.Add(new Attachment(arquivo));
                //

                if (_emailSettings.PrimaryPort > 0)
                {
                    using (SmtpClient smtp = new SmtpClient(_emailSettings.PrimaryDomain, _emailSettings.PrimaryPort))
                    {
                        smtp.Credentials = new NetworkCredential(mail.From.Address, _emailSettings.UsernamePassword);
                        smtp.EnableSsl = true;
                        await smtp.SendMailAsync(mail);
                    }
                }
                else
                {
                    using (SmtpClient smtp = new SmtpClient(_emailSettings.PrimaryDomain))
                    {

                        smtp.DeliveryMethod = SmtpDeliveryMethod.Network;

                        smtp.Credentials = new NetworkCredential(_emailSettings.UsernameEmail, _emailSettings.UsernamePassword);
                        smtp.EnableSsl = true;
                        // smtp.Send(mail);
                        await smtp.SendMailAsync(mail);
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        
    }
}
