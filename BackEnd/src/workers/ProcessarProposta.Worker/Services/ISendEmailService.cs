using SEG.Core.Messages.Integration;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ProcessarProposta.Worker.Services
{
    public interface ISendEmailService
    {
        Task ComunicarEmail(ComunicarEmailEvent message);
    }
}
