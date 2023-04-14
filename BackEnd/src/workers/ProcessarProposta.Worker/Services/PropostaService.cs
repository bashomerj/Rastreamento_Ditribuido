using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using ProcessarProposta.Worker.DTOs;
using ProcessarProposta.Worker.External_Services;
using ProcessarProposta.Worker.Model.Entities;
using SEG.Cobranca.API.Models.Repositories;
using SEG.Core.Communication;
using SEG.Core.Enum;
using SEG.Core.Messages.Integration;
using SEG.MessageBus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace ProcessarProposta.Worker.Services
{
    public class PropostaService : IPropostaService
    {

        private readonly ILogger<PropostaService> _logger;
        private readonly IServiceProvider _serviceProvider;



        public PropostaService(ILogger<PropostaService> logger, IServiceProvider serviceProvider)
        {
            _logger = logger;
            _serviceProvider = serviceProvider;

        }

        public async Task<ResponseResult> ProcessarProposta(PropostaValidadaIntegrationEvent messageEvent)
        {

            PropostaValidadaIntegrationEvent message = messageEvent;

            try
            {
                _logger.LogInformation("Inicio Processamento - Proposta: " + message.Seguro.proposta.ToString() + " - Data: " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss"));

                ResponseResult response = new ResponseResult();


                using (var scope = _serviceProvider.CreateScope())
                {

                    var fluxoRepository = scope.ServiceProvider.GetRequiredService<IFluxoProcessamentoRepository>();
                    var _bus = scope.ServiceProvider.GetRequiredService<IMessageBus>();

                    #region Controle do Fluxo

                    var idFluxo = JsonConvert.DeserializeObject<Guid>(message.AggregateId);
                    FluxoProcessamento fluxo = await fluxoRepository.Obter(f => f.id == idFluxo);
                    if (fluxo == null)
                    {
                        fluxo = new FluxoProcessamento(idFluxo, DateTime.Now, JsonConvert.SerializeObject(message), message.Seguro.proposta);
                        await fluxoRepository.Adicionar(fluxo);
                        await fluxoRepository.UnitOfWork.Commit();
                    }
                    
                    //Atualiza a mensagem. Isso se dá para casos de recuperação do ponto do processo por causa de exception ou interrupções
                    message = JsonConvert.DeserializeObject<PropostaValidadaIntegrationEvent>(fluxo.mensagem);


                    #endregion




                    var responsePropostaParaProcessamento = await ValidarPropostaLiberadaParaProcessamento(messageEvent.Empresa, messageEvent.Sucursal, messageEvent.Seguro.contrato, messageEvent.Seguro.proposta);
                    if (responsePropostaParaProcessamento.Errors.Mensagens.Any())
                    {
                        fluxo.AtribuirCancelamentoProcessamento("S");
                        fluxo.AtribuirMotivoCancelamento(String.Join(",", responsePropostaParaProcessamento.Errors.Mensagens));
                        fluxoRepository.Atualizar(fluxo);
                        await fluxoRepository.UnitOfWork.Commit();
                        return responsePropostaParaProcessamento;
                    }



                    #region Criação dos Segurados
                    if (string.IsNullOrEmpty(fluxo.cadastroSegurados)) /*Somente depois do risco aceito que não iremos processar segurado novamente. Por isso está setado o processo de aceitação do risco.*/
                    {
                        try
                        {
                            var listaSegurados = obterListaSegurados(message);
                            var responseSeguradosCadastrados = await CriarSegurados(listaSegurados);
                            if (responseSeguradosCadastrados.Errors.Mensagens.Any()) throw new Exception(string.Join(",", responseSeguradosCadastrados.Errors.Mensagens));

                            // Atribuir o CDPES de cada pessoa criada para a chamada de criação dos certificados
                            await AtribuirIdSegurados(message, listaSegurados);


                            fluxo.AtribuirSucessoCadastroSegurados();
                            fluxo.AtribuirMensagem(JsonConvert.SerializeObject(message));
                            fluxo.AtribuirJsonResultCadastroSegurados(JsonConvert.SerializeObject(listaSegurados));
                            fluxoRepository.Atualizar(fluxo);
                            await fluxoRepository.UnitOfWork.Commit();
                        }
                        catch (Exception e)
                        {

                            throw new Exception("Ocorreu erro na comunicação com API de criação dos segurados", e);
                        }

                        

                    }
                    #endregion

                    #region Criação do Certificado/Apólice (Seguro)

                    SeguroCriadoRetornoDTO seguroCriado = null;
                    if (string.IsNullOrEmpty(fluxo.aceitacaoRiscos))
                    {
                        try
                        {
                            var reponseSeguro = await CriarSeguro(message);
                            if (reponseSeguro.Errors.Mensagens.Any()) throw new Exception(string.Join(",", reponseSeguro.Errors.Mensagens));
                            
                            seguroCriado = reponseSeguro.ObterResponseObject<SeguroCriadoRetornoDTO>();



                            fluxo.AtribuirSucessoAceitacaoRisco();
                            fluxo.AtribuirMensagem(JsonConvert.SerializeObject(message));
                            fluxo.AtribuirJsonResultAceitacaoRiscos(JsonConvert.SerializeObject(seguroCriado));
                            //Atribui o certificado gerado. Isso se faz necessário em caso de recuperação por exception ou interrupção do processo para que o mesmo continue de onde parou, pois a continuidade precisa do numero do certificado
                            fluxo.AtribuirIdentificacaoSeguroTitular(seguroCriado.contrato, seguroCriado.emissao, seguroCriado.certificado, seguroCriado.item);
                            fluxoRepository.Atualizar(fluxo);
                            await fluxoRepository.UnitOfWork.Commit();
                        }
                        catch (Exception e)
                        {

                            throw new Exception("Ocorreu erro na comunicação com API de criação do certificado e aceitação do risco", e);
                        }
                        
                    }
                    seguroCriado = JsonConvert.DeserializeObject<SeguroCriadoRetornoDTO>(fluxo.jsonResultAceitacaoRiscos);


                    #endregion

                    #region Criação da Primeira Parcela

                    if (string.IsNullOrEmpty(fluxo.parcela))
                    {
                        try
                        {
                            var responseParcela = await CriarParcela(
                            (int)fluxo.contratoTitular, (int)fluxo.emissaoTitular, (int)fluxo.certificadoTitular, (int)fluxo.itemTitular, 1, message.Seguro.inicio_vigencia);
                            if (responseParcela.Errors.Mensagens.Any()) throw new Exception(string.Join(",", responseParcela.Errors.Mensagens));
                            


                            fluxo.AtribuirSucessoParcela();
                            fluxoRepository.Atualizar(fluxo);
                            await fluxoRepository.UnitOfWork.Commit();
                        }
                        catch (Exception e)
                        {

                            throw new Exception("Ocorreu erro na comunicação com API de criação da parcela", e);
                        }
                    }




                    #endregion

                    #region Publicação Evento para Enviar Email

                    ////Envia Email informando a conclusão do processamento
                    var messageSerialize = JsonConvert.SerializeObject(message);
                    var email = new ComunicarEmailEvent()
                    {
                        Para = "bsantos@sinaf.com.br",
                        Assunto = $"Proposta Processada.  Proposta: {fluxo.proposta} | Contrato: {fluxo.contratoTitular}| Certificado: {fluxo.certificadoTitular}",
                        Corpo = messageSerialize
                    };

                    await _bus.PublishAsync<ComunicarEmailEvent>(email);

                    #endregion

                    #region Publicação Evento para Webhook

                    //Cria o evento de PropostaProcessada que será enviado para o Webhook
                    var propostaProcessadaIntegrationEvent = new PropostaProcessadaIntegrationEvent()
                    {
                        origem = message.Origem,
                        proposta = seguroCriado.proposta,
                        situacao = seguroCriado.situacao_proposta,
                        redigitacao = null,
                        motivo_redigitacao = null,
                    };

                    propostaProcessadaIntegrationEvent.AtribuirAggregateRoot(new { proposta = propostaProcessadaIntegrationEvent.proposta });

                    await _bus.PublishAsync<PropostaProcessadaIntegrationEvent>(propostaProcessadaIntegrationEvent);




                    fluxo.ConcluirFluxo();
                    fluxo.AtribuirSucessoComunicacaoWebhook();
                    fluxo.AtribuirJsonResultComunicacaoWebhook(JsonConvert.SerializeObject(propostaProcessadaIntegrationEvent)); 
                    fluxoRepository.Atualizar(fluxo);
                    await fluxoRepository.UnitOfWork.Commit();


                    #endregion


                    response.Status = 200;
                    response.AtribuirResponseObject(new { certificado = fluxo.certificadoTitular, proposta = fluxo.proposta});


                    _logger.LogInformation("Fim Processamento - Proposta: " + message.Seguro.proposta.ToString() + " - Data: " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss"));

                    return response;
                }
            }
            catch (Exception e)
            {

                throw;
            }
        }


        private List<SeguradoCadastroDTO> obterListaSegurados(PropostaValidadaIntegrationEvent message)
        {
            List<SeguradoCadastroDTO> listaSegurados = new List<SeguradoCadastroDTO>();

            var titular = ExtrairSeguradoCadastroDeSeguradoValidado(message.Titular);
            titular.CodigoUsuario = message.Usuario;
            titular.CodigoEmpresa = message.Empresa;
            titular.CodigoSucursal = message.Sucursal;
            titular.DataVigenciaSeguro = message.Seguro.inicio_vigencia;
            titular.TipoSegurado = Tipo_Segurado.titular;

            listaSegurados.Add(titular);


            if (message.Agregados != null)
            {
                foreach (var item in message.Agregados)
                {

                    var agregado = ExtrairSeguradoCadastroDeSeguradoValidado(item);

                    agregado.CodigoUsuario = message.Usuario;
                    agregado.CodigoEmpresa = message.Empresa;
                    agregado.CodigoSucursal = message.Sucursal;
                    agregado.DataVigenciaSeguro = message.Seguro.inicio_vigencia;

                    listaSegurados.Add(agregado);
                }
            }

            return listaSegurados;

        }

        private SeguradoCadastroDTO ExtrairSeguradoCadastroDeSeguradoValidado(SeguradoValidado segurado)
        {
            var seguradoCadastro = new SeguradoCadastroDTO();

            seguradoCadastro.TipoSegurado = segurado.Tipo_Segurado;
            seguradoCadastro.Id = segurado.Pessoa.Id;
            seguradoCadastro.Nome = segurado.Pessoa.Nome;
            seguradoCadastro.Cdpes = segurado.Pessoa.Cdpes;
            seguradoCadastro.DataNascimento = segurado.Pessoa.Data_Nascimento;
            seguradoCadastro.Sexo = segurado.Pessoa.Sexo;
            seguradoCadastro.CPF = segurado.Pessoa.CPF;
            seguradoCadastro.CPFProprio = segurado.Pessoa.CPF_Proprio;
            seguradoCadastro.Renda = segurado.Pessoa.Renda;
            seguradoCadastro.Atividade = segurado.Pessoa.Atividade;
            seguradoCadastro.EstadoCivil = segurado.Pessoa.Estado_Civil;
            seguradoCadastro.Email = segurado.Pessoa.Email;
            seguradoCadastro.RG = segurado.Pessoa.RG;
            seguradoCadastro.OrgaoExpedidor = segurado.Pessoa.Orgao_Expedidor;
            seguradoCadastro.DataExpedicao = segurado.Pessoa.Data_Expedicao;
            seguradoCadastro.Telefone = new List<TelefoneDTO>();
            if (segurado.Pessoa.Endereco != null)
            {
                seguradoCadastro.Endereco = new EnderecoDTO()
                {
                    Cep = segurado.Pessoa.Endereco.Cep,
                    Logradouro = segurado.Pessoa.Endereco.Logradouro,
                    Numero = segurado.Pessoa.Endereco.Numero,
                    Bairro = segurado.Pessoa.Endereco.Bairro,
                    Complemento = segurado.Pessoa.Endereco.Complemento,
                    Cidade = segurado.Pessoa.Endereco.Cidade,
                    UF = segurado.Pessoa.Endereco.UF,
                    Pais = segurado.Pessoa.Endereco.Pais,
                    Referencia = segurado.Pessoa.Endereco.Referencia,
                };
            }
            if (segurado.Pessoa.Telefones != null)
            {
                foreach (var telefone in segurado.Pessoa.Telefones)
                {
                    seguradoCadastro.Telefone.Add(new TelefoneDTO()
                    {
                        Celular_Principal = telefone.Celular_Principal,
                        DDD = telefone.DDD,
                        Numero = telefone.Numero,
                        Receber_SMS = telefone.Receber_SMS,
                        Tipo = telefone.Tipo,
                        Whatsapp = telefone.Whatsapp
                    });
                }
            }

            return seguradoCadastro;
        }

        private async Task<ResponseResult> CriarSegurados(List<SeguradoCadastroDTO> segurados)
        {
            // ----------------------------------------------------------------------------------------------------------
            //  Passando por todas as críticas, vamos criar um mensagem na fila para processamento do seguro (RabbitMQ)
            //  ProcessarFila();
            // ----------------------------------------------------------------------------------------------------------

            using (var scope = _serviceProvider.CreateScope())
            {

                var _pessoaService = scope.ServiceProvider.GetRequiredService<IPessoaService>();

                // Criando os clientes como segurados
                var resultado = await _pessoaService.CadastrarListaPessoaSegurado(segurados);
                if (resultado.Errors.Mensagens.Any())
                    return resultado;

                //Processado com suceso, recupera o Objeto de Retorno contendo os Segurados cadastrados
                var cadastrarPessoa = resultado.ObterResponseObject<List<SeguradoCadastroDTO>>();
                if (cadastrarPessoa == null || cadastrarPessoa?.Count() <= 0) throw new Exception("Ocorreu um problema no processamento do cadastro de clientes.");


                // Atribuit o CDPES de cada pessoa criada para a chamada de criação dos certificados
                segurados?.ForEach(t => t.Cdpes = cadastrarPessoa.Where(p => p.Id == t.Id).FirstOrDefault().Cdpes);

                return resultado;

            }

        }

        private async Task AtribuirIdSegurados(PropostaValidadaIntegrationEvent message, List<SeguradoCadastroDTO> listaSegurados)
        {
            // Atribuit o CDPES de cada pessoa criada para a chamada de criação dos certificados
            message.Titular.Pessoa.Cdpes = listaSegurados.Where(t => t.Id == message.Titular.Pessoa.Id).FirstOrDefault().Cdpes;
            message.Agregados?.ForEach(t => t.Pessoa.Cdpes = listaSegurados.Where(p => p.Id == t.Pessoa.Id).FirstOrDefault().Cdpes);

        }

        private async Task<ResponseResult> CriarSeguro(PropostaValidadaIntegrationEvent message)
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var _seguroService = scope.ServiceProvider.GetRequiredService<ISeguroService>();

                //// Chamando a API para Criar o Seguro
                //CriarSeguroDTO seguro = _mapper.Map<CriarSeguroDTO>(novaPropostaDTO);
                var seguro = await ExtrairSeguroCadastroDePropostaValidadaIntegrationEvent(message);
                var resultado = await _seguroService.CriarSeguro(seguro);
                if (resultado.Errors.Mensagens.Any())
                    return resultado;
                var certificado = resultado.ObterResponseObject<SeguroCriadoRetornoDTO>();
                if (certificado == null) throw new Exception("Ocorreu um problema no processamento da criação dos certificados.");

                return resultado;
            }

            
        }

        private async Task<SeguroCadastroDTO> ExtrairSeguroCadastroDePropostaValidadaIntegrationEvent(PropostaValidadaIntegrationEvent message)
        {
            var seguroCadastrado = new SeguroCadastroDTO()
            {
                Empresa = message.Empresa,
                Sucursal = message.Sucursal,
                Usuario = message.Usuario,
                Seguro = new InfoSeguroDTO()
                {
                    contrato = message.Seguro.contrato,
                    proposta = message.Seguro.proposta,
                    inicio_vigencia = message.Seguro.inicio_vigencia,
                    produto = message.Seguro.produto,
                    digital = message.Seguro.digital,
                    premio_total = message.Seguro.premio_total,
                    colaborador = message.Seguro.colaborador,
                    periodicidade = message.Seguro.periodicidade,
                    MeioPagamento = new InfoMeioPagamento { 
                        meio_pagamento = message.Seguro.MeioPagamento.meio_pagamento,
                        debito_automatico = message.Seguro.MeioPagamento.meio_pagamento == Meio_Pagamento.debito_automatico ? new InfoDebitoAutomatico
                        {
                            agencia = message.Seguro.MeioPagamento.debito_automatico.agencia,
                            digito_Agencia = message.Seguro.MeioPagamento.debito_automatico.digito_agencia,
                            conta = message.Seguro.MeioPagamento.debito_automatico.conta,
                            digito_Conta = message.Seguro.MeioPagamento.debito_automatico.digito_conta,
                            tipo = message.Seguro.MeioPagamento.debito_automatico.tipo,
                            categoria = message.Seguro.MeioPagamento.debito_automatico.categoria,
                            titular = message.Seguro.MeioPagamento.debito_automatico.titular,
                            cpf_Titular = message.Seguro.MeioPagamento.debito_automatico.cpf_titular
                        } : null
                        },
                    
                    VendaAdministrativa = message.Seguro.VendaAdministrativa != null ? new VendaAdministrativaDTO()
                    {
                        Contrato_Original = message.Seguro.VendaAdministrativa.Contrato_Original,
                        Certificado_Original = message.Seguro.VendaAdministrativa.Certificado_Original,
                        Motivo = message.Seguro.VendaAdministrativa.Motivo
                    } : null
                    //meses_para_renda

                },
                Titular = await ExtrairInfoSeguradoDeSeguradoValidado(message.Titular)
            };

            if (message.Agregados != null && message.Agregados.Count > 0)
            {
                seguroCadastrado.Agregados = new List<InfoSeguradoDTO>();

                foreach (var item in message.Agregados)
                {
                    seguroCadastrado.Agregados.Add(await ExtrairInfoSeguradoDeSeguradoValidado(item));
                }
            }

            return seguroCadastrado;
        }

        private async Task<InfoSeguradoDTO> ExtrairInfoSeguradoDeSeguradoValidado(SeguradoValidado segurado)
        {
            var infoSegurado = new InfoSeguradoDTO()
            {
                Plano = segurado.Plano,
                Vigencia_Plano = segurado.Vigencia_Plano,
                Emissao = segurado.Emissao,
                Premio_Total = segurado.Premio_Total,
                Tipo_Segurado = segurado.Tipo_Segurado,
                Grau_Parentesco = segurado.Grau_Parentesco,
                meses_para_renda = segurado.meses_para_renda,
                Pessoa = new PessoaDTO()
                {
                    Id = segurado.Pessoa.Id,
                    Nome = segurado.Pessoa.Nome,
                    Cdpes = segurado.Pessoa.Cdpes,
                    Data_Nascimento = segurado.Pessoa.Data_Nascimento,
                    Sexo = segurado.Pessoa.Sexo,
                    CPF = segurado.Pessoa.CPF,
                    CPF_Proprio = segurado.Pessoa.CPF_Proprio,
                    Renda = segurado.Pessoa.Renda
                }
            };


            if (segurado.Coberturas != null && segurado.Coberturas.Count > 0)
            {
                infoSegurado.Coberturas = new List<CoberturasDTO>();
                foreach (var item in segurado.Coberturas)
                {
                    infoSegurado.Coberturas.Add(new CoberturasDTO()
                    {
                        Cobertura = item.Cobertura,
                        IS = item.IS,
                        Premio = item.Premio
                    });
                }
            }

            if (segurado.DPS != null && segurado.DPS.Count > 0)
            {
                infoSegurado.DPS = new List<DPSDTO>();
                foreach (var item in segurado.DPS)
                {
                    infoSegurado.DPS.Add(new DPSDTO()
                    {
                        Pergunta = item.Pergunta,
                        Resposta = item.Resposta,
                        Complemento = item.Complemento
                    });
                }
            }

            return infoSegurado;
        }

        private async Task<ResponseResult> CriarParcela(int contrato, int emissao, int certificado, int item, short parcelaAnterior, DateTime dataVencimento)
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var _cobrancaService = scope.ServiceProvider.GetRequiredService<ICobrancaService>();

                GerarParcelaDTO parcela = new GerarParcelaDTO()
                {
                    contrato = contrato,
                    emissao = emissao,
                    certificado = certificado,
                    item = item,
                    parcela = parcelaAnterior,
                    data_vencimento = dataVencimento
                };

                return await _cobrancaService.GerarParcela(parcela);
            }
        }

        private async Task<ResponseResult> ValidarPropostaLiberadaParaProcessamento(int empresa, int sucrusal, int contrato, int proposta)
        {
            // =======================================================
            //  Valida proposta se permite processamento
            // =======================================================

            using (var scope = _serviceProvider.CreateScope())
            {
                ResponseResult responseResult = new ResponseResult();

                var _seguroService = scope.ServiceProvider.GetRequiredService<ISeguroService>();

                var responseCertificado = await _seguroService.CertificadoPorProposta(empresa, sucrusal, contrato, proposta);

                if (responseCertificado.Errors.Mensagens.Any()) 
                {
                    //HttpStatusCode.UnprocessableEntity - Indica que houve crítica nos dados enviados para a requisição
                    responseCertificado.Status = (int)HttpStatusCode.UnprocessableEntity;
                    return responseCertificado;

                }
                else if (responseCertificado.Status != (int)HttpStatusCode.NotFound)
                {
                    var certificado = responseCertificado.ObterResponseObject<CertificadoPorPropostaDTO>();

                    if ((new List<int> { 1, 2, 3, 7, 9, 10 }).Contains(certificado.situacao))
                    {
                        //Http.Gone indica que o recurso não será processado e a requisição cancelada
                        responseResult.Status = (int)HttpStatusCode.Gone;
                        responseResult.Errors.Mensagens.Add($"A proposta informada já foi processada anteriormente e existe um certificado vinculado. Contrato: {certificado.contrato} - Certificado: {certificado.certificado}");
                    }
                    if (certificado.situacao == 5 && certificado.permite_redigitacao == "N")
                    {
                        responseResult.Status = (int)HttpStatusCode.Gone;
                        responseResult.Errors.Mensagens.Add($"A proposta informada está recusada mas não está marcada para redigitação. Para redigitar a proposta a mesma precisa ter sido recusada para redigitação.");
                    }

                }

               
                return responseResult;

            }

        }
    }
}
