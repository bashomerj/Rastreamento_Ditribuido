using FluentValidation;
using FluentValidation.Results;
using System;
using System.Collections.Generic;
using System.Linq;
using Core.Enum;
using Swashbuckle.AspNetCore.Annotations;
using Bff.Web.Attributes;
using System.Text.Json.Serialization;
using Newtonsoft.Json.Converters;
using System.ComponentModel.DataAnnotations;
using Bff.Web.Resources;
using Microsoft.Extensions.Localization;
using System.Globalization;


namespace Bff.Web.DTO
{

    [SwaggerSchema(Required = new[] { "empresa", "sucursal", "usuario", "origem", "seguro", "titular" })]
    public class NovaPropostaDTO
    {
        [SwaggerSchema(description: "código da empresa")]
        [SwaggerSchemaExample("1000")]
        public int Empresa { get; set; }

        [SwaggerSchema(description: "código da sucursal")]
        [SwaggerSchemaExample("2000")]
        public int Sucursal { get; set; }
        
        [SwaggerSchema(description: "código do usuário que realizou a digitação da proposta")]
        [SwaggerSchemaExample("informatica")]
        public string Usuario { get; set; }

        
        [SwaggerSchema(description: "Id de identificação fornecido pela SINAF SEGURO para identificação do Cliente")]
        [SwaggerSchemaExample("3ddd14db-de02-4789-85d2-63b18c3d6161")]
        public Guid Origem { get; set; }

        [SwaggerSchema(description: "Informações gerais pertinentes ao seguro que está sendo contratado")]
        public SeguroDTO Seguro { get; set; }

        [SwaggerSchema(description: "Informações relacionadas ao segurado titular. Tais como: Informações pessoais, DPS, Coberturas contratadas e etc")]
        public SeguradoDTO Titular { get; set; }

        [SwaggerSchema(description: "Lista de dependentes/agregados vinculados ao seguro do titular, assim as informações relacionadas a cada segurado")]
        public List<SeguradoDTO> Agregados { get; set; }

        public FluentValidation.Results.ValidationResult validationResult { get; private set; }

        public NovaPropostaDTO()
        {
        }

        public bool EhValido()
        {
            var validation = new NovaPropostaDTOValidation();
            validation.objeto = this;
            validationResult = validation.Validate(this);
            return validationResult.IsValid;
        }

        public class NovaPropostaDTOValidation : AbstractValidator<NovaPropostaDTO>
        {
            public NovaPropostaDTO objeto;


