using FluentValidation;
using FluentValidation.Results;
using Core.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ProcessarProposta.Worker.DTOs
{ 
    public class PropostaSeguroDTO
{
    public int Empresa { get; set; }
    public int Sucursal { get; set; }
    public string Usuario { get; set; }
    public Guid Origem { get; set; }
    public SeguroDTO Seguro { get; set; }
    public SeguradoDTO Titular { get; set; }
    public List<SeguradoDTO> Agregados { get; set; }

    public ValidationResult validationResult;

    public PropostaSeguroDTO()
    {
    }

    public bool EhValido()
    {
        var validation = new PropostaSeguroDTOValidation();
        validation.objeto = this;
        validationResult = validation.Validate(this);
        return validationResult.IsValid;
    }

    public class PropostaSeguroDTOValidation : AbstractValidator<PropostaSeguroDTO>
    {
        public PropostaSeguroDTO objeto;


        public PropostaSeguroDTOValidation()
        {

            //Root
            RuleFor(t => t.Empresa).GreaterThanOrEqualTo(1000).WithMessage(t => $"Empresa inválida");
            RuleFor(t => t.Sucursal).GreaterThanOrEqualTo(2000).WithMessage(t => $"Sucursal inválida");
            RuleFor(t => t.Usuario)
                .MinimumLength(4).WithMessage(t => $"Usuário tem que ter mais do que 4 caracteres")
                .MaximumLength(12).WithMessage(t => $"Usuário não pode ter ter mais do que 12 caracteres");
            //RuleFor(t => t.Origem).NotNull().WithMessage(t => $"Origem inválida ou não preenchida");
            //RuleFor(t => t.Origem).LessThan(4).WithMessage(t => $"Origem inválida");


            //Seguro
            RuleFor(t => t.Seguro).NotNull()
                .DependentRules(() => {
                    RuleFor(t => t.Seguro.proposta).GreaterThan(1).WithMessage(t => $"proposta não informada ou com valor inválido");
                    RuleFor(t => t.Seguro.contrato).GreaterThan(1).WithMessage(t => $"contrato não informado ou com valor inválido");
                        //TODO: DESCOMENTAR (COMENTADO SOMENTE PARA TESTE)
                        //RuleFor(t => t.Seguro.inicio_vigencia).GreaterThan(DateTime.Today.AddYears(-1)).WithMessage(t => $"Início da vigência não pode ser anterior a mais de 1 ano da data de hoje");
                        RuleFor(t => (int)t.Seguro.produto).GreaterThan(1).WithMessage(t => $"produto não informado ou com valor inválido");
                        //RuleFor(t => t.Seguro.digital).NotNull().WithMessage(t => $"Digital inválido ou não preenchido");
                        RuleFor(t => t.Seguro.premio_total)
                        .GreaterThan(0).WithMessage(t => $"Valor de prêmio total do seguro tem que ser maior do que R$ 0,00")
                        .ScalePrecision(2, 7).WithMessage(t => $"Valor de prêmio total com valor inválido. Só é permitido 2 casas decimais e ter no máximo 7 digitos. Ex.: R$ 99.999,99");

                    RuleFor(t => t.Seguro.colaborador).GreaterThan(0).WithMessage(t => $"colaborador não informado ou com valor inválido");
                        //RuleFor(t => t.Seguro.periodicidade.ToString()).Matches("[1,2,3,4,6]").WithMessage(t => $"A periodicidade só pode ser 1,2,3,4,6 ou 12")
                        //    .MaximumLength(2).WithMessage(t => $"A periodicidade só pode ser 1,2,3,4,6 ou 12");
                        //RuleFor(t => t.Seguro.periodicidade).NotNull().WithMessage(t => $"Periodicidade não inválida ou não preenchida");
                        //RuleFor(t => t.Seguro.periodicidade).NotEqual(11).WithMessage(t => $"A periodicidade só pode ser 1,2,3,4,6 ou 12");



                        //Beneficiários
                        RuleFor(t => t.Seguro.Beneficiarios)
                        .Must(TerPercentualParticipacaoValido).WithMessage(t => $"A somatória do Percentual de participação dos beneficiários tem que ser 100%. Não pode ser inferiro ou superior a 100%");

                    When(t => t.Seguro.Beneficiarios != null, () =>
                    {
                        RuleForEach(t => t.Seguro.Beneficiarios)
                                .ChildRules(c =>
                                {
                                    c.RuleFor(b => b.Nome)
                                        .NotNull().WithMessage(t => "Nome do beneficiário não informado")
                                        .MinimumLength(5).WithMessage(t => $"Nome do beneficiário {t.Nome.Trim()} não pode ter menor do que 5 caracteres")
                                        .MaximumLength(40).WithMessage(t => $"Nome do beneficiário {t.Nome.Trim()} tem que ter até 40 caracteres");

                                    c.RuleFor(b => b.Data_Nascimento.Date)
                                        .GreaterThanOrEqualTo(DateTime.Today.AddYears(-130).Date).WithMessage(t => $"Data de nascimento {t.Data_Nascimento.ToString("dd/MM/yyyy")} do beneficiário {t.Nome.Trim()} não pode ser menor do que {DateTime.Today.AddYears(-130).ToString("dd/MM/yyyy")}")
                                        .LessThanOrEqualTo(DateTime.Today.Date).WithMessage(t => $"Data de nascimento {t.Data_Nascimento.ToString("dd/MM/yyyy")} do beneficiário {t.Nome.Trim()} não pode ser maior do que a data de hoje {DateTime.Today.ToString("dd/MM/yyyy")}");
                                        //TODO: Validar enum de sexo
                                        //c.RuleFor(b => b.Sexo)
                                        //    .NotNull().WithMessage(t => $"Sexo do beneficiário {t.Nome.Trim()} não informado ou inválido");
                                        //.Must(TerSexoValido).WithMessage(t => $"Sexo do beneficiário {t.Nome.Trim()} tem que ser MASCULINO OU FEMININO");

                                        c.RuleFor(b => b.CPF)
                                        .NotEmpty().WithMessage(t => $"CPF do beneficiário {t.Nome.Trim()} não informado")
                                        .Must(TerCpfValido).WithMessage(t => $"CPF {t.CPF} do beneficiário {t.Nome.Trim()} inválido");

                                        //c.RuleFor(b => b.Parentesco)
                                        //    .NotNull().WithMessage(t => $"Parentesco do beneficiário {t.Nome.Trim()} não informado ou inválido");
                                        //.MinimumLength(5).WithMessage(t => $"Parentesco do beneficiário {t.Nome.Trim()} não pode ter menor do que 5 caracteres")
                                        //.Must(TerParentescoValido).WithMessage(t => $"Parentesco do beneficiário {t.Nome.Trim()} inválido");
                                        c.RuleFor(b => b.Porcentagem_Participacao)
                                        .NotNull().WithMessage(t => $"Percentual de participação do beneficiário {t.Nome.Trim()} não informado")
                                        .GreaterThan(0).WithMessage(t => $"Percentual de participação do beneficiário {t.Nome.Trim()} não pode ser menor ou igual a 0")
                                        .ScalePrecision(2, 5).WithMessage(t => $"Percentual de participação do beneficiário {t.Nome.Trim()} com valor inválido. Só é permitido 2 casas decimais e ter no máximo 5 digitos. Ex.: 100,00")
                                        .LessThanOrEqualTo(100).WithMessage(t => $"Percentual de participação do beneficiário {t.Nome.Trim()} não pode ser maior do que 100");
                                });
                    });

                    When(t => t.Seguro.meio_pagamento == Meio_Pagamento.debito_automatico, () =>
                    {
                        RuleFor(t => t.Seguro.periodicidade).Equal(Periodicidade.mensal).WithMessage(t => $"Meio de pagamento Débito automático só é habilitado para seguros com a periodicidade mensal");
                    });




                }).WithMessage(t => $"Não foi informado informações do seguro");


            //Titular
            RuleFor(t => t.Titular).NotNull()
                .DependentRules(() => {

                    RuleFor(t => t.Titular.Plano).GreaterThan(0).WithMessage(t => $"Plano do segurado {t.Titular?.Pessoa?.Nome} não informado ou com valor inválido");
                    RuleFor(t => t.Titular.Vigencia_Plano.Year).GreaterThan(2002).WithMessage(t => $"Início da vigência não pode ser anterior a 2002");
                    RuleFor(t => t.Titular.Premio_Total)
                        .GreaterThan(0).WithMessage(t => $"Valor de prêmio total do seguro tem que ser maior do que R$ 0,00")
                        .ScalePrecision(2, 7).WithMessage(t => $"Valor de prêmio total com valor inválido. Só é permitido 2 casas decimais e ter no máximo 7 digitos. Ex.: R$ 99.999,99");
                    RuleFor(t => (int)t.Titular.meses_para_renda).GreaterThanOrEqualTo(0).WithMessage(t => $"Meses para renda fora do limite aceitável. Precisa ser igual ou superior a 0 e inferior a 25")
                            .LessThanOrEqualTo(24).WithMessage(t => $"Meses para renda fora do limite aceitável. Precisa ser igual ou superior a 0 e inferior a 25");

                        //Coberturas Titular
                        RuleFor(t => t.Titular.Coberturas).NotNull()
                        .DependentRules(() =>
                        {
                            RuleForEach(t => t.Titular.Coberturas)
                                .ChildRules(c =>
                                {
                                    c.RuleFor(b => b.Cobertura)
                                        .GreaterThan(0).WithMessage(t => "Código da cobertura não informada para o segurado titular");

                                    c.RuleFor(t => t.IS)
                                        .GreaterThanOrEqualTo(0).WithMessage(t => $"Valor da IS da cobertura {t.Cobertura} do titular {objeto?.Titular?.Pessoa?.Nome?.Trim()} não pode ser negativo")
                                        .ScalePrecision(2, 9).WithMessage(t => $"Valor da IS da cobertura {t.Cobertura} com valor inválido. Só é permitido 2 casas decimais e ter no máximo 7 digitos. Ex.: R$ 9.999.999,99");

                                    c.RuleFor(t => t.Premio)
                                        .GreaterThanOrEqualTo(0).WithMessage(t => $"Valor do prêmio da cobertura {t.Cobertura} não pode ser negativo")
                                        .ScalePrecision(2, 7).WithMessage(t => $"Valor do prêmio da cobertura {t.Cobertura} com valor inválido. Só é permitido 2 casas decimais e ter no máximo 7 digitos. Ex.: R$ 99.999,99");
                                });
                        }).WithMessage(t => $"Coberturas do titular {t.Titular?.Pessoa?.Nome} não foram informadas.");


                        //DPS Titular
                        //RuleFor(t => t.Titular.DPS).NotNull()
                        //   .DependentRules(() =>
                        //   {
                        //       RuleForEach(t => t.Titular.DPS)
                        //           .ChildRules(c =>
                        //           {
                        //               c.RuleFor(b => b.Pergunta)
                        //                .NotNull().WithMessage(t => $"A descrição da Pergunta {t.Pergunta?.Trim()} da DPS do titular {objeto?.Titular?.Pessoa?.Nome?.Trim()} não foi informada")
                        //                .MinimumLength(3).WithMessage(t => $"A descrição da Pergunta {t.Pergunta?.Trim()} da DPS do titular {objeto?.Titular?.Pessoa?.Nome?.Trim()} tem que ter no mínimo 3 caracteres")
                        //                .MaximumLength(200).WithMessage(t => $"A descrição da Pergunta {t.Pergunta?.Trim()} da DPS do titular {objeto?.Titular?.Pessoa?.Nome?.Trim()} tem que ter no máximo 200 caracteres");

                        //               c.RuleFor(b => b.Resposta)
                        //                .NotNull().WithMessage(t => $"A resposta da pergunta {t.Pergunta?.Trim()} da DPS do titular {objeto?.Titular?.Pessoa?.Nome?.Trim()} não foi informada");
                        //                //.MinimumLength(3).WithMessage(t => $"A resposta {t.Resposta} da pergunta {t.Pergunta?.Trim()} da DPS do titular {objeto?.Titular?.Pessoa?.Nome?.Trim()} tem que ter no mínimo 3 caracteres")
                        //                //.MaximumLength(200).WithMessage(t => $"A resposta {t.Resposta} da pergunta {t.Pergunta?.Trim()} da DPS do titular {objeto?.Titular?.Pessoa?.Nome?.Trim()} tem que ter no máximo 200 caracteres");

                        //               c.RuleFor(b => b.Complemento)
                        //                .MaximumLength(200).WithMessage(t => $"O complemento {t.Complemento?.Trim()} da pergunta {t.Pergunta?.Trim()} da DPS do titular {objeto?.Titular?.Pessoa?.Nome?.Trim()} tem que ter no máximo 200 caracteres");
                        //           });
                        //   }).WithMessage(t => $"A DPS dp titular {t.Titular?.Pessoa?.Nome} não foi informada");


                        //Pessoa Titular
                        RuleFor(t => t.Titular.Pessoa).NotNull()
                        .DependentRules(() => {

                            RuleFor(t => t.Titular.Pessoa.Nome)
                                .NotNull().WithMessage(t => $"Nome do titular não informado")
                                .MinimumLength(5).WithMessage(t => $"Nome do titular {t.Titular.Pessoa.Nome?.Trim()} tem que ter mais do que 5 caracteres")
                                .MaximumLength(40).WithMessage(t => $"Nome do titular {t.Titular.Pessoa.Nome?.Trim()} tem que ter até 40 caracteres");

                            RuleFor(t => t.Titular.Pessoa.Data_Nascimento.Date)
                                .GreaterThan(DateTime.Today.Date.AddYears(-130)).WithMessage(t => $"Data de nascimento do titular {t.Titular.Pessoa.Nome?.Trim()} não pode ser menor do que {DateTime.Today.AddYears(-130).Date.ToString("dd/MM/yyyy")}")
                                .LessThanOrEqualTo(DateTime.Today.Date).WithMessage(t => $"Data de nascimento do titular {t.Titular.Pessoa.Nome?.Trim()} não pode ser maior do que hoje {DateTime.Today.Date.ToString("dd/MM/yyyy")}");

                                //TODO: Validar enum de sexo
                                //RuleFor(t => t.Titular.Pessoa.Sexo)
                                //    .NotEmpty().WithMessage(t => $"Sexo do titular {t.Titular.Pessoa.Nome?.Trim()} não informado ou inválido");
                                //.Must(TerSexoValido).WithMessage(t => $"Sexo do titular {t.Titular.Pessoa.Nome?.Trim()} tem que ser MASCULINO OU FEMININO");

                                RuleFor(t => t.Titular.Pessoa.CPF)
                                .NotEmpty().WithMessage(t => $"CPF do titular {t.Titular.Pessoa.Nome?.Trim()} não informado")
                                .Must(TerCpfValido).WithMessage(t => $"CPF {t.Titular.Pessoa.CPF} do titular {t.Titular.Pessoa.Nome?.Trim()} inválido");

                            RuleFor(t => t.Titular.Pessoa.CPF_Proprio)
                                .NotEmpty().WithMessage(t => $"Marcação de CPF Próprio do titular {t.Titular.Pessoa.Nome?.Trim()} não informado ou inválido");
                                //.Must(TerCPFProprioValido).WithMessage(t => $"Marcação de CPF Próprio do titular {t.Titular.Pessoa.Nome?.Trim()} inválido");

                                RuleFor(t => t.Titular.Pessoa.Renda)
                            .NotEmpty().WithMessage(t => $"Renda do titular {t.Titular.Pessoa.Nome?.Trim()} não informada")
                                .GreaterThan(0).WithMessage(t => $"Valor da renda do titular {t.Titular.Pessoa.Nome?.Trim()} tem que ser maior do que R$ 0,00")
                                .ScalePrecision(2, 9).WithMessage(t => $"Valor de prêmio total com valor inválido. Só é permitido 2 casas decimais e ter no máximo 9 digitos. Ex.: R$ 9.999.999,99");

                            RuleFor(t => t.Titular.Pessoa.Atividade)
                                .NotEmpty().WithMessage(t => $"Atividade do titular {t.Titular.Pessoa.Nome?.Trim()} não informada");
                                //.GreaterThanOrEqualTo(0).WithMessage(t => $"Atividade do titular {t.Titular.Pessoa.Nome?.Trim()} tem que ter código maior ou igual a 0");

                                RuleFor(t => t.Titular.Pessoa.Estado_Civil)
                                .NotEmpty().WithMessage(t => $"Estado Civil do titular {t.Titular.Pessoa.Nome?.Trim()} não informado ou inválido");
                                //.Must(TerEstadoCivilValido).WithMessage(t => $"Estado Civil do titular {t.Titular.Pessoa.Nome?.Trim()} inválido");


                                RuleFor(t => string.IsNullOrWhiteSpace(t.Titular.Pessoa.Email) ? null : t.Titular.Pessoa.Email.Trim())
                                .EmailAddress().WithMessage(t => $"Email do titular {t.Titular.Pessoa.Nome?.Trim()} inválido");

                            RuleFor(t => t.Titular.Pessoa.RG)
                                .NotEmpty().WithMessage(t => $"RG do titular {t.Titular.Pessoa.Nome?.Trim()} não informado");
                                //TODO: Validar somente números com uma expressao regular

                                RuleFor(t => t.Titular.Pessoa.Orgao_Expedidor)
                                 .NotEmpty().WithMessage(t => $"Órgão expedidor do titular {t.Titular.Pessoa.Nome?.Trim()} não informado");

                            RuleFor(t => t.Titular.Pessoa.Data_Expedicao.Date)
                                .GreaterThanOrEqualTo(t => t.Titular.Pessoa.Data_Nascimento).WithMessage(t => $"Data de expedição do RG do titular {t.Titular.Pessoa.Nome?.Trim()} não pode ser menor do que a data de nascimento {t.Titular.Pessoa.Data_Nascimento}")
                                .LessThanOrEqualTo(DateTime.Today.Date).WithMessage(t => $"Data de expedição do RG do titular {t.Titular.Pessoa.Nome?.Trim()} não pode ser maior do que hoje {DateTime.Today.Date}");


                                //Endereço Titular
                                RuleFor(t => t.Titular.Pessoa.Endereco).NotNull()
                                .DependentRules(() => {

                                    RuleFor(t => t.Titular.Pessoa.Endereco.Cep)
                                 .NotEmpty().WithMessage(t => $"Cep do endereço do titular não informado");
                                        //TODO: Validar somente números com uma expressao regular

                                        RuleFor(t => t.Titular.Pessoa.Endereco.Logradouro)
                                        .NotEmpty().WithMessage(t => $"Lograrouro do endereço do titular não informado")
                                        .MinimumLength(1).WithMessage(t => $"Lograrouro do endereço do titular tem que ter mais do que 1 caracter")
                                        .MaximumLength(40).WithMessage(t => $"Lograrouro do endereço do titular tem que ter até 40 caracteres");

                                        //RuleFor(t => t.Titular.Pessoa.Endereco.Numero)


                                        RuleFor(t => t.Titular.Pessoa.Endereco.Complemento)
                                        //    .MinimumLength(1).WithMessage(t => $"Complemento do endereço do titular tem que ter mais do que 1 caracter")
                                        .MaximumLength(20).WithMessage(t => $"Complemento do endereço do titular tem que ter até 20 caracteres");

                                    RuleFor(t => t.Titular.Pessoa.Endereco.Bairro)
                                        .NotEmpty().WithMessage(t => $"Bairro do endereço do titular não informado")
                                        .MinimumLength(1).WithMessage(t => $"Bairro do endereço do titular tem que ter mais do que 1 caracter")
                                        .MaximumLength(20).WithMessage(t => $"Bairro do endereço do titular tem que ter até 20 caracteres");

                                    RuleFor(t => t.Titular.Pessoa.Endereco.Cidade)
                                        .NotEmpty().WithMessage(t => $"Cidade do endereço do titular não informado")
                                        .MinimumLength(1).WithMessage(t => $"Cidade do endereço do titular tem que ter mais do que 1 caracter")
                                        .MaximumLength(20).WithMessage(t => $"Cidade do endereço do titular tem que ter até 20 caracteres");

                                    RuleFor(t => t.Titular.Pessoa.Endereco.UF)
                                        .NotEmpty().WithMessage(t => $"UF do endereço do titular não informado")
                                        .Length(2).WithMessage(t => $"UF do endereço do titular tem que ter 2 caracteres");

                                    RuleFor(t => t.Titular.Pessoa.Endereco.Referencia)
                                        //    .MinimumLength(1).WithMessage(t => $"Referência do endereço do titular tem que ter mais do que 1 caracter")
                                        .MaximumLength(60).WithMessage(t => $"Referência do endereço do titular tem que ter até 60 caracteres");


                                    RuleFor(t => t.Titular.Pessoa.Endereco.Pais)
                                        .NotEmpty().WithMessage(t => $"País do endereço do titular  não informado");
                                }).NotEmpty().WithMessage(t => $"Endereço do titular não informado");



                                //Telefone Titular
                                When(t => t.Titular.Pessoa.Telefones != null, () =>
                            {
                                RuleForEach(t => t.Titular.Pessoa.Telefones)
                                       .ChildRules(c =>
                                       {
                                           c.RuleFor(b => b.DDD)
                                            .GreaterThan(10).WithMessage(t => $"DDD do telefone do titular não foi informado ou inválido");


                                           c.RuleFor(b => b.Numero)
                                            .NotNull().WithMessage(t => $"Número do telefone do titular não foi informado")
                                            .MinimumLength(7).WithMessage(t => $"Número do telefone {t.Numero} do titular tem que ter no mínimo 7 dígitos")
                                            .MaximumLength(9).WithMessage(t => $"Número do telefone {t.Numero} do titular tem que ter no máximo 9 dígitos");
                                               //TODO: Validar somente números com uma expressao regular


                                               //c.RuleFor(b => b.Tipo)
                                               // .NotNull().WithMessage(t => $"Tipo do telefone {t.Numero} do titular não foi informado ou inválido");
                                               //.Must(TerTipoTelefoneValido).WithMessage(t => $"Tipo do telefone  {t.Numero} do titular está inválido");

                                               c.When(t => t.Tipo == Tipo_Telefone.celular, () =>
                                           {
                                               c.RuleFor(b => b.Celular_Principal)
                                                .NotNull().WithMessage(t => $"O campo Celular principal do Telefone {t.Numero} não infomado ou inválido");

                                               c.RuleFor(b => b.Receber_SMS)
                                                .NotNull().WithMessage(t => $"O campo Receber sms do Telefone {t.Numero} não infomado ou inválido");

                                               c.RuleFor(b => b.Whatsapp)
                                                .NotNull().WithMessage(t => $"O campo Whatsapp do Telefone {t.Numero} não infomado ou inválido");
                                           });

                                       });

                            });



                        }).WithMessage(t => $"Não foi informado nenhuma informação da pessoa do titular");


                }).WithMessage(t => $"Não foi informado nenhuma informação relacionada ao titular");


            //Agregados
            When(t => t.Agregados != null, () =>
            {
                RuleForEach(t => t.Agregados)
                       .ChildRules(c =>
                       {
                           c.RuleFor(t => t.Plano).GreaterThan(0).WithMessage(t => $"Plano do segurado {t.Pessoa?.Nome} não informado ou com valor inválido");
                           c.RuleFor(t => t.Vigencia_Plano.Year).GreaterThan(2002).WithMessage(t => $"Início da vigência não pode ser anterior a 2002");
                           c.RuleFor(t => t.Premio_Total)
                               .GreaterThanOrEqualTo(0).WithMessage(t => $"Valor de prêmio total do seguro não pode ser negativo")
                               .ScalePrecision(2, 7).WithMessage(t => $"Valor de prêmio total com valor inválido. Só é permitido 2 casas decimais e ter no máximo 7 digitos. Ex.: R$ 99.999,99");
                           c.RuleFor(t => (int)t.meses_para_renda).GreaterThanOrEqualTo(0).WithMessage(t => $"Meses para renda fora do limite aceitável. Precisa ser igual ou superior a 0 e inferior a 25")
                            .LessThanOrEqualTo(24).WithMessage(t => $"Meses para renda fora do limite aceitável. Precisa ser igual ou superior a 0 e inferior a 25");

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
                                                    .GreaterThan(0).WithMessage(t => "Código da cobertura não informada para alguma cobertura do agregado");

                                                co.RuleFor(t => t.IS)
                                                    .GreaterThanOrEqualTo(0).WithMessage(t => $"Valor da IS da cobertura {t.Cobertura} do agregado não pode ser negativo")
                                                    .ScalePrecision(2, 9).WithMessage(t => $"Valor da IS da cobertura {t.Cobertura} com valor inválido. Só é permitido 2 casas decimais e ter no máximo 7 digitos. Ex.: R$ 9.999.999,99");

                                                co.RuleFor(t => t.Premio)
                                                    .GreaterThanOrEqualTo(0).WithMessage(t => $"Valor do prêmio da cobertura {t.Cobertura} do agregado não pode ser negativo")
                                                    .ScalePrecision(2, 7).WithMessage(t => $"Valor do prêmio da cobertura {t.Cobertura} com valor inválido. Só é permitido 2 casas decimais e ter no máximo 7 digitos. Ex.: R$ 99.999,99");

                                            });
                                       });
                               }).WithMessage(t => $"Coberturas do agregado {t.Pessoa?.Nome} não foram informadas.");


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
                            .NotNull().WithMessage(p => "Informação da pessoa do agregado não foi informado")
                            .DependentRules(() => {
                                RuleForEach(t => t.Agregados).ChildRules(c =>
                                {
                                    c.When(t => t.Pessoa != null, () => {
                                        c.RuleFor(t => t.Pessoa.Nome)
                                            .NotNull().WithMessage(t => $"Nome do agregado não informado")
                                            .MinimumLength(5).WithMessage(t => $"Nome do agregado {t.Pessoa.Nome?.Trim()} tem que ter mais do que 5 caracteres")
                                            .MaximumLength(40).WithMessage(t => $"Nome do agregado {t.Pessoa.Nome?.Trim()} tem que ter até 40 caracteres");

                                        c.RuleFor(t => t.Pessoa.Data_Nascimento.Date)
                                            .GreaterThan(DateTime.Today.Date.AddYears(-130)).WithMessage(t => $"Data de nascimento do agregado {t.Pessoa.Nome?.Trim()} não pode ser menor do que {DateTime.Today.AddYears(-130).Date.ToString("dd/MM/yyyy")}")
                                            .LessThanOrEqualTo(DateTime.Today.Date).WithMessage(t => $"Data de nascimento do agregado {t.Pessoa.Nome?.Trim()} não pode ser maior do que hoje {DateTime.Today.Date.ToString("dd/MM/yyyy")}");

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
                                                .NotEmpty().WithMessage(t => $"Estado Civil do agregado {t.Pessoa.Nome?.Trim()} não informado ou inválido");
                                            //.Must(TerEstadoCivilValido).WithMessage(t => $"Estado Civil do agregado {t.Pessoa.Nome?.Trim()} inválido");

                                            //Endereço Agregado
                                            c.When(e => e.Pessoa.Endereco != null, () => {
                                            c.RuleFor(t => t.Pessoa.Endereco.Cep)
                                                     .NotEmpty().WithMessage(t => $"Cep do endereço do agregado não informado");
                                                //TODO: Validar somente números com uma expressao regular

                                                c.RuleFor(t => t.Pessoa.Endereco.Logradouro)
                                                               .NotEmpty().WithMessage(t => $"Lograrouro do endereço do agregado não informado")
                                                               .MinimumLength(1).WithMessage(t => $"Lograrouro do endereço do agregado tem que ter mais do que 1 caracter")
                                                               .MaximumLength(40).WithMessage(t => $"Lograrouro do endereço do agregado tem que ter até 40 caracteres");

                                                //RuleFor(t => t.Pessoa.Endereco.Numero)


                                                c.RuleFor(t => t.Pessoa.Endereco.Complemento)
                                                               .MinimumLength(1).WithMessage(t => $"Complemento do endereço do agregado tem que ter mais do que 1 caracter")
                                                               .MaximumLength(20).WithMessage(t => $"Complemento do endereço do agregado tem que ter até 20 caracteres");

                                            c.RuleFor(t => t.Pessoa.Endereco.Bairro)
                                                    .NotEmpty().WithMessage(t => $"Bairro do endereço do agregado não informado")
                                                    .MinimumLength(1).WithMessage(t => $"Bairro do endereço do agregado tem que ter mais do que 1 caracter")
                                                    .MaximumLength(20).WithMessage(t => $"Bairro do endereço do agregado tem que ter até 20 caracteres");

                                            c.RuleFor(t => t.Pessoa.Endereco.Cidade)
                                                    .NotEmpty().WithMessage(t => $"Cidade do endereço do agregado não informado")
                                                    .MinimumLength(1).WithMessage(t => $"Cidade do endereço do agregado tem que ter mais do que 1 caracter")
                                                    .MaximumLength(20).WithMessage(t => $"Cidade do endereço do agregado tem que ter até 20 caracteres");

                                            c.RuleFor(t => t.Pessoa.Endereco.UF)
                                                    .NotEmpty().WithMessage(t => $"UF do endereço do agregado não informado")
                                                    .Length(2).WithMessage(t => $"UF do endereço do agregado tem que ter 2 caracteres");

                                            c.RuleFor(t => t.Pessoa.Endereco.Referencia)
                                                    .MinimumLength(1).WithMessage(t => $"Referência do endereço do agregado tem que ter mais do que 1 caracter")
                                                    .MaximumLength(60).WithMessage(t => $"Referência do endereço do agregado tem que ter até 60 caracteres");


                                            c.RuleFor(t => t.Pessoa.Endereco.Pais)
                                                    .NotEmpty().WithMessage(t => $"País do endereço do agregado  não informado");


                                        });
                                    });
                                });
                            });

                       });

            });





        }

     
        protected static bool TerCpfValido(string cpf)
        {
            if (string.IsNullOrEmpty(cpf))
                return true;

            return Core.DomainObjects.Cpf.Validar(cpf);
        }
      
        protected static bool TerPercentualParticipacaoValido(List<BeneficiarioDTO> beneficiarios)
        {
            if (beneficiarios == null)
                return true;

            if (beneficiarios.Count() > 0)
                return beneficiarios.Sum(x => x.Porcentagem_Participacao) == 100;

            return false;

        }

    }


}

    public class SeguroDTO
    {
        public int contrato { get; set; }
        public int proposta { get; set; }
        public DateTime inicio_vigencia { get; set; }
        //public int? Agrupamento { get; set; }
        public short produto { get; set; }
        public Digital digital { get; set; }
        public decimal premio_total { get; set; }
        public int colaborador { get; set; }

        public Periodicidade periodicidade { get; set; }
        public Meio_Pagamento meio_pagamento { get; set; }

        public VendaAdministrativaDTO VendaAdministrativa { get; set; }
        public List<BeneficiarioDTO>? Beneficiarios { get; set; }
    }
    public class SeguradoDTO
    {

        public int Plano { get; set; }
        public DateTime Vigencia_Plano { get; set; }
        public int? Emissao { get; set; }
        public decimal Premio_Total { get; set; }
        public short meses_para_renda { get; set; }
        public Tipo_Segurado Tipo_Segurado { get; set; }
        public Grau_de_Parentesco Grau_Parentesco { get; set; }
        public List<CoberturasDTO> Coberturas { get; set; }
        public List<DPSDTO> DPS { get; set; }
        public PessoaDTO Pessoa { get; set; }
    }
    public class PessoaDTO
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
        public string Email { get; set; }
        public string RG { get; set; }
        public string Orgao_Expedidor { get; set; }
        public DateTime Data_Expedicao { get; set; }
        public List<TelefoneDTO> Telefones { get; set; }
        public EnderecoDTO Endereco { get; set; }

        public PessoaDTO()
        {
            Id = Guid.NewGuid();
            Cdpes = 1;
        }
    }
    public class EnderecoDTO
    {
        public string Cep { get; set; }
        public string Logradouro { get; set; }
        public int? Numero { get; set; }
        public string Complemento { get; set; }
        public string Bairro { get; set; }
        public string Cidade { get; set; }
        public string UF { get; set; }
        public string Referencia { get; set; }
        public string Pais { get; set; }
    }
    public class CoberturasDTO
    {
        public int Cobertura { get; set; }
        public decimal IS { get; set; }
        public decimal Premio { get; set; }
    }
    public class VendaAdministrativaDTO
    {
        public Motivo_Venda_Administrativa Motivo { get; set; }
        public int Contrato_Original { get; set; }
        public int Certificado_Original { get; set; }
    }
    public class DPSDTO
    {
        public Guid Pergunta { get; set; }
        public Guid Resposta { get; set; }
        public string Complemento { get; set; }
    }
    public class TelefoneDTO
    {
        public int DDD { get; set; }
        public string Numero { get; set; }
        public Tipo_Telefone Tipo { get; set; }
        public Celular_Principal? Celular_Principal { get; set; }
        public Receber_SMS? Receber_SMS { get; set; }
        public Whastapp? Whatsapp { get; set; }
    }
    public class BeneficiarioDTO
    {
        public string Nome { get; set; }
        public DateTime Data_Nascimento { get; set; }
        public Sexo? Sexo { get; set; }
        public string CPF { get; set; }
        public Grau_de_Parentesco Parentesco { get; set; }
        public decimal? Porcentagem_Participacao { get; set; }
        public List<TelefoneDTO> Telefones { get; set; }
    }







