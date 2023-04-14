using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Polly;
using SEG.Bff.Web.DTO;
using SEG.Bff.Web.Extensions;
using SEG.Bff.Web.Services;
using SEG.WebAPI.Core.Extensions;
using SEG.WebAPI.Core.Usuario;
using System;
using static SEG.Bff.Web.DTO.NovaPropostaDTO;

namespace SEG.Bff.Web.Configuration.Configuration
{
    public static class DependencyInjectionConfig
    {
        public static void RegisterServices(this IServiceCollection services)
        {
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddScoped<IAspNetUser, AspNetUser>();

            services.AddTransient<HttpClientAuthorizationDelegatingHandler>();

            services.AddHttpClient<IPessoaService, PessoaService>()
                .AddHttpMessageHandler<HttpClientAuthorizationDelegatingHandler>()
                .AddPolicyHandler(PollyExtensions.EsperarTentar())
                .AddTransientHttpErrorPolicy(
                   p => p.CircuitBreakerAsync(5, TimeSpan.FromSeconds(30)));

            services.AddHttpClient<ISeguroService, SeguroService>()
                .AddHttpMessageHandler<HttpClientAuthorizationDelegatingHandler>()
                .AddPolicyHandler(PollyExtensions.EsperarTentar())
                .AddTransientHttpErrorPolicy(
                   p => p.CircuitBreakerAsync(5, TimeSpan.FromSeconds(30)));

            services.AddHttpClient<IProdutosService, ProdutosService>()
                .AddHttpMessageHandler<HttpClientAuthorizationDelegatingHandler>()
                .AddPolicyHandler(PollyExtensions.EsperarTentar())
                .AddTransientHttpErrorPolicy(
                   p => p.CircuitBreakerAsync(5, TimeSpan.FromSeconds(30)));

            services.AddHttpClient<ICobrancaService, CobrancaService>()
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