            public NovaPropostaDTOValidation()
            {
                //Root
                RuleFor(t => t.Empresa).Equal(1000);
                RuleFor(t => t.Sucursal).Equal(2000);
                RuleFor(t => t.Usuario)
                    .MinimumLength(4)
                    .MaximumLength(12);

                //Seguro
                RuleFor(t => t.Seguro).NotNull()
                    .DependentRules(() => {
                        RuleFor(t => t.Seguro.proposta).GreaterThan(1);
                        RuleFor(t => t.Seguro.contrato).GreaterThan(1);
                        //TODO: DESCOMENTAR (COMENTADO SOMENTE PARA TESTE)
                        //RuleFor(t => t.Seguro.inicio_vigencia).GreaterThan(DateTime.Today.AddYears(-1)).WithMessage(t => $"Início da vigência não pode ser anterior a mais de 1 ano da data de hoje");
                        RuleFor(t => (int)t.Seguro.produto).GreaterThan(1);
                        RuleFor(t => t.Seguro.premio_total)
                            .GreaterThan(0)
                            .ScalePrecision(2, 7);

                        RuleFor(t => t.Seguro.colaborador).GreaterThan(0);




                        //Beneficiários
                        RuleFor(t => t.Seguro.Beneficiarios)
                            .Must(TerPercentualParticipacaoValido);

                        When(t => t.Seguro.Beneficiarios != null, () =>
                        {
                            RuleForEach(t => t.Seguro.Beneficiarios)
                                    .ChildRules(c =>
                                    {
                                        c.RuleFor(b => b.Nome)
                                            .NotNull()
                                            .MinimumLength(5)
                                            .MaximumLength(40);

                                        c.RuleFor(b => b.Data_Nascimento.Date)
                                            .GreaterThanOrEqualTo(DateTime.Today.AddYears(-130).Date)
                                            .LessThanOrEqualTo(DateTime.Today.Date);
                                        //TODO: Validar enum de sexo
                                        //c.RuleFor(b => b.Sexo)
                                        //    .NotNull().WithMessage(t => $"Sexo do beneficiário {t.Nome.Trim()} não informado ou inválido");
                                        //.Must(TerSexoValido).WithMessage(t => $"Sexo do beneficiário {t.Nome.Trim()} tem que ser MASCULINO OU FEMININO");

                                        c.RuleFor(b => b.CPF)
                                            .NotEmpty()
                                            .Must(TerCpfValido).WithMessage(b => $"Beneficiário com CPF {b.CPF} inválido");


                                        //c.RuleFor(b => b.Parentesco)
                                        //    .NotNull().WithMessage(t => $"Parentesco do beneficiário {t.Nome.Trim()} não informado ou inválido");
                                        //.MinimumLength(5).WithMessage(t => $"Parentesco do beneficiário {t.Nome.Trim()} não pode ter menor do que 5 caracteres")
                                        //.Must(TerParentescoValido).WithMessage(t => $"Parentesco do beneficiário {t.Nome.Trim()} inválido");
                                        c.RuleFor(b => b.Porcentagem_Participacao)
                                            .NotNull()
                                            .GreaterThan(0)
                                            .ScalePrecision(2, 5)
                                            .LessThanOrEqualTo(100);
                                    });
                        });

                        //Meio Pagamento
                        When(t => t.Seguro.MeioPagamento.meio_pagamento == Meio_Pagamento.debito_automatico, () =>
                        {
                            RuleFor(t => t.Seguro.periodicidade).Equal(Periodicidade.mensal);
                            RuleFor(t => t.Seguro.MeioPagamento.debito_automatico).NotNull();
                            RuleFor(t => t.Seguro.MeioPagamento.debito_automatico).ChildRules(c =>
                            {
                                c.RuleFor(b => b.agencia).NotNull();
                                c.RuleFor(b => b.conta).NotNull();
                                c.RuleFor(b => b.digito_conta).NotNull();
                                c.RuleFor(b => b.tipo).NotNull();
                                c.RuleFor(b => b.categoria).NotNull();
                                c.RuleFor(b => b.titular).NotNull();
                                c.RuleFor(b => b.cpf_titular).NotNull().Must(TerCpfValido).WithMessage(b => $"CPF do titular do débito automático {b.cpf_titular} inválido");
                                

                            });
                        });



                        //Transferencia de Pagamento
                        When(t => t.Seguro.TransferenciaPagamento != null, () =>
                        {
                            RuleFor(t => t.Seguro.TransferenciaPagamento).ChildRules(c =>
                            {
                                c.RuleFor(b => b.proposta_origem).GreaterThan(1);
                                c.RuleFor(b => b.valor_proposta_origem)
                                    .GreaterThan(0)
                                    .ScalePrecision(2, 7);
                            });
                        });

                    });

                //Titular
                RuleFor(t => t.Titular).NotNull()
                    .DependentRules(() => {

                        RuleFor(t => t.Titular.Plano).GreaterThan(0);
                        RuleFor(t => t.Titular.Vigencia_Plano.Year).GreaterThan(2002);
                        RuleFor(t => t.Titular.Premio_Total)
                            .GreaterThan(0)
                            .ScalePrecision(2, 7);
                        RuleFor(t => (int)t.Titular.meses_para_renda).GreaterThanOrEqualTo(0)
                                .LessThanOrEqualTo(24);

                        //Coberturas Titular
                        RuleFor(t => t.Titular.Coberturas).NotNull()
                            .DependentRules(() =>
                            {
                                RuleForEach(t => t.Titular.Coberturas)
                                    .ChildRules(c =>
                                    {
                                        c.RuleFor(b => b.Cobertura)
                                            .GreaterThan(0);

                                        c.RuleFor(t => t.IS)
                                            .GreaterThanOrEqualTo(0)
                                            .ScalePrecision(2, 9);

                                        c.RuleFor(t => t.Premio)
                                            .GreaterThanOrEqualTo(0)
                                            .ScalePrecision(2, 7);
                                    });
                            });


                        //DPS Titular
                        //RuleFor(t => t.Titular.DPS).NotNull()
                        //   .DependentRules(() =>
                        //   {
                        //       RuleForEach(t => t.Titular.DPS)
                        //           .ChildRules(c =>
                        //           {
                        //               c.RuleFor(b => b.Pergunta)
                        //                .NotNull().WithMessage(t => $"A identificação da pergunta da DPS do titular {objeto?.Titular?.Pessoa?.Nome?.Trim()} não foi informada");


                        //               c.RuleFor(b => b.Resposta)
                        //                .NotNull().WithMessage(t => $"A resposta da pergunta {t.Pergunta} da DPS do titular {objeto?.Titular?.Pessoa?.Nome?.Trim()} não foi informada");
                        //               //.MinimumLength(3).WithMessage(t => $"A resposta {t.Resposta} da pergunta {t.Pergunta?.Trim()} da DPS do titular {objeto?.Titular?.Pessoa?.Nome?.Trim()} tem que ter no mínimo 3 caracteres")
                        //               //.MaximumLength(200).WithMessage(t => $"A resposta {t.Resposta} da pergunta {t.Pergunta?.Trim()} da DPS do titular {objeto?.Titular?.Pessoa?.Nome?.Trim()} tem que ter no máximo 200 caracteres");

                        //               c.RuleFor(b => b.Complemento)
                        //                .MaximumLength(200).WithMessage(t => $"O complemento {t.Complemento?.Trim()} da pergunta {t.Pergunta} da DPS do titular {objeto?.Titular?.Pessoa?.Nome?.Trim()} tem que ter no máximo 200 caracteres");
                        //           });
                        //   }).WithMessage(t => $"A DPS dp titular {t.Titular?.Pessoa?.Nome} não foi informada");


                        //Pessoa Titular
                        RuleFor(t => t.Titular.Pessoa).NotNull()
                            .DependentRules(() => {

                                RuleFor(t => t.Titular.Pessoa.Nome)
                                    .NotNull()
                                    .MinimumLength(5)
                                    .MaximumLength(40);

                                RuleFor(t => t.Titular.Pessoa.Data_Nascimento.Date)
                                    .GreaterThan(DateTime.Today.Date.AddYears(-110))
                                    .LessThanOrEqualTo(DateTime.Today.Date);

                                //TODO: Validar enum de sexo
                                //RuleFor(t => t.Titular.Pessoa.Sexo)
                                //    .NotEmpty().WithMessage(t => $"Sexo do titular {t.Titular.Pessoa.Nome?.Trim()} não informado ou inválido");
                                //.Must(TerSexoValido).WithMessage(t => $"Sexo do titular {t.Titular.Pessoa.Nome?.Trim()} tem que ser MASCULINO OU FEMININO");

                                RuleFor(t => t.Titular.Pessoa.CPF)
                                    .NotEmpty()
                                    .Must(TerCpfValido).WithMessage(b => $"Segurado com CPF {b.Titular.Pessoa.CPF} inválido");

                                RuleFor(t => t.Titular.Pessoa.CPF_Proprio)
                                    .NotEmpty();

                                RuleFor(t => t.Titular.Pessoa.Renda)
                                .NotEmpty()
                                    .GreaterThan(0)
                                    .ScalePrecision(2, 9);

                                RuleFor(t => t.Titular.Pessoa.Atividade)
                                    .NotEmpty();

                                RuleFor(t => t.Titular.Pessoa.Estado_Civil)
                                    .NotEmpty();

                                RuleFor(t => string.IsNullOrWhiteSpace(t.Titular.Pessoa.Email) ? null : t.Titular.Pessoa.Email.Trim())
                                    .EmailAddress();

                                //Endereço Titular
                                RuleFor(t => t.Titular.Pessoa.Endereco).NotNull()
                                    .DependentRules(() => {

                                        RuleFor(t => t.Titular.Pessoa.Endereco.Cep)
                                     .NotEmpty();
                                        //TODO: Validar somente números com uma expressao regular

                                        RuleFor(t => t.Titular.Pessoa.Endereco.Logradouro)
                                            .NotEmpty()
                                            .MinimumLength(1)
                                            .MaximumLength(40);

                                        //RuleFor(t => t.Titular.Pessoa.Endereco.Numero)


                                        RuleFor(t => t.Titular.Pessoa.Endereco.Complemento)
                                        //    .MinimumLength(1).WithMessage(t => $"Complemento do endereço do titular tem que ter mais do que 1 caracter")
                                            .MaximumLength(20);

                                        RuleFor(t => t.Titular.Pessoa.Endereco.Bairro)
                                            .NotEmpty()
                                            .MinimumLength(1)
                                            .MaximumLength(20);

                                        RuleFor(t => t.Titular.Pessoa.Endereco.Cidade)
                                            .NotEmpty()
                                            .MinimumLength(1)
                                            .MaximumLength(20);

                                        RuleFor(t => t.Titular.Pessoa.Endereco.UF)
                                            .NotEmpty()
                                            .Length(2);

                                        RuleFor(t => t.Titular.Pessoa.Endereco.Referencia)
                                        //    .MinimumLength(1).WithMessage(t => $"Referência do endereço do titular tem que ter mais do que 1 caracter")
                                            .MaximumLength(60);


                                        RuleFor(t => t.Titular.Pessoa.Endereco.Pais)
                                            .NotEmpty();
                                    }).NotEmpty();

                                //Telefone Titular
                                When(t => t.Titular.Pessoa.Telefones != null, () =>
                                {
                                    RuleForEach(t => t.Titular.Pessoa.Telefones)
                                           .ChildRules(c =>
                                           {
                                               c.RuleFor(b => b.DDD)
                                                .GreaterThan(10);


                                               c.RuleFor(b => b.Numero)
                                                .NotNull()
                                                .MinimumLength(7)
                                                .MaximumLength(9);
                                               //TODO: Validar somente números com uma expressao regular


                                               //c.RuleFor(b => b.Tipo)
                                               // .NotNull().WithMessage(t => $"Tipo do telefone {t.Numero} do titular não foi informado ou inválido");
                                               //.Must(TerTipoTelefoneValido).WithMessage(t => $"Tipo do telefone  {t.Numero} do titular está inválido");

                                               c.When(t => t.Tipo == Tipo_Telefone.celular, () =>
                                               {
                                                   c.RuleFor(b => b.Celular_Principal)
                                                    .NotNull();

                                                   c.RuleFor(b => b.Receber_SMS)
                                                    .NotNull();

                                                   c.RuleFor(b => b.Whatsapp)
                                                    .NotNull();
                                               });

                                           });

                                });



                            });


                    });


                //Agregados
                When(t => t.Agregados != null, () =>
                {
                    RuleForEach(t => t.Agregados)
                           .ChildRules(c =>
                           {
                               c.RuleFor(t => t.Plano).GreaterThan(0);
                               c.RuleFor(t => t.Vigencia_Plano.Year).GreaterThan(2002);
                               c.RuleFor(t => t.Premio_Total)
                                   .GreaterThanOrEqualTo(0)
                                   .ScalePrecision(2, 7);
                               c.RuleFor(t => (int)t.meses_para_renda).GreaterThanOrEqualTo(0)
                                .LessThanOrEqualTo(24);

                               //Coberturas Agregado
                               c.RuleFor(x => x.Coberturas).NotNull()
                                   .DependentRules(() =>
                                   {
                                       RuleForEach(a => a.Agregados)
                                           .ChildRules(c =>
                                           {
                                               c.RuleForEach(b => b.Coberturas)
                                                .ChildRules(co =>
                                                {
                                                    co.RuleFor(b => b.Cobertura)
                                                        .GreaterThan(0);

                                                    co.RuleFor(t => t.IS)
                                                        .GreaterThanOrEqualTo(0)
                                                        .ScalePrecision(2, 9);

                                                    co.RuleFor(t => t.Premio)
                                                        .GreaterThanOrEqualTo(0)
                                                        .ScalePrecision(2, 7);

                                                });
                                           });
                                   });


                               //DPS
                               //c.RuleFor(t => t.DPS).NotNull().WithMessage(t => $"A DPS dp titular {t.Pessoa?.Nome} não foi informada");

                               //c.When(a => a.DPS != null, () => {
                               //    c.RuleForEach(d => d.DPS)
                               //    .ChildRules(d =>
                               //    {
                               //        d.RuleFor(b => b.Pergunta)
                               //         .NotNull().WithMessage(t => $"A descrição da Pergunta {t.Pergunta?.Trim()} da DPS do agregado {objeto?.Titular?.Pessoa?.Nome?.Trim()} não foi informada")
                               //         .MinimumLength(3).WithMessage(t => $"A descrição da Pergunta {t.Pergunta?.Trim()} da DPS do agregado {objeto?.Titular?.Pessoa?.Nome?.Trim()} tem que ter no mínimo 3 caracteres")
                               //         .MaximumLength(200).WithMessage(t => $"A descrição da Pergunta {t.Pergunta?.Trim()} da DPS do agregado {objeto?.Titular?.Pessoa?.Nome?.Trim()} tem que ter no máximo 200 caracteres");

                               //        d.RuleFor(b => b.Resposta)
                               //         .NotNull().WithMessage(t => $"A resposta {t.Resposta?.Trim()} da pergunta {t.Pergunta?.Trim()} da DPS do agregado {objeto?.Titular?.Pessoa?.Nome?.Trim()} não foi informada")
                               //         .MinimumLength(3).WithMessage(t => $"A resposta {t.Resposta?.Trim()} da pergunta {t.Pergunta?.Trim()} da DPS do agregado {objeto?.Titular?.Pessoa?.Nome?.Trim()} tem que ter no mínimo 3 caracteres")
                               //         .MaximumLength(200).WithMessage(t => $"A resposta {t.Resposta?.Trim()} da pergunta {t.Pergunta?.Trim()} da DPS do agregado {objeto?.Titular?.Pessoa?.Nome?.Trim()} tem que ter no máximo 200 caracteres");

                               //        d.RuleFor(b => b.Complemento)
                               //         .MaximumLength(200).WithMessage(t => $"O complemento {t.Complemento?.Trim()} da pergunta {t.Pergunta?.Trim()} da DPS do agregado {objeto?.Titular?.Pessoa?.Nome?.Trim()} tem que ter no máximo 200 caracteres");
                               //    });
                               //});



                               //Pessoa Agregado
                               c.RuleFor(t => t.Pessoa)
                                .NotNull()
                                .DependentRules(() => {
                                    RuleForEach(t => t.Agregados).ChildRules(c =>
                                    {
                                        c.When(t => t.Pessoa != null, () => {
                                            c.RuleFor(t => t.Pessoa.Nome)
                                            .NotNull()
                                            .MinimumLength(5)
                                            .MaximumLength(40);

                                            c.RuleFor(t => t.Pessoa.Data_Nascimento.Date)
                                                .GreaterThan(DateTime.Today.Date.AddYears(-130))
                                                .LessThanOrEqualTo(DateTime.Today.Date);

                                            //TODO: Validar enum de sexo
                                            //c.RuleFor(t => t.Pessoa.Sexo)
                                            //    .NotEmpty().WithMessage(t => $"Sexo do agregado {t.Pessoa.Nome?.Trim()} não informado ou inválido");
                                            //.Must(TerSexoValido).WithMessage(t => $"Sexo do agregado {t.Pessoa.Nome?.Trim()} tem que ser MASCULINO OU FEMININO");

                                            //c.RuleFor(t => t.Pessoa.CPF)
                                            //    .NotEmpty().WithMessage(t => $"CPF do agregado {t.Pessoa.Nome?.Trim()} não informado")
                                            //    .Must(TerCpfValido).WithMessage(t => $"CPF {t.Pessoa.CPF} do agregado {t.Pessoa.Nome?.Trim()} inválido");

                                            //c.RuleFor(t => t.Pessoa.CPF_Proprio)
                                            //    .NotEmpty().WithMessage(t => $"Marcação de CPF Próprio do agregado {t.Pessoa.Nome?.Trim()} não informado")
                                            //    .Must(TerCPFProprioValido).WithMessage(t => $"Marcação de CPF Próprio do agregado {t.Pessoa.Nome?.Trim()} inválido");


                                            c.RuleFor(t => t.Pessoa.Estado_Civil)
                                                .NotEmpty();
                                            //.Must(TerEstadoCivilValido).WithMessage(t => $"Estado Civil do agregado {t.Pessoa.Nome?.Trim()} inválido");

                                            //Endereço Agregado
                                            c.When(e => e.Pessoa.Endereco != null, () => {
                                                c.RuleFor(t => t.Pessoa.Endereco.Cep)
                                                     .NotEmpty();
                                                //TODO: Validar somente números com uma expressao regular

                                                c.RuleFor(t => t.Pessoa.Endereco.Logradouro)
                                                               .NotEmpty()
                                                               .MinimumLength(1)
                                                               .MaximumLength(40);

                                                //RuleFor(t => t.Pessoa.Endereco.Numero)


                                                c.RuleFor(t => t.Pessoa.Endereco.Complemento)
                                                               .MinimumLength(1)
                                                               .MaximumLength(20);

                                                c.RuleFor(t => t.Pessoa.Endereco.Bairro)
                                                    .NotEmpty()
                                                    .MinimumLength(1)
                                                    .MaximumLength(20);

                                                c.RuleFor(t => t.Pessoa.Endereco.Cidade)
                                                    .NotEmpty()
                                                    .MinimumLength(1)
                                                    .MaximumLength(20);

                                                c.RuleFor(t => t.Pessoa.Endereco.UF)
                                                    .NotEmpty()
                                                    .Length(2);

                                                c.RuleFor(t => t.Pessoa.Endereco.Referencia)
                                                    .MinimumLength(1)
                                                    .MaximumLength(60);


                                                c.RuleFor(t => t.Pessoa.Endereco.Pais)
                                                    .NotEmpty();


                                            });
                                        });
                                    });
                                });

                           });

                });
            }

