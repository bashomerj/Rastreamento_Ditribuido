//using AutoMapper;
//using Microsoft.Extensions.DependencyInjection;
//using Microsoft.Extensions.Hosting;
//using Newtonsoft.Json;
//using Bff.Web.Controllers;
//using Bff.Web.DTO;
//using Bff.Web.Services;
//using Core.Mediator;
//using Core.Messages.Integration;
//using Email;
//using MessageBus;
//using System;
//using System.Linq;
//using System.Threading;
//using System.Threading.Tasks;
//using Email;
//using System.Collections.Generic;

//namespace Bff.Web.IntegrationServices
//{

//    public class PropostaValidadaIntegrationHandler : BackgroundService
//    {
//        private readonly IMessageBus _bus;
//        private readonly IServiceProvider _serviceProvider;

//        public PropostaValidadaIntegrationHandler(IServiceProvider serviceProvider, IMessageBus bus)
//        {
//            _serviceProvider = serviceProvider;
//            _bus = bus;
//        }
//        protected override Task ExecuteAsync(CancellationToken stoppingToken)
//        {
//            SetSubscribers();
//            return Task.CompletedTask;
//        }

//        private void SetSubscribers()
//        {
//            _bus.SubscribeAsync<PropostaValidadaIntegrationEvent>("PropostaValidada", async request =>
//                await ProcessarProposta(request));
//        }

//        private async Task ProcessarProposta(PropostaValidadaIntegrationEvent message)
//        {
//            try
//            {
//                message.Titular.Pessoa.Id = Guid.NewGuid();
//                foreach (var item in message.Agregados)
//                {
//                    item.Pessoa.Id = Guid.NewGuid();
//                }
                

//                using var scope = _serviceProvider.CreateScope();
//                var mapper = scope.ServiceProvider.GetRequiredService<IMapper>();
//                //var _mediator = scope.ServiceProvider.GetRequiredService<IMediatorHandler>();
//                var _email = scope.ServiceProvider.GetRequiredService<IEmailSender>();
//                var _internalService = scope.ServiceProvider.GetRequiredService<IInternalService>();



//                //Chama serviço responsável por comunicar as APIs para a criação do Seguro e Segurados
//                NovaPropostaDTO novaPropostaDTO = mapper.Map<NovaPropostaDTO>(message);
//                var retorno = await _internalService.ProcessarProposta(novaPropostaDTO);
//                if (retorno.Errors.Mensagens.Any())
//                    return;
//                var seguroRetorno = retorno.ObterResponseObject<SeguroRetornoCriacaoDTO>();



//                //Cria o evento de PropostaProcessada que será enviado para o Webhook
//                var propostaProcessadaIntegrationEvent = new PropostaProcessadaIntegrationEvent()
//                {
//                    Empresa = novaPropostaDTO.Empresa,
//                    Sucursal = novaPropostaDTO.Sucursal,
//                    Origem = novaPropostaDTO.Origem,
//                    Proposta = novaPropostaDTO.Seguro.proposta,
//                    CertificadoTitular = new Seguro()
//                    {
//                        Contrato = seguroRetorno.Titular.cdconseg,
//                        Certificado = seguroRetorno.Titular.nrcer,
//                        Meio_pagamento = novaPropostaDTO.Seguro.meio_pagamento,
//                        Colaborador = novaPropostaDTO.Seguro.colaborador,
//                        Segurado = new Segurado()
//                        {
//                            CPF = novaPropostaDTO.Titular.Pessoa.CPF,
//                            Nome = novaPropostaDTO.Titular.Pessoa.Nome,
//                            Data_Nascimento = novaPropostaDTO.Titular.Pessoa.Data_Nascimento
//                        },
//                        Coberturas = novaPropostaDTO.Titular.Coberturas.Select(x => new CoberturaSegurada { Cobertura = x.Cobertura, IS = x.IS}).ToList()
//                    },
//                    CertificadoAgregados = new List<Seguro>()
//                };
                

//                foreach (var agregado in seguroRetorno.Agregados ?? Enumerable.Empty<CertificadoRetornoCriacaoDTO>())
//                {
//                    propostaProcessadaIntegrationEvent.CertificadoAgregados.Add(new Seguro()
//                    {
//                        Contrato = agregado.cdconseg,
//                        Certificado = agregado.nrcer,
//                        Segurado = new Segurado()
//                        {
//                            CPF = agregado.CPF.ToString(),
//                            Nome = agregado.nome,
//                            Data_Nascimento = agregado.dataNascimento
//                        },
//                        Coberturas = agregado.Coberturas.Select(x => new CoberturaSegurada { Cobertura = x.Coberura, IS = x.IS}).ToList()
//                    });
//                }

//                propostaProcessadaIntegrationEvent.AtribuirAggregateRoot(new { proposta = propostaProcessadaIntegrationEvent.Proposta });

//                await _bus.PublishAsync<PropostaProcessadaIntegrationEvent>(propostaProcessadaIntegrationEvent);





//                //Envia Email informando a conclusão do processamento
//                var messageSerialize = JsonConvert.SerializeObject(message);
//                var email = new ComunicarEmailEvent() { 
//                    Para = "bashomerj@gmail.com", 
//                    Assunto = $"Proposta Processada.  Proposta: {novaPropostaDTO.Seguro.proposta} | Contrato: {seguroRetorno.Titular.cdconseg}| Certificado: {seguroRetorno.Titular.nrcer}" ,
//                    Corpo = messageSerialize
//                };

//                await _bus.PublishAsync<ComunicarEmailEvent>(email);

//            }
//            catch (Exception e )
//            {

//                throw;
//            }
//        }
//    }
//}