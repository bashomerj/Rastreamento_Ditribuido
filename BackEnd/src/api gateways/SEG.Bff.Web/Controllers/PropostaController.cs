using AutoMapper;
using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
using SEG.Bff.Web.Attributes;
using SEG.Bff.Web.DTO;
using SEG.Bff.Web.DTO.Proposta;
using SEG.Bff.Web.Services;
using SEG.Core.Enum;
using SEG.Core.Messages.Integration;
using SEG.Core.Utils;
using SEG.MessageBus;
using SEG.WebAPI.Core.Controllers;
using Swashbuckle.AspNetCore.Annotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mime;
using System.Threading.Tasks;

namespace SEG.Bff.Web.Controllers
{
    [Route("api/v1/proposta")]
    [ApiController]
    public class PropostaController : MainController
    {
        private readonly ISeguroService _seguroService;
        private readonly IPessoaService _pessoaService;
        private readonly IMapper _mapper;
        private readonly IMessageBus _bus;
        //private readonly IInternalService _internalService;

        public PropostaController(ISeguroService seguroService, IPessoaService pessoaService,
                                IMapper mapper, IMessageBus bus/*, IInternalService internalService*/)
        {
            _seguroService = seguroService;
            _pessoaService = pessoaService;
            _mapper = mapper;
            _bus = bus;
        }


        /// <summary>
        /// Utilizado para transmitir uma proposta para processamento na seguradora
        /// </summary>
        /// /// <remarks>Utilize essa função para enviar propostas para o processo de aceitação do seguro. As propostas enviadas são validadas, e quando aceitas, colocadas na fila de processamento para execução do processo de aceitação do seguro. Esse processo de aceitação consiste em passar por várias análise que são feitas pela seguradora para identificar se o risco da proposta está dentro dos parametros aceitos pela seguradora.</remarks>
        /// <response code="200">Retorna o aceite da requisição para processamento da proposta. Proposta adicionada na fila de processamento. Após o processamento da mensagem na fila a reposta será enviada via webhook</response>
        /// <response code="400">Parâmetros inválidos</response>
        /// <param name="novaPropostaDTO">Json contendo as informações de contratação da proposta</param>
        /// <returns></returns>
        /// 