            //protected static bool TerSexoValido(string sexo)
            //{
            //    if (string.IsNullOrEmpty(sexo))
            //        return false;

            //    var lista = new List<string>() {
            //            "MASCULINO","FEMININO"};

            //    return lista.Contains(sexo.Trim().ToUpper());
            //}
            //protected static bool TerCPFProprioValido(string cpfProprio)
            //{
            //    if (string.IsNullOrEmpty(cpfProprio))
            //        return false;

            //    var lista = new List<string>() {
            //            "S","N"};

            //    return lista.Contains(cpfProprio.Trim().ToUpper());
            //}
            protected static bool TerCpfValido(string cpf)
            {
                if (string.IsNullOrEmpty(cpf))
                    return true;

                return Core.DomainObjects.Cpf.Validar(cpf);
            }
            //protected static bool TerParentescoValido(string parentesco)
            //{
            //    if (string.IsNullOrEmpty(parentesco))
            //        return false;

            //    var lista = new List<string>() { 
            //            "CUNHADO","CUNHADA","SOBRINHO","SOBRINHA","EMPREGADO","EMPREGADA","OUTROS","GENRO","NORA",
            //            "NETO","FILHO","BISNETO","ENTEADA","ENTEADO","AGREGADO","CÔNJUGE", "CÔNJUGE AGREGADO","FILHO AGREGADO",
            //            "FILHA AGREGADO","CURATELADO","TUTELADO","FILHA","PAI","MÃE","IRMÃO","IRMÃ","SOGRO","SOGRA","CONJUGE" };

