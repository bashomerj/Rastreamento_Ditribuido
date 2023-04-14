using Newtonsoft.Json;
using System;

namespace Core.Messages
{
    public abstract class Message
    {
        public string MessageType { get; protected set; }
        public string AggregateId { get;  set; }

        protected Message()
        {
            MessageType = GetType().Name;
        }

        public void AtribuirAggregateRoot(object obj)
        {
            //faz o tratamento para o AggregateId criando um json com a chave primaria da agregação
            var aggregateRoot = JsonConvert.SerializeObject(obj,Formatting.Indented);
            this.AggregateId = aggregateRoot;
        }
    }
}