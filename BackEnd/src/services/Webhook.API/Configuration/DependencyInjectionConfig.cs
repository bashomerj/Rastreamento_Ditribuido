using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using SEG.Core.Mediator;
using SEG.Email;
using SEG.WebAPI.Core.Usuario;
using SEG.Webhook.API.Data;
using SEG.Webhook.API.Data.Repositories;
using SEG.Webhook.API.Models.Repositories;
using SEG.Webhook.API.Services;

namespace SEG.Webhook.API.Configuration
{
    public static class DependencyInjectionConfig
    {
        public static void RegisterServices(this IServiceCollection services)
        {
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddScoped<IAspNetUser, AspNetUser>();
            services.AddScoped<IMediatorHandler, MediatorHandler>();


            services.AddHttpClient();

            /*Repositories*/
            services.AddScoped<IClienteIntegracaoRepository, ClienteIntegracaoRepository>();
            services.AddScoped<IAssinaturaWebhookRepository, AssinaturaWebhookRepository>();
            services.AddScoped<IWebHookSaidaRepository, WebHookSaidaRepository>();


            /*Services*/
            services.AddScoped<IWebhooksSender, WebhooksSender>();
            services.AddScoped<IWebhooksRetriever, WebhooksRetriever>();
            

            /*Integration Services Handle*/
            //services.AddHostedService<PropostaValidadaIntegrationHandler>();


            /*Context Entity*/
            services.AddScoped<WebHookContext>();

            //Email
            services.AddScoped<IEmailSender, EmailSender>();

        }
    }
}