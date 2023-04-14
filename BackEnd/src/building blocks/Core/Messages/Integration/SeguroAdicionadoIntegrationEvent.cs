using Newtonsoft.Json;
using Core.Enum;
using System;

namespace Core.Messages.Integration
{
    public class SeguroAdicionadoIntegrationEvent : IntegrationEvent
    {
        public SeguroAdicionadoIntegrationEvent(int contrato, int emissao, int certificado, int proposta, DateTime inicio_vigencia)
        {
            this.contrato = contrato;
            this.emissao = emissao;
            this.certificado = certificado;
            this.proposta = proposta;
            this.inicio_vigencia = inicio_vigencia;
            AtribuirAggregateRoot(new { contrato = contrato, emissao = emissao, certificado = certificado });
        }

        public int contrato { get; set; }
        public int emissao { get; set; }
        public int certificado { get; set; }
        public int proposta { get; set; }
        public DateTime inicio_vigencia { get; set; }
       

        
    }
}