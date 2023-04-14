using SEG.Core.DomainObjects;
using SEG.Webhook.API.Models.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SEG.Webhook.API.Models.Entities
{
    public class WebHookSaida : Entity, IWebHookSaida, IAggregateRoot
    {
        public Guid id { get; set; }
        public Guid idAssinatura { get; set; }
        public string tipo { get; set; }
        public DateTime dataEnvio { get; set; }
        public string urlDestino { get; set; }
        public string token { get; set; }
        public string payload { get; set; }
        public int? codigoRetorno { get; set; }

        public virtual AssinaturaWebhook AssinaturaWebhook { get; set; }


        public WebHookSaida()
        {
                
        }
    }
}
