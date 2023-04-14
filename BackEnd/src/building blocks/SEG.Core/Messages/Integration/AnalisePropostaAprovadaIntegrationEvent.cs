using SEG.Core.Enum;
using System;
using System.Collections.Generic;

namespace SEG.Core.Messages.Integration
{
    public class AnalisePropostaAprovadaIntegrationEvent : IntegrationEvent
    {
        public int empresa { get; set; }
        public int sucursal { get; set; }
        public string usuario { get; set; }//{ get => Usuario; set { if (value != null) Usuario = value.Trim().ToUpper(); } }
        public decimal nossoNumeroOrigem { get; set; }
        public decimal nossoNumeroDestino { get; set; }

    }
}
