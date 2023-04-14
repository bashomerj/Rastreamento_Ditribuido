using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using SEG.Core.Mediator;
using SEG.Email;
using Catalogo.API.Data.Repositories;
using Catalogo.API.Extensions;
using Catalogo.API.Models.Repositories;
using Catalogo.API.Services;
using SEG.WebAPI.Core.Usuario;

namespace Catalogo.API.Configuration
{
    public static class DependencyInjectionConfig
    {
        public static void RegisterServices(this IServiceCollection services)
        {
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddScoped<IAspNetUser, AspNetUser>();
            services.AddScoped<IMediatorHandler, MediatorHandler>();


            services.AddTransient<HttpClientAuthorizationDelegatingHandler>();
     

            /*Repositories*/
            services.AddScoped<ICertificadoRepository, CertificadoRepository>();
 
            /*Services*/
            services.AddScoped<IContratoService, ContratoService>();


            /*Email*/
            services.AddScoped<IEmailSender, EmailSender>();

        }
    }
}