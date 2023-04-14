using SEG.Core.DomainObjects;
using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using Catalogo.API.Models.Interfaces;
using System.Globalization;

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