            //    return lista.Contains(parentesco.Trim().ToUpper());
                
            //}
            protected static bool TerPercentualParticipacaoValido(List<BeneficiarioDTO> beneficiarios)
            {
                if (beneficiarios == null)
                    return true;

                if (beneficiarios.Count() > 0)
                    return beneficiarios.Sum(x => x.Porcentagem_Participacao) == 100;

                return false;

            }

           
            //protected static bool TerEstadoCivilValido(int? estadoCivil)
            //{
            //    if (estadoCivil == null)
            //        return false;

            //    var lista = new List<int>() {
            //            0, 1, 2, 3, 4 };

            //    return lista.Contains((int)estadoCivil);

            //}

            //protected static bool TerTipoTelefoneValido(int tipoTelefone)
            //{
               
            //    var lista = new List<int>() {
            //            0, 1, 2, 3};

            //    return lista.Contains((int)tipoTelefone);

            //}


        }

        
    }



    [SwaggerSchema(Required = new[] { "contrato", "proposta", "inicio_vigencia", "produto", "digital", "premio_total", "colaborador", "periodicidade", "meio_pagamento", "unidade", "data_venda"})]
    public class SeguroDTO
    {
        [SwaggerSchema(description: "contrato de seguro que foi realizado a venda")]
        [SwaggerSchemaExample("1631")]
        public int contrato { get; set; }