        [SwaggerOperation(summary: "Utilizado para transmitir uma proposta para processamento na seguradora", description: "Utilize essa função para enviar propostas para o processo de aceitação do seguro. As propostas enviadas são validadas, e quando aceitas, colocadas na fila de processamento para execução do processo de aceitação do seguro. Esse processo de aceitação consiste em passar por várias análise que são feitas pela seguradora para identificar se o risco da proposta está dentro dos parametros aceitos pela seguradora.")]
        [Consumes(MediaTypeNames.Application.Json)]
        [Produces(MediaTypeNames.Application.Json)]
        [SwaggerResponse(statusCode: 200, type: typeof(string), description: "Retorna o aceite da requisição para processamento da proposta. Proposta adicionada na fila de processamento. Após o processamento da mensagem na fila a reposta será enviada via webhook")]
        [SwaggerResponse(statusCode: 400, type: typeof(ValidationProblemDetails), description: "Parâmetros inválidos")]
        [HttpPost]
        [Route("transmitir")]
        public async Task<IActionResult> ValidarSeguroPessoa(NovaPropostaDTO novaPropostaDTO)
        {
            //Obtem o Id da requisição
            string idRequest = null;
            if (Request.Headers.TryGetValue("traceparent", out StringValues traceParent)) idRequest = traceParent.ToString().Split('-')[1];

            const int agrupamento = 1;

            if (!novaPropostaDTO.EhValido())
                return CustomResponse(novaPropostaDTO.validationResult);


            // Validando o contrato: verificando se é um Vida Individual
            // Crítica 01: contrato não existe na Empresa/Sucursal ou não está vigente
            // Crítica 03: A data de início de vigência do seguro não pode ser anterior ao início de vigência do contrato

            AdicionarErroProcessamento(await _seguroService.ValidarVidaIndividual(new
            {
                Contrato = novaPropostaDTO.Seguro.contrato,
                Data = novaPropostaDTO.Seguro.inicio_vigencia,
                Sucursal = novaPropostaDTO.Sucursal,
                Empresa = novaPropostaDTO.Empresa
            }));


            // Crítica 02: proposta não existe na Empresa/Sucursal ou não está LIBERADA

            AdicionarErroProcessamento(await _seguroService.ValidarPropostaExisteEmpresaSucursalPendenteOuLiberada(new
            {
                Contrato = novaPropostaDTO.Seguro.contrato,
                Proposta = novaPropostaDTO.Seguro.proposta,
                Sucursal = novaPropostaDTO.Sucursal
            }));


            // Verificar se o endereço está habilitado para a venda
            // Crítica 05: 'inicioVigenciaEmissao', 'A UF de endereço do cliente não é habilitada para venda nessa empresa/sucursal'
            if (novaPropostaDTO.Titular.Pessoa.Endereco == null)
                AdicionarErroProcessamento("error_id: endereco_cobranca_nao_informado - Endereço de cobrança não informado!");
            else
                AdicionarErroProcessamento(await _seguroService.ValidarEnderecoHabilitadoVenda(new ValidarEnderecoHabilitadoVendaDTO()
                {
                    Empresa = novaPropostaDTO.Empresa,
                    UF = novaPropostaDTO.Titular.Pessoa.Endereco.UF,
                    Sucursal = novaPropostaDTO.Sucursal
                }));


            // Crítica 09: 'idade', 'Idade do segurado fora do limite de aceitação do agrupamento'
            // Crítica 06: Agrupamento do contrato não existe ou não está ativo
            AdicionarErroProcessamento(await _seguroService.VerificarAgrupamentoAtivo(new VerificarAgrupamentoAtivoDTO()
            {
                Contrato = novaPropostaDTO.Seguro.contrato,
                Agrupamento = agrupamento,
                DataBase = novaPropostaDTO.Seguro.inicio_vigencia,
                DataNascimento = novaPropostaDTO.Titular.Pessoa.Data_Nascimento,
                TipoSegurado = Tipo_Segurado.titular
            }));
            foreach (var agregado in novaPropostaDTO.Agregados ?? Enumerable.Empty<SeguradoDTO>())
                {
                    AdicionarErroProcessamento(await _seguroService.VerificarAgrupamentoAtivo(new VerificarAgrupamentoAtivoDTO()
                    {
                        Contrato = novaPropostaDTO.Seguro.contrato,
                        Agrupamento = agrupamento,
                        DataBase = novaPropostaDTO.Seguro.inicio_vigencia,
                        DataNascimento = agregado.Pessoa.Data_Nascimento,
                        TipoSegurado = agregado.Tipo_Segurado
                    }));
                }
            


            // Crítica: 07 'cobertura', 'Cobertura informada com valor de IS negativo'
            if (novaPropostaDTO.Titular.Coberturas == null)
                AdicionarErroProcessamento($"error_id: titular_cobertura_nao_informada - Nenhuma cobertura informada para o titular");
            else
            {
                novaPropostaDTO.Titular.Coberturas.Where(c => c.IS < 0).ToList().ForEach(delegate (CoberturasDTO item)
                {
                    AdicionarErroProcessamento($"error_id: cobertura_is_negativo - Cobertura {item.Cobertura} com valor de IS negativo");
                });


                //AdicionarErroProcessamento(await _produtosService
                //        .VerificarSeListaCoberturaExiste(novaPropostaDTO
                //        .Titular
                //        .Coberturas
                //        .Where(c => c.IS >= 0)
                //        .Select(c => c.Cobertura).ToList()));
            }

            // Crítica 08: @vendaAdministrativa = 'S'
            // Verificando se trata-se de uma venda administrativa e em caso positivo, validando os campos
            if (novaPropostaDTO.Seguro.VendaAdministrativa != null)
            {
                AdicionarErroProcessamento(await _seguroService.ValidarVendaAdministrativa(
                new ValidarVendaAdministrativaDTO()
                {
                    CertificadoOriginal = novaPropostaDTO.Seguro.VendaAdministrativa.Certificado_Original,
                    ContratoOriginal = novaPropostaDTO.Seguro.VendaAdministrativa.Contrato_Original,
                    Motivo = novaPropostaDTO.Seguro.VendaAdministrativa.Motivo
                }));
            }



            // Validar transferência de pagamento
            if (novaPropostaDTO.Seguro.TransferenciaPagamento != null)
            {
                AdicionarErroProcessamento(await _seguroService.ValidarTransferenciaPagamento(
               new TransferenciaPagamentoPropostaValidacaoDTO
               {
                   proposta_origem = novaPropostaDTO.Seguro.TransferenciaPagamento.proposta_origem,
                   proposta_destino = novaPropostaDTO.Seguro.proposta,
                   valor_premio_origem = novaPropostaDTO.Seguro.TransferenciaPagamento.valor_proposta_origem,
                   valor_premio_destino = novaPropostaDTO.Seguro.premio_total,
                   meio_pagamento_proposta_destino = novaPropostaDTO.Seguro.MeioPagamento.meio_pagamento
               }));
                
            }


            if (novaPropostaDTO.Seguro.MeioPagamento.meio_pagamento == Meio_Pagamento.debito_automatico)
            { 
                AdicionarErroProcessamento(await _seguroService.ValidarDebitoAutomatico(
                new DebitoAutomaticoDTO
                {
                    agencia = novaPropostaDTO.Seguro.MeioPagamento.debito_automatico.agencia,
                    digito_agencia = novaPropostaDTO.Seguro.MeioPagamento.debito_automatico.digito_agencia,
                    conta = novaPropostaDTO.Seguro.MeioPagamento.debito_automatico.conta,
                    digito_conta = novaPropostaDTO.Seguro.MeioPagamento.debito_automatico.digito_conta,
                    tipo = novaPropostaDTO.Seguro.MeioPagamento.debito_automatico.tipo,
                    categoria = novaPropostaDTO.Seguro.MeioPagamento.debito_automatico.categoria,
                    titular = novaPropostaDTO.Seguro.MeioPagamento.debito_automatico.titular,
                    cpf_titular = novaPropostaDTO.Seguro.MeioPagamento.debito_automatico.cpf_titular
                    
                }));
            }

            // Crítica 10: 'crahaColaborador', 'colaborador não encontrado ou não está ativo'
            // Verificando se o colaborador está ativo no sistema
            AdicionarErroProcessamento(await _pessoaService.ValidarColaborador(novaPropostaDTO.Seguro.colaborador));



            // =======================================================
            //  Valida proposta se permite processamento
            // =======================================================
            var responseCertificado = await _seguroService.CertificadoPorProposta(novaPropostaDTO.Empresa, novaPropostaDTO.Sucursal, novaPropostaDTO.Seguro.contrato, novaPropostaDTO.Seguro.proposta);
            if (responseCertificado.Errors.Mensagens.Any())
                AdicionarErroProcessamento(responseCertificado.Errors.Mensagens);
            else if (responseCertificado.Status != (int)HttpStatusCode.NotFound)
            {
                var certificado = responseCertificado.ObterResponseObject<CertificadoPorPropostaDTO>();

                if ((new List<int> { 1, 2, 3, 7, 9, 10 }).Contains(certificado.situacao))
                    AdicionarErroProcessamento($"error_id: proposta_processada_anteriormente - A proposta informada já foi processada anteriormente e existe um certificado vinculado. Contrato: {certificado.contrato} - Certificado: {certificado.certificado}");

                if (certificado.situacao == 5 && certificado.permite_redigitacao == "N")
                    AdicionarErroProcessamento($"error_id: proposta_recusada_nao_marcada_redigitacao - A proposta informada está recusada mas não está marcada para redigitação. Para redigitar a proposta a mesma precisa ter sido recusada para redigitação.");

            }



            // =======================================================
            //  Validações para o Cadastro do cliente
            // =======================================================

            List<CadastrarPessoaSeguradoDTO> listaSeguradosValidacao = new List<CadastrarPessoaSeguradoDTO>();


            //Titular
            listaSeguradosValidacao.Add(new CadastrarPessoaSeguradoDTO()
            {
                TipoSegurado = Tipo_Segurado.titular,
                Id = novaPropostaDTO.Titular.Pessoa.Id, //Guid
                CodigoUsuario = novaPropostaDTO.Usuario,

                Nome = novaPropostaDTO.Titular.Pessoa.Nome,
                DataNascimento = novaPropostaDTO.Titular.Pessoa.Data_Nascimento,
                Sexo = (Sexo)novaPropostaDTO.Titular.Pessoa.Sexo,
                CPF = novaPropostaDTO.Titular.Pessoa.CPF,
                CPFProprio = (Cpf_Proprio)novaPropostaDTO.Titular.Pessoa.CPF_Proprio,
                Renda = novaPropostaDTO.Titular.Pessoa.Renda,
                Atividade = novaPropostaDTO.Titular.Pessoa.Atividade,
                EstadoCivil = novaPropostaDTO.Titular.Pessoa.Estado_Civil,
                RG = novaPropostaDTO.Titular.Pessoa.RG,
                OrgaoExpedidor = novaPropostaDTO.Titular.Pessoa.Orgao_Expedidor,
                DataExpedicao = novaPropostaDTO.Titular.Pessoa.Data_Expedicao,
                Email = novaPropostaDTO.Titular.Pessoa.Email,
                DataVigenciaSeguro = novaPropostaDTO.Seguro.inicio_vigencia,
                Telefone = novaPropostaDTO.Titular.Pessoa.Telefones,
                Endereco = novaPropostaDTO.Titular.Pessoa.Endereco,


                CodigoEmpresa = novaPropostaDTO.Empresa,
                CodigoSucursal = novaPropostaDTO.Sucursal
            });

            //Agregados
            if (novaPropostaDTO.Agregados != null)
            {
                novaPropostaDTO.Agregados.ForEach(delegate (SeguradoDTO Agregado)
                {
                    listaSeguradosValidacao.Add(new CadastrarPessoaSeguradoDTO()
                    {
                        TipoSegurado = Agregado.Tipo_Segurado,
                        Id = Agregado.Pessoa.Id, //Guid
                        CodigoUsuario = novaPropostaDTO.Usuario,

                        Nome = Agregado.Pessoa.Nome,
                        DataNascimento = Agregado.Pessoa.Data_Nascimento,
                        Sexo = (Sexo)Agregado.Pessoa.Sexo,
                        CPF = Agregado.Pessoa.CPF,
                        CPFProprio = (Cpf_Proprio?)Agregado.Pessoa.CPF_Proprio,
                        Renda = Agregado.Pessoa.Renda,
                        Atividade = Agregado.Pessoa.Atividade,
                        EstadoCivil = Agregado.Pessoa.Estado_Civil,
                        Email = Agregado.Pessoa.Email,
                        RG = Agregado.Pessoa.RG,
                        OrgaoExpedidor = Agregado.Pessoa.Orgao_Expedidor,
                        DataExpedicao = Agregado.Pessoa.Data_Expedicao,
                        DataVigenciaSeguro = novaPropostaDTO.Seguro.inicio_vigencia,
                        Telefone = Agregado.Pessoa.Telefones,
                        Endereco = Agregado.Pessoa.Endereco,
                       
                        CodigoEmpresa = novaPropostaDTO.Empresa,
                        CodigoSucursal = novaPropostaDTO.Sucursal
                    });
                });
            }
            AdicionarErroProcessamento(await _pessoaService.ValidarInclusaoPessoa(listaSeguradosValidacao));


            //Validação dos beneficiários
            AdicionarErroProcessamento(await _pessoaService.ValidarInclusaoListaBeneficiario(novaPropostaDTO.Seguro.Beneficiarios));



            // =======================================================
            //  Validação plano
            // =======================================================
            var planoCoberturaDTO = new PlanoCoberturaDTO()
            {
                Agrupamento = agrupamento,
                Produto = novaPropostaDTO.Seguro.produto,
                SequencialPlano = novaPropostaDTO.Titular.Plano,
                DataVigencia = novaPropostaDTO.Titular.Vigencia_Plano,
                Sexo = novaPropostaDTO.Titular.Pessoa.Sexo,
                TipoSegurado = Tipo_Segurado.titular,
                PeriodicidadePagamento = novaPropostaDTO.Seguro.periodicidade,
                Idade = Util.CalcularIdade(novaPropostaDTO.Seguro.inicio_vigencia, novaPropostaDTO.Titular.Pessoa.Data_Nascimento),
                Coberturas = novaPropostaDTO.Titular.Coberturas != null ? novaPropostaDTO.Titular.Coberturas.Select(c => new CoberturaDTO()
                {
                    CodigoCoberura = c.Cobertura,
                    Premio = c.Premio,
                    IS = c.IS
                }).ToList() : null
            };
            AdicionarErroProcessamento(await _seguroService.ValidarPlanoSegurado(planoCoberturaDTO));
            foreach (var agregado in novaPropostaDTO.Agregados ?? Enumerable.Empty<SeguradoDTO>())
            {
                var AgregadoplanoCoberturaDTO = new PlanoCoberturaDTO()
                {
                    Agrupamento = agrupamento,
                    Produto = novaPropostaDTO.Seguro.produto,
                    SequencialPlano = agregado.Plano,
                    DataVigencia = agregado.Vigencia_Plano,
                    Sexo = agregado.Pessoa.Sexo,
                    TipoSegurado = agregado.Tipo_Segurado,
                    //TipoSegurado = Enum.GetName(agregado.Tipo_Agregado.GetType(), agregado.Tipo_Agregado),
                    PeriodicidadePagamento = novaPropostaDTO.Seguro.periodicidade,
                    Idade = Util.CalcularIdade(novaPropostaDTO.Seguro.inicio_vigencia, agregado.Pessoa.Data_Nascimento),
                    Coberturas = agregado.Coberturas != null ? agregado.Coberturas.Select(c => new CoberturaDTO()
                    {
                        CodigoCoberura = c.Cobertura,
                        Premio = c.Premio,
                        IS = c.IS
                    }).ToList() : null
                };
                AdicionarErroProcessamento(await _seguroService.ValidarPlanoSegurado(AgregadoplanoCoberturaDTO));
            }
        


            //Valida DPS titular
            AdicionarErroProcessamento(await _seguroService.ValidarDPS(new validarDpsDTO
            {
                Produto = novaPropostaDTO.Seguro.produto,
                Sexo = novaPropostaDTO.Titular.Pessoa.Sexo,
                Idade = Util.CalcularIdade(novaPropostaDTO.Seguro.inicio_vigencia, novaPropostaDTO.Titular.Pessoa.Data_Nascimento),
                TipoSegurado = novaPropostaDTO.Titular.Tipo_Segurado,
                Coberturas = novaPropostaDTO.Titular.Coberturas.Select(c => c.Cobertura).ToList(),
                DPS = novaPropostaDTO.Titular.DPS,
                InicioVigencia = novaPropostaDTO.Seguro.inicio_vigencia
            }));
            foreach (var agregado in novaPropostaDTO.Agregados ?? Enumerable.Empty<SeguradoDTO>())
            {
                AdicionarErroProcessamento(await _seguroService.ValidarDPS(new validarDpsDTO
                {
                    Produto = novaPropostaDTO.Seguro.produto,
                    Sexo = agregado.Pessoa.Sexo,
                    Idade = Util.CalcularIdade(novaPropostaDTO.Seguro.inicio_vigencia, agregado.Pessoa.Data_Nascimento),
                    TipoSegurado = agregado.Tipo_Segurado,
                    Coberturas = agregado.Coberturas.Select(c => c.Cobertura).ToList(),
                    DPS = agregado.DPS                    ,
                    InicioVigencia = novaPropostaDTO.Seguro.inicio_vigencia
                }));
            }


            //Validar Dados Meio de Cobrança
            AdicionarErroProcessamento(await _seguroService.ValidarMeioPagamento(new validarMeioPagamentoDTO
            {
                Proposta = novaPropostaDTO.Seguro.proposta,
                MeioPagamento = novaPropostaDTO.Seguro.MeioPagamento.meio_pagamento,
                debitoAutomatico = novaPropostaDTO.Seguro.MeioPagamento.meio_pagamento == Meio_Pagamento.debito_automatico ? novaPropostaDTO.Seguro.MeioPagamento.debito_automatico : null
            }));


            //Caso encontre erros. Retorna os erros encontrados e não coloca na fila para processamento
            if (!this.OperacaoValida())
                return CustomResponse();



            //TODO: Realizar o protocolo da proposta para liberado
            var response = await _seguroService.ObterControleProposta(novaPropostaDTO.Empresa, novaPropostaDTO.Sucursal, novaPropostaDTO.Seguro.proposta);
            var controleProposta = response.ObterResponseObject<ControlePropostaDTO>();
            if (controleProposta == null) AdicionarErroProcessamento(response.Errors.Mensagens);
            else
                //Protocola a proposta caso a mesma ainda não tenha sido protocolada
                if (controleProposta.Stprop == 4)
                    AdicionarErroProcessamento(await _seguroService.AlterarProposta(new PropostaDTO()
                    {
                        Empresa = novaPropostaDTO.Empresa,
                        Sucursal = novaPropostaDTO.Sucursal,
                        proposta = novaPropostaDTO.Seguro.proposta,
                        contrato = novaPropostaDTO.Seguro.contrato,
                        unidade = novaPropostaDTO.Seguro.unidade,
                        dataVenda = novaPropostaDTO.Seguro.data_venda,
                        valorPremio = novaPropostaDTO.Seguro.premio_total,
                        dataVencimento = novaPropostaDTO.Seguro.inicio_vigencia,
                        colaborador = novaPropostaDTO.Seguro.colaborador,
                        tipoCobranca = novaPropostaDTO.Seguro.MeioPagamento.meio_pagamento.ToString(),
                        situacao = 2
                    }));


            //Caso encontre erros na liberação da proposta. Retorna os erros encontrados e não coloca na fila para processamento
            if (!this.OperacaoValida())
                return CustomResponse();


            //Publicar na Fila para processamento
            var propostaValidadaEvent = _mapper.Map<PropostaValidadaIntegrationEvent>(novaPropostaDTO);
            propostaValidadaEvent.AtribuirAggregateRoot(idRequest);
            //propostaValidadaEvent.AtribuirAggregateRoot(Guid.NewGuid());
            //propostaValidadaEvent.AtribuirAggregateRoot(new { proposta = novaPropostaDTO.Seguro.proposta });
            await _bus.PublishAsync<PropostaValidadaIntegrationEvent>(propostaValidadaEvent);


            return CustomResponse("Adicionado na fila de processamento");
        }


