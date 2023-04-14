using System;
using System.Collections;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using FluentValidation.Results;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using SEG.Core.Messages.Integration;
using SEG.MessageBus;
using SEG.Webhook.API.Models.Entities;
using SEG.Webhook.API.Models.Interfaces;
using SEG.Webhook.API.Models.Repositories;
using SEG.Webhook.API.Services;

namespace SEG.Webhook.API.IntegrationServices
{
    public class PropostaProcessadaIntegrationHandler : BackgroundService
    {
        private readonly IMessageBus _bus;
        private readonly IServiceProvider _serviceProvider;

        public PropostaProcessadaIntegrationHandler(
                            IServiceProvider serviceProvider,
                            IMessageBus bus)
        {
            _serviceProvider = serviceProvider;
            _bus = bus;

        }

        private void SetResponder()
        {
            _bus.SubscribeAsync<PropostaProcessadaIntegrationEvent>("PropostaProcessada", async request =>
                await EnviarWebhook(request));

            _bus.AdvancedBus.Connected += OnConnect;
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            SetResponder();
            return Task.CompletedTask;
        }

        private void OnConnect(object s, EventArgs e)
        {
            SetResponder();
        }

        private async Task<ResponseMessage> EnviarWebhook(PropostaProcessadaIntegrationEvent message)
        {
            var response = new ResponseMessage(new ValidationResult());

            using var scope = _serviceProvider.CreateScope();
            var _webhooksSender = scope.ServiceProvider.GetRequiredService<IWebhooksSender>();
            var _retriever = scope.ServiceProvider.GetRequiredService<IWebhooksRetriever>();
            var _loggerFactory = scope.ServiceProvider.GetRequiredService<ILoggerFactory> ();
            var _logger = _loggerFactory.CreateLogger("");

            string id = message.AggregateId.Replace("\n", "").Replace("\r", ""); //Remove caracter nova linha
            _logger.LogInformation($"inicio processamento webhook {id}");

            //Montando objeto do Webhook
             WebhookData payload = new WebhookData(WebhookType.proposta_processada, message);

            //Recuperar os assinantes do Webhook
            var assinaturas = await _retriever.ObterAssinaturasPorTipo(message.origem, WebhookType.proposta_processada) ;

            //Enviando a mensagem para todos os assinantes cadastrados
            await _webhooksSender.SendAll(assinaturas, payload, WebhookType.proposta_processada);


            _logger.LogInformation($"fim processamento webhook {id}");

            return response;

        }

       
    }
}