        [SwaggerSchema(description: "número da proposta que foi realizada a venda")]
        [SwaggerSchemaExample("111111")]
        public int proposta { get; set; }

        [SwaggerSchema(description: "Data de início da vigência do seguro (data da venda)", Format = "Date")]
        [SwaggerSchemaExample("2023-01-01")]
        public DateTime inicio_vigencia { get; set; }

        [SwaggerSchema(description: "Código do produto de seguro")]
        [SwaggerSchemaExample("9201")]
        public short produto { get; set; }

        [SwaggerSchema(description: "tipo da comunicação que a sinaf seguro terá com o segurado digital ou física")]
        public Digital digital { get; set; }

        [SwaggerSchema(description: "Valor total de prêmio da apólice. Esse valor consiste no somatório de prêmio de todos os segurados")]
        [SwaggerSchemaExample("50.00")]
        public decimal premio_total { get; set; }

        [SwaggerSchema(description: "Código do colaborador (cracha) que realizou a venda da proposta")]
        [SwaggerSchemaExample("1234")]
        public int colaborador { get; set; }

        [SwaggerSchema(description: "Periodicidade de pagamento do seguro")]
        public Periodicidade periodicidade { get; set; }

        [SwaggerSchema(description: "Informação pertinentes ao motivo e seguro que originou a venda administrativa")]
        public VendaAdministrativaDTO VendaAdministrativa { get; set; }

        [SwaggerSchema(description: "Lista dos beneficiários designados a receber as indenizações do seguro")]
        public List<BeneficiarioDTO>? Beneficiarios { get; set; }

        [SwaggerSchema(description: "Informação pertinentes a procedimento de transferência de pagamento de uma proposta origem para a proposta que está sendo transmitida")]
        public TransferenciaPagamentoDTO TransferenciaPagamento { get; set; }

        [SwaggerSchema(description: "Informação pertinentes o meio de pagamento")]
        public MeioPagamentoDTO MeioPagamento { get; set; }

        [SwaggerSchema(description: "Unidade que realizou a venda da proposta")]
        public string unidade { get; set; }

        [SwaggerSchema(description: "data da venda da proposta")]
        public DateTime data_venda { get; set; }

    }



    [SwaggerSchema(Required = new[] { "plano", "vigencia_Plano", "premio_Total", "coberturas", "pessoa" })]
    public class SeguradoDTO
    {