        [SwaggerOperation(summary: "Processo para liberar uma proposta", description: "Utilizado para realizar o processo de liberação de uma proposta. Esse processo é necessário ser executado antes da transmissão de uma proposta para a seguradora. Nesse processo, algumas informações sobre o meio de cobrança da proposta serão informados para serem aproveitados pelo processo de aceitação de riscos da seguradora e criação do seguro")]
        [Consumes(MediaTypeNames.Application.Json)]
        [Produces(MediaTypeNames.Application.Json)]
        [SwaggerResponse(statusCode: 200, description: "Processo OK")]
        [SwaggerResponse(statusCode: 400, type: typeof(ValidationProblemDetails), description: "Parâmetros inválidos")]
        [HttpPost]
        [Route("liberar")]
        public async Task<IActionResult> Liberar(PropostaLiberadaDTO proposta)
        {
            AdicionarErroProcessamento(await _seguroService.AlterarProposta(new PropostaDTO()
            {
                Empresa = proposta.Empresa,
                Sucursal = proposta.Sucursal,
                proposta = proposta.proposta,
                contrato = proposta.contrato,
                unidade = proposta.unidade,
                dataVenda = proposta.dataVenda,
                valorPremio = proposta.valorPremio,
                dataVencimento = proposta.dataVencimento,
                colaborador = proposta.colaborador,
                tipoCobranca = proposta.tipoCobranca,
                situacao = 2
            }));
            return CustomResponse();
        }


