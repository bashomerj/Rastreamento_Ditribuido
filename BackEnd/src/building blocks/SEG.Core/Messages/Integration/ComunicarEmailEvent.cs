using SEG.Core.Enum;
using System;
using System.Collections.Generic;

namespace SEG.Core.Messages.Integration
{
    public class ComunicarEmailEvent : IntegrationEvent
    {
        public string Para { get; set; }
        public string  Assunto { get; set; }
        public string Corpo { get; set; }
           
    }

    

}
