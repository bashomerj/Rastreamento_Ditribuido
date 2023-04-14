using SEG.Core.Enum;
using System;
using System.Collections.Generic;

namespace SEG.Core.Messages.Integration
{
    public class PropostaProcessadaIntegrationEvent : IntegrationEvent
    {
        public Guid origem { get; set; }
        public int proposta { get; set; }
        public int situacao { get; set; }
        public bool? redigitacao { get; set; }
        public string motivo_redigitacao { get; set; }

    }
}