        [SwaggerOperation(summary: "Processo para anular uma proposta", description: "Utilizado para realizar o processo de anulação de uma proposta. Depois de anulada a proposta a mesma não aceitará mudança para outra situação.")]
        [Consumes(MediaTypeNames.Application.Json)]
        [Produces(MediaTypeNames.Application.Json)]
        [SwaggerResponse(statusCode: 200, description: "Processo OK")]
        [SwaggerResponse(statusCode: 400, type: typeof(ValidationProblemDetails), description: "Parâmetros inválidos")]
        [HttpPost]
        [Route("anular")]
        public async Task<IActionResult> Anular(PropostaAnuladaDTO proposta)
        {
            AdicionarErroProcessamento(await _seguroService.AlterarProposta(new PropostaDTO()
            {
                Empresa = proposta.Empresa,
                Sucursal = proposta.Sucursal,
                proposta = proposta.proposta,
                unidade = proposta.unidade,
                dataVencimento = proposta.dataVencimento,
                dataVenda = proposta.dataVenda,
                valorPremio = proposta.valorPremio,
                motivo = proposta.motivo,
                situacao = 1
            }));
            return CustomResponse();
        }


        [SwaggerOperation(summary: "Processo para inutilizar uma proposta", description: "Utilizado para realizar o processo de inutilização de uma proposta. Depois de inutilizada a proposta a mesma não aceitará mudança para outra situação.")]
        [Consumes(MediaTypeNames.Application.Json)]
        [Produces(MediaTypeNames.Application.Json)]
        [SwaggerResponse(statusCode: 200, description: "Processo OK")]
        [SwaggerResponse(statusCode: 400, type: typeof(ValidationProblemDetails), description: "Parâmetros inválidos")]
        [HttpPost]
        [Route("inutilizar")]
        public async Task<IActionResult> Inutilizar(PropostaInutilizadaDTO proposta)
        {
            

            AdicionarErroProcessamento(await _seguroService.AlterarProposta(new PropostaDTO()
            {
                Empresa = proposta.Empresa,
                Sucursal = proposta.Sucursal,
                proposta = proposta.proposta,
                unidade = proposta.unidade,
                dataVencimento = proposta.dataVencimento,
                dataVenda = proposta.dataVenda,
                valorPremio = proposta.valorPremio,
                situacao = 3
            }));
            return CustomResponse();
        }


