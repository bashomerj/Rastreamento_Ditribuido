using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using ProcessarProposta.Worker.Services;
using Core.Messages.Integration;
using MessageBus;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace ProcessarProposta.Worker
{
    public class WorkerEnviarEmail : BackgroundService
    {
        private readonly ILogger<WorkerEnviarEmail> _logger;
        private readonly IMessageBus _bus;
        private readonly IServiceProvider _serviceProvider;


        public WorkerEnviarEmail(IServiceProvider serviceProvider, IMessageBus bus, ILogger<WorkerEnviarEmail> logger)
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

        private void SetSubscribers()
        {
            try
            {
                using (var scope = _serviceProvider.CreateScope())
                {
                    var emailService = scope.ServiceProvider.GetRequiredService<ISendEmailService>();
                    _bus.SubscribeAsync<ComunicarEmailEvent>("ComunicarEmail", async request => await emailService.ComunicarEmail(request));

                    _logger.LogInformation("consumindo fila ComunicarEmail [Evento: ComunicarEmailEvent]");
                }
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Erro no envio de email");
                throw;
            }
        }
    }
}
