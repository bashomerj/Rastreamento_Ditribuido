using SEG.Core.Enum;
using System;
using System.Collections.Generic;
using System.Text;

namespace ProcessarProposta.Worker.DTOs
{

    public class SeguroCadastroDTO
    {
        public int Empresa { get; set; }
        public int Sucursal { get; set; }
        public string Usuario { get; set; }

        public InfoSeguroDTO Seguro { get; set; }
        public InfoSeguradoDTO Titular { get; set; }
        public List<InfoSeguradoDTO> Agregados { get; set; }
    }

    public class InfoSeguroDTO
    {
        public int contrato { get; set; }
        public int proposta { get; set; }
        public DateTime inicio_vigencia { get; set; }
        public int Agrupamento { get; set; }
        public short produto { get; set; }
        public Digital digital { get; set; }
        public decimal premio_total { get; set; }
        public int colaborador { get; set; }
        public short meses_para_renda { get; set; }
        public Periodicidade periodicidade { get; set; }

        public VendaAdministrativaDTO VendaAdministrativa { get; set; }

        public InfoMeioPagamento MeioPagamento { get; set; }
        public InfoTransferenciaPagamento TransferenciaPagamento { get; set; }
        public List<InfoBeneficiario>? Beneficiarios { get; set; }


    }

    public class InfoMeioPagamento
    {
        public Meio_Pagamento meio_pagamento { get; set; }

        public InfoDebitoAutomatico debito_automatico { get; set; }
    }

    public class InfoDebitoAutomatico
    {

        public string agencia { get; set; }
        public string digito_Agencia { get; set; }
        public string conta { get; set; }
        public string digito_Conta { get; set; }
        public string tipo { get; set; } //char
        public string categoria { get; set; } //char
        public string titular { get; set; }
        public string cpf_Titular { get; set; }


        private readonly List<string> listaTipoConta = new List<string> { "conta_corrente", "conta_poupanca" };
        private readonly List<string> listaTipoCategoria = new List<string> { "individual", "conjunta" };


    }

    public class InfoTransferenciaPagamento
    {
        public int proposta_origem { get; set; }
        public decimal valor_proposta_origem { get; set; }
    }
    public class InfoBeneficiario
    {
        public string Nome { get; set; }//{ get => Nome; set { if (value != null) Nome = value.Trim().ToUpper(); } }
        public DateTime Data_Nascimento { get; set; }
        public Sexo Sexo { get; set; }//{ get => Sexo; set { if (value != null) Sexo = value.Trim().ToUpper(); } }
        public string CPF { get; set; }
        public Grau_de_Parentesco Parentesco { get; set; }//{ get => Parentesco; set { if (value != null) Parentesco = value.Trim().ToUpper(); } }
        public decimal? Porcentagem_Participacao { get; set; }
        public List<TelefoneDTO> Telefones { get; set; }
    }

    public class InfoSeguradoDTO
    {
        public int Plano { get; set; }
        public DateTime Vigencia_Plano { get; set; }
        public int? Emissao { get; set; }
        public decimal Premio_Total { get; set; }
        public Tipo_Segurado Tipo_Segurado { get; set; }
        public Grau_de_Parentesco Grau_Parentesco { get; set; }
        public short meses_para_renda { get; set; }
        public List<CoberturasDTO> Coberturas { get; set; }
        public List<DPSDTO> DPS { get; set; }
        public PessoaDTO Pessoa { get; set; }
    }

    public class InfoPessoaDTO
    {
        public Guid Id { get; private set; }
        public string Nome { get; set; }
        public int Cdpes { get; set; }
        public DateTime Data_Nascimento { get; set; }
        public Sexo Sexo { get; set; }
        public string CPF { get; set; }
        public Cpf_Proprio? CPF_Proprio { get; set; }
        public decimal? Renda { get; set; }

    }

}
