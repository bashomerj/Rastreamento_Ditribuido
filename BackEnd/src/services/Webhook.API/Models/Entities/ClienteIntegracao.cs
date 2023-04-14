using SEG.Core.DomainObjects;
using SEG.Webhook.API.Models.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SEG.Webhook.API.Models.Entities
{
    public class ClienteIntegracao : Entity, IClienteIntegracao, IAggregateRoot
    {
        public Guid id { get; set; }
        public string nome { get; set; }
        public DateTime dataCriacao { get; set; }
        


        public ClienteIntegracao()
        {
                
        }
    }
}