        [SwaggerOperation(summary: "Utilizado para identificar o plano de acordo acordo com as coberturas contratadas", 
            description: "Identifar qual o plano foi selecionado de acordo com as coberturas contratadas para cada tipo de segurado.")]
        [Consumes(MediaTypeNames.Application.Json)]
        [Produces(MediaTypeNames.Application.Json)]
        [SwaggerResponse(statusCode: 200, type: typeof(TraducaoPlanoDTO), description: "Retorna o plano de contratado")]
        [SwaggerResponse(statusCode: 400, type: typeof(ValidationProblemDetails), description: "Parâmetros inválidos. Pode acontecer também por ter sido encontrado mais do que um plano com as mesma configuração informada. Essa informação estará na resposta")]
        [SwaggerResponse(statusCode: 404, type: typeof(ValidationProblemDetails), description: "Plano não encontrado")]
        [HttpGet]
        [Route("traduzir-plano")]
        public async Task<IActionResult> TraduzirPlano(
            [SwaggerParameter(Required = true)]
            int produto,
            [SwaggerParameter(Required = true)]
            DateTime inicio_vigencia,
            [SwaggerParameter(Required = true)]
            Tipo_Segurado tipo_segurado,
            [SwaggerParameter(Required = true)]
            Sexo sexo,
            [SwaggerParameter(Required = true)]
            int idade,
            [SwaggerParameter(Required = true)]
            [FromQuery] List<int> lista_coberturas)
        {

            var response = await _seguroService.TraduzirPlano(
                produto,
                inicio_vigencia,
                tipo_segurado.ToString(),
                sexo.ToString(),
                idade,
                lista_coberturas);

            TraducaoPlanoDTO traducaoPlano = null;

            if (response.Status == (int)HttpStatusCode.NotFound)
                return NotFound(response.Errors.Mensagens[0]);
            if (response.Errors.Mensagens.Any())
                AdicionarErroProcessamento(response.Errors.Mensagens);
            else
                traducaoPlano = response.ObterResponseObject<TraducaoPlanoDTO>();
            return CustomResponse(traducaoPlano);
        }




