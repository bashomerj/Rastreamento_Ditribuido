using SEG.Core.DomainObjects;
using SEG.Webhook.API.Models.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SEG.Webhook.API.Models.Entities
{
    public class AssinaturaWebhook : Entity, IAssinaturaWebhook, IAggregateRoot
    {
        public Guid id { get; set; }
        public Guid idCliente { get; set; }
        public string tipo { get; set; }
        public DateTime dataCriacao { get; set; }
        public string urlDestino { get; set; }
        public string token { get; set; }
        
        public virtual ClienteIntegracao ClienteIntegracao { get; set; }

        public AssinaturaWebhook()
        {
                
        }
    }
}
