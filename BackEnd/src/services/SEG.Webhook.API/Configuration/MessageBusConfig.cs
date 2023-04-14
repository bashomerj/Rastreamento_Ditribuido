using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SEG.Core.Utils;
using SEG.MessageBus;
using SEG.Webhook.API.IntegrationServices;

namespace SEG.Webhook.API.Configuration
{
    public static class MessageBusConfig
    {
        public static void AddMessageBusConfiguration(this IServiceCollection services,
            IConfiguration configuration)
        {
            //services.AddMessageBus(configuration.GetMessageQueueConnection("MessageBus"));


            services.AddMessageBus(configuration.GetMessageQueueConnection("MessageBus"))
                .AddHostedService<PropostaProcessadaIntegrationHandler>();
        }
    }
}