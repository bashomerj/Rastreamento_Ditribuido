using SEG.Webhook.API.Data;
using SEG.Webhook.API.Models.Entities;
using SEG.Webhook.API.Models.Interfaces;
using SEG.Webhook.API.Models.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SEG.Webhook.API.Services
{
    public class WebhooksRetriever : IWebhooksRetriever
    {
        private readonly IAssinaturaWebhookRepository _assinaturaWebhookRepository;
        public WebhooksRetriever(IAssinaturaWebhookRepository assinaturaWebhookRepository)
        {
            _assinaturaWebhookRepository = assinaturaWebhookRepository;
        }
        public async Task<IEnumerable<AssinaturaWebhook>> ObterAssinaturasPorTipo(Guid idCliente, WebhookType type)
        {
            var result = _assinaturaWebhookRepository.ObterLista(a => a.idCliente.CompareTo(idCliente)==0 && a.tipo == type.ToString());
            return result;
        }

      
    }
}
