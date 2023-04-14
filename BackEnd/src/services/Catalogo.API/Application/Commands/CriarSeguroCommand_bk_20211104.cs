using FluentValidation;
using SEG.Core.Messages;
using SEG.Seguro.API.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SEG.Seguro.API.Application.Commands
{
    public class CriarSeguroCommand : Command
    {
        public int Empresa { get; set; }//
        public int Sucursal { get; set; }//
        public int Contrato { get; set; }//
        public int Proposta { get; set; }//
        public Titular Titular { get; set; }//
        public List<Agregado> Agregado { get; set; }//
        public List<Beneficiario> Beneficiario { get; set; }
        public int Colaborador { get; set; }//
        public char Usuario { get; set; }//
        public DateTime DataSistema { get; set; }
        public int Emissao { get; set; }


        public CriarSeguroCommand()
        {
        }

        public CriarSeguroCommand(int contrato, int propostaCorretor, DateTime inicioVigencia)
        {
            Contrato = contrato;
            Proposta = propostaCorretor;
            Titular.InicioVigencia = inicioVigencia;
        }

        public override bool EhValido()
        {
            ValidationResult = new CriarSeguroValidation().Validate(this);
            return ValidationResult.IsValid;
        }

        public class CriarSeguroValidation : AbstractValidator<CriarSeguroCommand>
        {
            public CriarSeguroValidation()
            {
                RuleFor(s => s.Empresa).NotEmpty().WithMessage("A empresa não foi informada");
                RuleFor(s => s.Sucursal).NotEmpty().WithMessage("A Sucursal não foi informada");
                RuleFor(s => s.Contrato).NotEmpty().WithMessage("O contrato não foi informado");
                RuleFor(s => s.Titular.Pessoa.Nome).NotEmpty().WithMessage("O titular não foi informado");
                RuleFor(s => s.Beneficiario).NotEmpty().WithMessage("O beneficiario não foi informado");
                RuleFor(s => s.Proposta).NotEmpty().WithMessage("A proposta não foi informada");
                RuleFor(s => s.Colaborador).NotEmpty().WithMessage("O colaborador não foi informado");
                RuleFor(s => s.Usuario).NotEmpty().WithMessage("O usuario não foi informado");
            }
        }
    }



    public class Titular
    {
        public DateTime InicioVigencia { get; set; }//
        public int Agrupamento { get; set; }//
        public short Produto { get; set; }//
        public short Plano { get; set; }//
        public DateTime VigenciaPlano { get; set; }//
        public string Digital { get; set; }//
        public decimal PremioTotal { get; set; }//
        public List<Coberturas> Coberturas { get; set; }//
        public VendaAdministrativa VendaAdministrativa { get; set; }//
        public short MesesParaRenda { get; set; }//
        public short Periodicidade { get; set; }//
        public List<DPS> DPS { get; set; }//
        public Pessoa Pessoa { get; set; }//
    }

    public class Coberturas
    {
        public short Cobertura { get; set; }
        public int IS { get; set; }
    }


    public class VendaAdministrativa
    {
        public short Motivo { get; set; }
        public int ContratoOriginal { get; set; }
        public int CertificadoOriginal { get; set; }
    }

 
    public class DPS
    {
        public string Pergunta { get; set; }
        public string Resposta { get; set; }
        public string Complemento { get; set; }
    }

    public class Pessoa
    {
        public int Cdpes { get; set; }//
        public char Nome { get; set; }//
        public DateTime DataNascimento { get; set; }//
        public string Sexo { get; set; }
        public decimal CPF { get; set; }//
        //public string CPFProprio { get; set; }
        //public decimal Renda { get; set; }
        //public string Atividade { get; set; }
        //public string EstadoCivil { get; set; }
        //public string Email { get; set; }
        //public Decimal RG { get; set; }
        //public char OrgaoExpedidor { get; set; }
        //public DateTime DataExpedicao { get; set; }
        //public List<Telefone> Telefones { get; set; }
        //public List<Endereco> Endereco { get; set; }
    }

    

    //public class Telefone
    //{
    //    public int DDD { get; set; }
    //    public string Numero { get; set; }
    //    public string Tipo { get; set; }
    //    public string CelularPrincipal { get; set; }
    //    public string ReceberSMS { get; set; }
    //    public string Whatsapp { get; set; }
    //}

    //public class Endereco
    //{
    //    public string Cep { get; set; }
    //    public string Logradouro { get; set; }
    //    public string Numero { get; set; }
    //    public string Complemento { get; set; }
    //    public string Bairro { get; set; }
    //    public string Cidade { get; set; }
    //    public string UF { get; set; }
    //    public string Referencia { get; set; }
    //    public string Pais { get; set; }
    //    public string Tipo { get; set; }
    //}

    public class Agregado
    {
        public DateTime InicioVigencia { get; set; }
        public int Agrupamento { get; set; }
        public short Produto { get; set; }
        public string Plano { get; set; }
        public DateTime VigenciaPlano { get; set; }
        public decimal PremioTotal { get; set; }
        public List<Coberturas> Coberturas { get; set; }
        public short? TipoAgregado { get; set; } // 
        public string GrauParentesco { get; set; }
        public List<DPS> DPS { get; set; }
        public Pessoa Pessoa { get; set; }
    }

    public class Beneficiario
    {
        public char Nome { get; set; }
        public DateTime DataNascimento { get; set; }
        public string Sexo { get; set; }
        public decimal CPF { get; set; }
        public string Parentesco { get; set; }
        public decimal PercentualParticipacao { get; set; }
        //public List<Telefone> Telefones { get; set; }
    }



}
