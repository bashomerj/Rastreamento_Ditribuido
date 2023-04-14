using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using ProcessarProposta.Worker.Model.Entities;
using ProcessarProposta.Worker.Services;
using SEG.Cobranca.API.Models.Repositories;
using Core.Communication;
using Core.Messages.Integration;
using MessageBus;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace ProcessarProposta.Worker
{
    public class WorkerProcessarProposta : BackgroundService
    {
        private readonly ILogger<WorkerProcessarProposta> _logger;
        private readonly IMessageBus _bus;
        private readonly IServiceProvider _serviceProvider;


        public WorkerProcessarProposta(IServiceProvider serviceProvider, IMessageBus bus, ILogger<WorkerProcessarProposta> logger)
        {
            _serviceProvider = serviceProvider;
            _bus = bus;
            _logger = logger;

        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            SetSubscribers();
            return Task.CompletedTask;
        }

        private async Task SetSubscribers()
        {
            try
            {
                using (var scope = _serviceProvider.CreateScope())
                {
                    var propostaService = scope.ServiceProvider.GetRequiredService<IPropostaService>();

                    _bus.SubscribeAsync<PropostaValidadaIntegrationEvent>("PropostaValidada", async request => await propostaService.ProcessarProposta(request));

                    _logger.LogInformation("consumindo fila PropostaValidada [Evento: PropostaValidadaIntegrationEvent]");
                }
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Erro ao processar proposta");
                throw;
            }
        }
    }
}
