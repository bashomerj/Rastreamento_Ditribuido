using SEG.Webhook.API.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SEG.Webhook.API.Services
{
    public interface IWebhooksRetriever
    {

        Task<IEnumerable<AssinaturaWebhook>> ObterAssinaturasPorTipo(Guid idCliente, WebhookType type);
    }
}