        [SwaggerSchema(description: "Código do plano contratado. A informação do plano é fornecidada em /api/v1/proposta/traduzir-plano")]
        [SwaggerSchemaExample("5")]
        public int Plano { get; set; }

        [SwaggerSchema(description: "Data da vigência do plano. A informação do plano é fornecidada em /api/v1/proposta/traduzir-plano")]
        [SwaggerSchemaExample("2022-10-01")]
        public DateTime Vigencia_Plano { get; set; }

        public int? Emissao { get; set; }

        [SwaggerSchema(description: "Somatório dos valores de prêmio líquido do segurado. Somatório do prêmio liquido de todas as coberturas do segurado")]
        [SwaggerSchemaExample("71.36")]
        public decimal Premio_Total { get; set; }

        [SwaggerSchema(description: "Quantidade de meses(parcelas mensais) que o beneficiário receberá a indenização")]
        [SwaggerSchemaExample("7")]
        public short meses_para_renda { get; set; }

        [SwaggerSchema(description: "Tipo de identificação do segurado")]
        public Tipo_Segurado Tipo_Segurado { get; set; }


        [SwaggerSchema(description: "Grau de parentesco do Dependente/Agregado em relação ao titular do seguro [Informação só precisa ser preenchida para dependentes e agregados]")]
        public Grau_de_Parentesco Grau_Parentesco { get; set; }


        public List<CoberturasDTO> Coberturas { get; set; }

        public List<DPSDTO> DPS { get; set; }

        public PessoaDTO Pessoa { get; set; }
    }
    //public class AgregadoDTO
    //{
    //    public int Plano { get; set; }
    //    public DateTime Vigencia_Plano { get; set; }
    //    public decimal Premio_Total { get; set; }
    //    public Tipo_Segurado Tipo_Agregado { get; set; }
    //    public Grau_de_Parentesco Grau_Parentesco { get; set; }
    //    public List<CoberturasDTO> Coberturas { get; set; }
    //    public List<DPSDTO> DPS { get; set; }
    //    public PessoaDTO Pessoa { get; set; }
    //}



    [SwaggerSchema(Required = new[] { "nome", "data_Nascimento", "sexo" })]
    public class PessoaDTO
    {
        [JsonIgnore]
        public Guid Id { get; set; }

        //[Required]
        //[StringLength(40, MinimumLength = 5)]
        [SwaggerSchema("Nome do segurado")]
        [SwaggerSchemaMinimumAndMaximum(5, 40)]
        public string Nome { get; set; }

        public int Cdpes { get; set; }

        
        public DateTime Data_Nascimento { get; set; }

        
        public Sexo Sexo { get; set; }

        [SwaggerSchema(description: "CPF. [Somente números]")]
        [SwaggerSchemaMinimumAndMaximum(maximum: 11)]
        public string CPF { get; set; }

        [SwaggerSchema(description: "Pessoa informada é a dona do CPF. Preenchimento obrigatório caso CPF seja fornecido")]
        public Cpf_Proprio? CPF_Proprio { get; set; }


        [SwaggerSchema(description: "Renda. [Obrigatório somente para o titular]")]
        public decimal? Renda { get; set; }


        [SwaggerSchema(description: "Atividade remunerada que o segurado exerce (profissão). A lista de atividades é fornecidada em /api/v1/pessoa/profissao. [Obrigatório somente para o titular]")]
        [StringLength(200)]
        public string Atividade { get; set; }


        [SwaggerSchema(description: "Estado Civil. [Obrigatório somente para o titular]")]
        public Estado_Civil? Estado_Civil { get; set; }


        [SwaggerSchemaMinimumAndMaximum(maximum: 40)]
        public string Email { get; set; }//{ get => Email; set { if (value != null) Email = value.Trim(); } }

        [SwaggerSchemaMinimumAndMaximum(maximum: 11)]
        [SwaggerSchema(description: "RG do segurado. Somente números. [Obrigatório somente para o titular]")]
        public string RG { get; set; }

        [SwaggerSchemaMinimumAndMaximum(maximum: 40)]
        [SwaggerSchema(description: "Órgão de expedição do RG. [Obrigatório somente para o titular e quando informado o RG]")]
        public string Orgao_Expedidor { get; set; }

        [SwaggerSchema(description: "data de expedição do RG. [Obrigatório somente para o titular e quando informado o RG]")]
        public DateTime Data_Expedicao { get; set; }

        public List<TelefoneDTO> Telefones { get; set; }

        public EnderecoDTO Endereco { get; set; }

        public PessoaDTO()
        {
            Id = Guid.NewGuid();
            Cdpes = 1;
        }
    }




    [SwaggerSchema(Required = new[] { "cep", "logradouro", "bairro", "cidade", "uf", "pais" })]
    public class EnderecoDTO
    {
        public string Cep { get; set; }//{ get => Cep; set { if (value != null) Cep = value.Trim().ToUpper(); } }

        [SwaggerSchemaMinimumAndMaximum(maximum: 40)]
        public string Logradouro { get; set; }//{ get => Logradouro; set { if (value != null) Logradouro = value.Trim().ToUpper(); } }


        public int? Numero { get; set; }//{ get => Numero; set { if (value != null) Numero = value.Trim().ToUpper(); } }

        [SwaggerSchemaMinimumAndMaximum(maximum: 20)]
        public string Complemento { get; set; }//{ get => Complemento; set { if (value != null) Complemento = value.Trim().ToUpper(); } }

