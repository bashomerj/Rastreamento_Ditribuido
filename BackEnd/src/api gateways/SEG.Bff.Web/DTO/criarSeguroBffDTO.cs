using SEG.Core.Enum;
using System;
using System.Collections.Generic;

namespace SEG.Bff.Web.DTO
{
    public class CriarSeguroDTO
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
        public Meio_Pagamento meio_pagamento { get; set; }

        public VendaAdministrativaDTO VendaAdministrativa { get; set; }

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
