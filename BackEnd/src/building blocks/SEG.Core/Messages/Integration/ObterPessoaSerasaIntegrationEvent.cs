using Newtonsoft.Json;
using SEG.Core.Enum;
using System;

namespace SEG.Core.Messages.Integration
{
    public class ObterPessoaSerasaIntegrationEvent : IntegrationEvent
    {
        public string cpf { get; set; }

        public ObterPessoaSerasaIntegrationEvent(string cpf){
            this.cpf = cpf;
        }

        
        
    }
}