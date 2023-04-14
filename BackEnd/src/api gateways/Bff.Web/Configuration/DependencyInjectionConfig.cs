using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Polly;
using Bff.Web.DTO;
using Bff.Web.Extensions;
using Bff.Web.Services;
using WebAPI.Core.Extensions;
using WebAPI.Core.Usuario;
using System;
using static Bff.Web.DTO.NovaPropostaDTO;

namespace Bff.Web.Configuration.Configuration
{
    public static class DependencyInjectionConfig
    {
        public static void RegisterServices(this IServiceCollection services)
        {
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddScoped<IAspNetUser, AspNetUser>();

            services.AddTransient<HttpClientAuthorizationDelegatingHandler>();

            services.AddHttpClient<IClienteService, ClienteService>()
                .AddHttpMessageHandler<HttpClientAuthorizationDelegatingHandler>()
                .AddPolicyHandler(PollyExtensions.EsperarTentar())
                .AddTransientHttpErrorPolicy(
                   p => p.CircuitBreakerAsync(5, TimeSpan.FromSeconds(30)));

            services.AddHttpClient<ICatalogoService, CatalogoService>()
                .AddHttpMessageHandler<HttpClientAuthorizationDelegatingHandler>()
                .AddPolicyHandler(PollyExtensions.EsperarTentar())
                .AddTransientHttpErrorPolicy(
                   p => p.CircuitBreakerAsync(5, TimeSpan.FromSeconds(30)));


            services.AddScoped<AbstractValidator<NovaPropostaDTO>, NovaPropostaDTOValidation>(); 

            //services.AddScoped<IInternalService, InternalService>();

            //services.AddScoped<IEmailSender, EmailSender>();
            //services.AddHostedService<PropostaValidadaIntegrationHandler>();
            //services.AddHostedService<ComunicarEmailIntegrationHandler>();

        }
    }
}