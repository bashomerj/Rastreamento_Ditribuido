using Newtonsoft.Json;
using System;

namespace Core.Messages.Integration
{
    public class PessoaAdicionadoIntegrationEvent : IntegrationEvent
    {
        public int cdpes { get; private set; }
        public int tppes { get; private set; }
        public decimal nrcgccpf { get; private set; }
        public char nmpes { get; private set; } // temp
        public DateTime dtnas { get; private set; }
        public int tpsex { get; private set; }
        public int stestciv { get; private set; }

        public PessoaAdicionadoIntegrationEvent(int cdpes, int tppes, decimal nrcgccpf, char nmpes, DateTime dtnas, int tpsex, int stestci)
        {
            this.cdpes = cdpes;
            this.tppes = tppes;
            this.nrcgccpf = nrcgccpf;
            this.nmpes = nmpes;
            this.dtnas = dtnas;
            this.tpsex = tpsex;
            this.stestciv = stestciv;
            AtribuirAggregateRoot(new { cdpes = cdpes});
        }

        //public void AtribuirAggregateRoot(int cdpes)
        //{
        //    //faz o tratamento para o AggregateId crinado um json com a chave primaria da agregação
        //    var entity = new { cdpes = cdpes };
        //    var aggregateRoot = JsonConvert.SerializeObject(entity);
        //    this.AggregateId = aggregateRoot;
        //}
    }
}