        [SwaggerOperation(summary: "Utilizado para identificar o plano do cônjuge e/ou filho a partir da escolha do plano do titular",
          description: "Retorna o plano do cônjuge e/ou filho de acordo com a contratação do plano do titular e as coberturas opcionais escolhidas para o dependente")]
        [Consumes(MediaTypeNames.Application.Json)]
        [Produces(MediaTypeNames.Application.Json)]
        [SwaggerResponse(statusCode: 200, type: typeof(DeParaPlanoDependenteDTO), description: "Retorna o plano de DE/PARA para ser utilizado no dependente")]
        [SwaggerResponse(statusCode: 400, type: typeof(ValidationProblemDetails), description: "Parâmetros inválidos")]
        [SwaggerResponse(statusCode: 404, type: typeof(string), description: "DE/PARA de Plano não encontrado")]
        [HttpGet]
        [Route("de-para-plano")]
        public async Task<IActionResult> DeParaPlano(
          [SwaggerParameter(Required = true)]
            int produto,
          [SwaggerParameter(Required = true)]
            int plano_titular,
          [SwaggerParameter(Required = true)]
            DateTime vigencia_plano_titular,
          [SwaggerParameter(Required = true)]
            Tipo_Dependente tipo_dependente,
          [SwaggerParameter(Required = true)]
            bool cobertura_renda_mensal,
          [SwaggerParameter(Required = true)]
            bool cobertura_assistencia_emergencial)
        {

            if (produto <= 0) AdicionarErroProcessamento("produto inválido");
            if (plano_titular <= 0) AdicionarErroProcessamento("sequencial do plano do titular inválido");
            if (vigencia_plano_titular > DateTime.Now) AdicionarErroProcessamento("vigência do plano maior do que a data atual");


            var response = await _seguroService.DeParaPlanoDenpendentes(
                produto,
                plano_titular,
                vigencia_plano_titular,
                (int)tipo_dependente,
                cobertura_renda_mensal,
                cobertura_assistencia_emergencial);

            DeParaPlanoDependenteDTO deParaPlanoDependente = null;

            if (response.Status == (int)HttpStatusCode.NotFound)
                return NotFound(response.Errors.Mensagens[0]);
            if (response.Errors.Mensagens.Any())
                AdicionarErroProcessamento(response.Errors.Mensagens);
            else
                deParaPlanoDependente = response.ObterResponseObject<DeParaPlanoDependenteDTO>();
            return CustomResponse(deParaPlanoDependente);
        }

        public enum Tipo_Dependente
        {
            conjuge = 0,
            filho = 1
        }

    }
}