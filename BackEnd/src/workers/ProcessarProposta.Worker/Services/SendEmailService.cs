using Core.Messages.Integration;
using Email;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ProcessarProposta.Worker.Services
{
    public class SendEmailService : ISendEmailService
    {

        public readonly IEmailSender _email;

        public SendEmailService(IEmailSender emailSender)
        {
            _email = emailSender;
        }

        public async Task ComunicarEmail(ComunicarEmailEvent message)
        {
            await _email.SendEmailAsync(message.Para, message.Assunto, message.Corpo);

            return;
        }
    }
}
