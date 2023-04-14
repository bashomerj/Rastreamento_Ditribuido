using Newtonsoft.Json;
using Core.Enum;
using System;

namespace Core.Messages.Integration
{
    public class ObterPessoaSerasaIntegrationEvent : IntegrationEvent
    {
        public string cpf { get; set; }

        public ObterPessoaSerasaIntegrationEvent(string cpf){
            this.cpf = cpf;
        }

        
        
    }
}