        [SwaggerSchemaMinimumAndMaximum(maximum: 20)]
        public string Bairro { get; set; }//{ get => Bairro; set { if (value != null) Bairro = value.Trim().ToUpper(); } }

        [SwaggerSchemaMinimumAndMaximum(maximum: 20)]
        public string Cidade { get; set; }//{ get => Cidade; set { if (value != null) Cidade = value.Trim().ToUpper(); } }

        [SwaggerSchemaMinimumAndMaximum(maximum: 2)]
        public string UF { get; set; }//{ get => UF; set { if (value != null) UF = value.Trim().ToUpper(); } }

        [SwaggerSchemaMinimumAndMaximum(maximum: 60)]
        public string Referencia { get; set; }//{ get => Referencia; set { if (value != null) Referencia = value.Trim().ToUpper(); } }

        [SwaggerSchemaMinimumAndMaximum(maximum: 30)]
        public string Pais { get; set; }//{ get => Pais; set { if (value != null) Pais = value.Trim().ToUpper(); } }
    }



    [SwaggerSchema(Required = new[] { "cobertura", "is", "premio" })]
    public class CoberturasDTO
    {
        [SwaggerSchema(description: "Código da Cobertura Contratada")]
        [SwaggerSchemaExample("9101")]
        public int Cobertura { get; set; }

        [SwaggerSchema(description: "Valor da Importância Segurada (IS) da cobertura")]
        [SwaggerSchemaExample("15000.00")]
        public decimal IS { get; set; }

        [SwaggerSchema(description: "Valor da prêmio líquido cobrado pertinente a contratação da cobertura")]
        [SwaggerSchemaExample("11.32")]
        public decimal Premio { get; set; }
    }



    [SwaggerSchema(Required = new[] { "motivo", "contrato_Original", "certificado_Original"})]
    public class VendaAdministrativaDTO
    {
        [SwaggerSchema(description: "Motivo de origem da venda administrativa")]
        public Motivo_Venda_Administrativa Motivo { get; set; }

        [SwaggerSchema(description: "Contrato do seguro que originou a venda administrativa")]
        [SwaggerSchemaExample("1631")]
        public int Contrato_Original { get; set; }

        [SwaggerSchema(description: "Certificado que originou a venda administrativa")]
        [SwaggerSchemaExample("44455655")]
        public int Certificado_Original { get; set; }
    }

    [SwaggerSchema(Required = new[] { "proposta_origem", "valor_proposta_origem"})]
    public class TransferenciaPagamentoDTO
    {
        [SwaggerSchema(description: "Proposta que será aproveitada o pagamento para a transferência")]
        [SwaggerSchemaExample("123456")]
        public int proposta_origem { get; set; }

        [SwaggerSchema(description: "Valor de prêmio total")]
        [SwaggerSchemaExample("55.23")]
        public decimal valor_proposta_origem { get; set; }

    }


    [SwaggerSchema(Required = new[] { "meio_pagamento" })]
    public class MeioPagamentoDTO
    {
        [SwaggerSchema(description: "Forma principal que o segurado escolheu para efetuar o pagamento do seguro")]
        public Meio_Pagamento meio_pagamento { get; set; }

        [SwaggerSchema(description: "Informação do débito automático")]
        public DebitoAutomaticoDTO debito_automatico { get; set; }
    }


    [SwaggerSchema(Required = new[] { "agencia", "conta", "digitoConta", "tipo", "categoria", "titular", "cpfTitular" })]
    public class DebitoAutomaticoDTO
    {

        [SwaggerSchema(description: "Agência do banco")]
        [SwaggerSchemaExample("0445")]
        public string agencia { get; set; }

        [SwaggerSchema(description: "Dígito da Agência")]
        [SwaggerSchemaExample("6")]
        public string digito_agencia { get; set; }

        [SwaggerSchema(description: "Numero da conta")]
        [SwaggerSchemaExample("013999")]
        public string conta { get; set; }

        [SwaggerSchema(description: "Dígito da conta")]
        [SwaggerSchemaExample("6")]
        public string digito_conta { get; set; }

        [SwaggerSchema(description: "Tipo da conta")]
        [SwaggerSchemaExample("conta_corrente, conta_poupanca")]
         public string tipo { get; set; } //char

        [SwaggerSchema(description: "Categoria da conta")]
        [SwaggerSchemaExample("individual, conjunta")]
        public string categoria { get; set; } //char

        [SwaggerSchema(description: "Nome do titular da conta")]
        [SwaggerSchemaExample("Manoel Joaquim da silva")]
        public string titular { get; set; }

        [SwaggerSchema(description: "CPF do titular da conta")]
        [SwaggerSchemaExample("11111111111")]
        public string cpf_titular { get; set; }


        private readonly List<string> listaTipoConta = new List<string> { "conta_corrente", "conta_poupanca" };
        private readonly List<string> listaTipoCategoria = new List<string> { "individual", "conjunta" };



        public List<string> IsValid()
        {
            List<string> lista = new List<string>();
            if (string.IsNullOrEmpty(agencia)) lista.Add("Agência não pode ser zero");
            if (string.IsNullOrEmpty(digito_agencia)) lista.Add("digito da agência não informado");
            if (string.IsNullOrEmpty(conta)) lista.Add("conta não informada");
            if (string.IsNullOrEmpty(digito_conta)) lista.Add("digito da conta não informado");


            if (string.IsNullOrEmpty(tipo) || !listaTipoConta.Any(x => x == tipo)) lista.Add("tipo da conta não informado ou inválido. Somente os tipos " + String.Join(",", listaTipoConta) + " são permitidos.");
            if (string.IsNullOrEmpty(categoria) || !listaTipoCategoria.Any(x => x == categoria)) lista.Add("categoria da conta não informada ou inválida. Somente as categorias " + String.Join(",", listaTipoConta) + " são permitidas.");


            return lista;
        }
    }


