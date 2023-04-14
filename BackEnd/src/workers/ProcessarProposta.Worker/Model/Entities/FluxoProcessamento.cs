using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Schema;
using SEG.Cobranca.API.Models.Interfaces;
using Core.DomainObjects;
using System;

namespace ProcessarProposta.Worker.Model.Entities
{
    public class FluxoProcessamento : Entity, IFluxoProcessamento, IAggregateRoot
    {


        public Guid id { get; private set; }
        public DateTime dataInicio { get; private set; }
        public string cadastroSegurados { get; private set; }
        public string aceitacaoRiscos { get; private set; }
        public string parcela { get; private set; }
        public string comunicacaoWebhook { get; private set; }
        public DateTime dataConclusao { get; private set; }
        public string mensagem { get; private set; }
        public int proposta { get; private set; }
        public int? contratoTitular { get; private set; }
        public int? emissaoTitular { get; private set; }
        public int? certificadoTitular { get; private set; }
        public int? itemTitular { get; private set; }
        public string jsonResultCadastroSegurados { get; private set; }
        public string jsonResultAceitacaoRiscos { get; private set; }
        public string jsonResultParcela { get; private set; }
        public string jsonResultcomunicacaoWebhook { get; private set; }
        public string canceladoProcessamento { get; private set; }
        public string motivoCancelamento { get; private set; }

        





        protected FluxoProcessamento()
        {

        }

        public FluxoProcessamento(Guid id, DateTime dataInicio, string json, int proposta)
        {
            this.id = id;
            this.dataInicio = dataInicio;
            this.cadastroSegurados = null;
            this.aceitacaoRiscos = null;
            this.parcela = null;
            this.comunicacaoWebhook = null;
            this.jsonResultcomunicacaoWebhook = null;
            this.dataConclusao = DateTime.ParseExact("1901-01-01", "yyyy-MM-dd", null);
            this.mensagem = json;
            this.proposta = proposta;
            this.contratoTitular = null;
            this.emissaoTitular = null;
            this.certificadoTitular = null;
            this.itemTitular = null;
            this.jsonResultCadastroSegurados = null;
            this.jsonResultAceitacaoRiscos = null;
            this.jsonResultParcela = null;

        }

        public void AtribuirSucessoCadastroSegurados() => this.cadastroSegurados = "S";
        public void AtribuirRejeicaoCadastroSegurados() => this.cadastroSegurados = "N";


        public void AtribuirSucessoAceitacaoRisco() => this.aceitacaoRiscos = "S";
        public void AtribuirRejeicaoAceitacaoRisco() => this.aceitacaoRiscos= "N";


        public void AtribuirSucessoParcela() => this.parcela = "S";
        public void AtribuirRejeicaoParcela() => this.parcela = "N";

        public void AtribuirMensagem(string mensagem) => this.mensagem = mensagem;

        public void AtribuirIdentificacaoSeguroTitular(int contrato, int emissao, int certificado, int item)
        {
            this.contratoTitular = contrato;
            this.emissaoTitular = emissao;
            this.certificadoTitular = certificado;
            this.itemTitular = item;
        }

        public void ConcluirFluxo() => this.dataConclusao = DateTime.Now;

        public void AtribuirJsonResultCadastroSegurados(string jsonResult) => this.jsonResultCadastroSegurados = jsonResult;
        public void AtribuirJsonResultAceitacaoRiscos(string jsonResult) => this.jsonResultAceitacaoRiscos = jsonResult;
        public void AtribuirJsonResultParcela(string jsonResult) => this.jsonResultParcela = jsonResult;
        public void AtribuirSucessoComunicacaoWebhook() => this.comunicacaoWebhook = "S";
        public void AtribuirJsonResultComunicacaoWebhook(string jsonResult) => this.jsonResultcomunicacaoWebhook = jsonResult;

        public void AtribuirCancelamentoProcessamento(string jsonResult) => this.canceladoProcessamento = jsonResult;
        public void AtribuirMotivoCancelamento(string jsonResult) => this.motivoCancelamento = jsonResult;


    }
}
