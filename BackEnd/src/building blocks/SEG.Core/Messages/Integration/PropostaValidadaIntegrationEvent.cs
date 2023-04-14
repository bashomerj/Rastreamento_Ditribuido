using SEG.Core.Enum;
using System;
using System.Collections.Generic;

namespace SEG.Core.Messages.Integration
{
    public class PropostaValidadaIntegrationEvent : IntegrationEvent
    {
        public int Empresa { get; set; }
        public int Sucursal { get; set; }
        public string Usuario { get; set; }//{ get => Usuario; set { if (value != null) Usuario = value.Trim().ToUpper(); } }

        public Guid Origem { get; set; }
        public SeguroValidado Seguro { get; set; }
        public SeguradoValidado Titular { get; set; }
        public List<SeguradoValidado> Agregados { get; set; }


    }

    public class SeguroValidado
    {
        public int contrato { get; set; }
        public int proposta { get; set; }
        public DateTime inicio_vigencia { get; set; }
        public short produto { get; set; }
        public Digital digital { get; set; }
        public decimal premio_total { get; set; }
        public int colaborador { get; set; }
        public Periodicidade periodicidade { get; set; }
        public MeioPagamentoValidada MeioPagamento { get; set; }

        public VendaAdministrativaValidada VendaAdministrativa { get; set; }
        public List<BeneficiarioValidado>? Beneficiarios { get; set; }
        public TransferenciaPagamentoValidada TransferenciaPagamento { get; set; }
    }
    public class SeguradoValidado
    {

        public int Plano { get; set; }
        public DateTime Vigencia_Plano { get; set; }
        public int? Emissao { get; set; }
        public decimal Premio_Total { get; set; }
        public short meses_para_renda { get; set; }
        public Tipo_Segurado Tipo_Segurado { get; set; }
        public Grau_de_Parentesco Grau_Parentesco { get; set; }
        public List<CoberturasValidadas> Coberturas { get; set; }
        public List<DPSValidada> DPS { get; set; }
        public PessoaValidada Pessoa { get; set; }
    }
    public class PessoaValidada
    {
        public Guid Id { get; set; }
        public string Nome { get; set; }
        public int Cdpes { get; set; }
        public DateTime Data_Nascimento { get; set; }
        public Sexo Sexo { get; set; }
        public string CPF { get; set; }
        public Cpf_Proprio? CPF_Proprio { get; set; }
        public decimal? Renda { get; set; }
        public string Atividade { get; set; }
        public Estado_Civil? Estado_Civil { get; set; }
        public string Email { get; set; }//{ get => Email; set { if (value != null) Email = value.Trim(); } }
        public string RG { get; set; }
        public string Orgao_Expedidor { get; set; }
        public DateTime Data_Expedicao { get; set; }
        public List<TelefoneValidado> Telefones { get; set; }
        public EnderecoValidado Endereco { get; set; }

    }
    public class EnderecoValidado
    {
        public string Cep { get; set; }//{ get => Cep; set { if (value != null) Cep = value.Trim().ToUpper(); } }
        public string Logradouro { get; set; }//{ get => Logradouro; set { if (value != null) Logradouro = value.Trim().ToUpper(); } }
        public int? Numero { get; set; }//{ get => Numero; set { if (value != null) Numero = value.Trim().ToUpper(); } }
        public string Complemento { get; set; }//{ get => Complemento; set { if (value != null) Complemento = value.Trim().ToUpper(); } }
        public string Bairro { get; set; }//{ get => Bairro; set { if (value != null) Bairro = value.Trim().ToUpper(); } }
        public string Cidade { get; set; }//{ get => Cidade; set { if (value != null) Cidade = value.Trim().ToUpper(); } }
        public string UF { get; set; }//{ get => UF; set { if (value != null) UF = value.Trim().ToUpper(); } }
        public string Referencia { get; set; }//{ get => Referencia; set { if (value != null) Referencia = value.Trim().ToUpper(); } }
        public string Pais { get; set; }//{ get => Pais; set { if (value != null) Pais = value.Trim().ToUpper(); } }
    }
    public class CoberturasValidadas
    {
        public int Cobertura { get; set; }
        public decimal IS { get; set; }
        public decimal Premio { get; set; }
    }
    public class VendaAdministrativaValidada
    {
        public Motivo_Venda_Administrativa Motivo { get; set; }
        public int Contrato_Original { get; set; }
        public int Certificado_Original { get; set; }
    }
    public class DPSValidada
    {
        public Guid Pergunta { get; set; }//{ get => Pergunta; set { if (value != null) Pergunta = value.Trim().ToUpper(); } }
        //public Opcao_Resposta_DPS? Resposta { get; set; }//{ get => Resposta; set { if (value != null) Resposta = value.Trim().ToUpper(); } }
        public Guid Resposta { get; set; }//{ get => Resposta; set { if (value != null) Resposta = value.Trim().ToUpper(); } }
        public string Complemento { get; set; }//{ get => Complemento; set { if (value != null) Complemento = value.Trim().ToUpper(); } }
    }
    public class TelefoneValidado
    {
        public int DDD { get; set; }
        public string Numero { get; set; }
        public Tipo_Telefone Tipo { get; set; }
        public Celular_Principal? Celular_Principal { get; set; }
        public Receber_SMS? Receber_SMS { get; set; }
        public Whastapp? Whatsapp { get; set; }
    }
    public class BeneficiarioValidado
    {
        public string Nome { get; set; }
        public DateTime Data_Nascimento { get; set; }
        public Sexo? Sexo { get; set; }
        public string CPF { get; set; }
        public Grau_de_Parentesco Parentesco { get; set; }
        public decimal? Porcentagem_Participacao { get; set; }
        public List<TelefoneValidado> Telefones { get; set; }
    }

    public class TransferenciaPagamentoValidada
    {
        public int proposta_origem { get; set; }

        public decimal valor_proposta_origem { get; set; }

    }

    public class MeioPagamentoValidada
    {
        public Meio_Pagamento meio_pagamento { get; set; }
        public DebitoAutomaticoValidada debito_automatico { get; set; }
    }


    
    public class DebitoAutomaticoValidada
    {
        public string agencia { get; set; }
        public string digito_agencia { get; set; }
        public string conta { get; set; }
        public string digito_conta { get; set; }
        public string tipo { get; set; }
        public string categoria { get; set; }
        public string titular { get; set; }
        public string cpf_titular { get; set; }
    }
}