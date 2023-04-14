using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Cliente.API.Data;
using Cliente.API.Models.Repositories;
using Cliente.API.Services;
using SEG.Core.Mediator;
using SEG.WebAPI.Core.Usuario;

namespace Cliente.API.Configuration
{
    public static class DependencyInjectionConfig
    {
        public static void RegisterServices(this IServiceCollection services)
        {
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddScoped<IAspNetUser, AspNetUser>();

            services.AddScoped<IMediatorHandler, MediatorHandler>();

            services.AddScoped<IClienteRepository, ClienteRepository>();
            services.AddScoped<IBaixaPagamentoService, BaixaPagamentoService>();

            services.AddScoped<ParcelaContext>();
            services.AddScoped<CobrancaContext>();


        }
    }
}