    [SwaggerSchema(Required = new[] { "pergunta", "resposta", "complemento" })]
    public class DPSDTO
    {
        [SwaggerSchema(description: "Pergunda da DPS (GUID)")]
        //[SwaggerSchemaExample("Encontra-se trabalhando?")]
        public Guid Pergunta { get; set; }//{ get => Pergunta; set { if (value != null) Pergunta = value.Trim().ToUpper(); } }

        [SwaggerSchema(description: "Resposta do segurado para a pergunta (GUID)")]
        //[SwaggerSchemaMinimumAndMaximum(2, 200)]
        public Guid Resposta { get; set; }//{ get => Resposta; set { if (value != null) Resposta = value.Trim().ToUpper(); } }

        [SwaggerSchema(description: "Complemento da resposta (Campo texto livre)")]
        [SwaggerSchemaMinimumAndMaximum(maximum: 400)]
        //public Opcao_Resposta_DPS? Resposta { get; set; }//{ get => Resposta; set { if (value != null) Resposta = value.Trim().ToUpper(); } }
        public string Complemento { get; set; }//{ get => Complemento; set { if (value != null) Complemento = value.Trim().ToUpper(); } }
    }



    [SwaggerSchema(Required = new[] { "ddd", "numero", "tipo", "celular_Principal", "receber_SMS", "whatsapp" })]
    public class TelefoneDTO
    {
        [SwaggerSchema(description: "DDD do telefone")]
        [SwaggerSchemaExample("21")]
        [SwaggerSchemaMinimumAndMaximum(2, 2)]
        public int DDD { get; set; }

        [SwaggerSchema(description: "Número do telefone ou celular")]
        [SwaggerSchemaExample("999999999")]
        [SwaggerSchemaMinimumAndMaximum(8, 9)]
        public string Numero { get; set; }

        [SwaggerSchema(description: "Tipo do telefone")]
        public Tipo_Telefone Tipo { get; set; }

        [SwaggerSchema(description: "Identificação se é o celular principal")]
        public Celular_Principal? Celular_Principal { get; set; }

        [SwaggerSchema(description: "Permitido receber mensagem por SMS")]
        public Receber_SMS? Receber_SMS { get; set; }

        [SwaggerSchema(description: "Permitido receber mensagem por Whatsapp")]
        public Whastapp? Whatsapp { get; set; }
    }



    [SwaggerSchema(Required = new[] { "Nome", "parentesco", "porcentagem_Participacao" })]
    public class BeneficiarioDTO
    {
        //[StringLength(40)]
        [SwaggerSchemaMinimumAndMaximum(minimum: 5, maximum: 40)]
        public string Nome { get; set; }//{ get => Nome; set { if (value != null) Nome = value.Trim().ToUpper(); } }

        [SwaggerSchemaExample("1986-06-27")]
        public DateTime Data_Nascimento { get; set; }

        public Sexo Sexo { get; set; }//{ get => Sexo; set { if (value != null) Sexo = value.Trim().ToUpper(); } }

        [SwaggerSchemaMinimumAndMaximum(maximum: 11)]
        [SwaggerSchemaExample("11111111111")]
        public string CPF { get; set; }


        public Grau_de_Parentesco Parentesco { get; set; }//{ get => Parentesco; set { if (value != null) Parentesco = value.Trim().ToUpper(); } }


        [SwaggerSchema(description: "% que o beneficiário irá receber em relação ao valor total da indenização. A somatória do % de todos os beneficiário precisa ser igual a 100")]
        [SwaggerSchemaExample("55.30")]
        public decimal? Porcentagem_Participacao { get; set; }


        public List<TelefoneDTO> Telefones { get; set; }
    }





    public class validarDpsDTO
    {
        public short Produto { get; set; }
        public Tipo_Segurado TipoSegurado { get; set; }
        public Sexo Sexo { get; set; }
        public int Idade { get; set; }
        public List<int> Coberturas { get; set; }
        public List<DPSDTO> DPS { get; set; }
        public DateTime InicioVigencia { get; set; }
    }
    public class validarMeioPagamentoDTO
    {
        public int Proposta { get; set; }
        public Meio_Pagamento MeioPagamento { get; set; }

        public DebitoAutomaticoDTO debitoAutomatico { get; set; }
    }



    public class ValidarEnderecoHabilitadoVendaDTO
    {
        public string UF { get; set; }
        public int Empresa { get; set; }
        public int Sucursal { get; set; }
    }
    public class VerificarAgrupamentoAtivoDTO
    {
        public int Contrato { get; set; }
        public int? Agrupamento { get; set; }
        public DateTime DataNascimento { get; set; }
        public DateTime DataBase { get; set; }
        public Tipo_Segurado TipoSegurado { get; set; }
    }
    public class ValidarVendaAdministrativaDTO
    {
        public int Contrato { get; set; }
        public int ContratoOriginal { get; set; }
        public int CertificadoOriginal { get; set; }
        public Motivo_Venda_Administrativa Motivo { get; set; }

    }
    public class ValidarIdadeSeguradoDentroLimiteAceitacaoAgrupamentoDTO
    {
        public int Contrato { get; set; }
        public int Agrupamento { get; set; }
        public DateTime DataNascimento { get; set; }
    }
    public class VerificarDocumentoExistente
    {
        public string Nome { get; set; }
        public string Valor { get; set; }
    }
}
