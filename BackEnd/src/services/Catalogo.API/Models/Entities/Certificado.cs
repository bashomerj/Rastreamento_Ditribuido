using Catalogo.API.Models.Interfaces;
using Core.DomainObjects;
using System;

namespace Catalogo.API.Models.Entities
{
    public class Certificado : Entity, ICertificado, IAggregateRoot
    {
        public Guid id { get; private set; }
        public Guid idProposta { get; private set; }
        public int certificado { get; private set; }


  
        
        protected Certificado()
        {

        }

        
    }
}