//public class validarDpsDTO
//{
//    public short Produto { get; set; }
//    public Tipo_Segurado TipoSegurado { get; set; }
//    public Sexo Sexo { get; set; }
//    public int Idade { get; set; }
//    public List<int> Coberturas { get; set; }
//    public List<DPSDTO> DPS { get; set; }
//}


//public class validarMeioPagamentoDTO
//{
//    public int Proposta { get; set; }
//    public Meio_Pagamento MeioPagamento { get; set; }
//}

























// DTO para validação



//public class ValidarEnderecoHabilitadoVendaDTO
//{
//    public string UF { get; set; }
//    public int Empresa { get; set; }
//    public int Sucursal { get; set; }
//}

//public class VerificarAgrupamentoAtivoDTO
//{
//    public int Contrato { get; set; }
//    public int? Agrupamento { get; set; }
//    public DateTime DataNascimento { get; set; }
//    public DateTime DataBase { get; set; }
//    public Tipo_Segurado TipoSegurado { get; set; }
//}



//public class ValidarVendaAdministrativaDTO
//{
//    public int Contrato { get; set; }
//    public int ContratoOriginal { get; set; }
//    public int CertificadoOriginal { get; set; }
//    public Motivo_Venda_Administrativa Motivo { get; set; }

//}

//public class ValidarIdadeSeguradoDentroLimiteAceitacaoAgrupamentoDTO
//{
//    public int Contrato { get; set; }
//    public int Agrupamento { get; set; }
//    public DateTime DataNascimento { get; set; }
//}

//public class VerificarDocumentoExistente
//{
//    public string Nome { get; set; }
//    public string Valor { get; set; }
//}
}
