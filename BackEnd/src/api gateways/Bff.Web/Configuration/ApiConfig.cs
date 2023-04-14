using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Bff.Web.Automapper;
using Bff.Web.Extensions;
using Email;
using WebAPI.Core.Identidade;
using System.Reflection;

namespace Bff.Web.Configuration
{
    public static class ApiConfig
    {
        public static void AddApiConfiguration(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddControllers().AddFluentValidation(
                c => { c.ValidatorOptions.LanguageManager.Culture = new System.Globalization.CultureInfo("pt-BR");
                    //c.RegisterValidatorsFromAssembly(Assembly.GetExecutingAssembly());
                });

            //Adicionando Localization
            services.AddLocalization();

            //Utilização de resources
            services.AddLocalization(options => options.ResourcesPath = "Resources");

            services.Configure<AppServicesSettings>(configuration);

            services.AddCors(options =>
            {
                options.AddPolicy("Total",
                    builder =>
                        builder
                            .AllowAnyOrigin()
                            .AllowAnyMethod()
                            .AllowAnyHeader());
            });

            services.AddAutoMapper(typeof(AutomapperSetup));

            //EMAIL
            //var emailConfigSection = configuration.GetSection("EmailConfig");
            //services.Configure<EmailSetting>(emailConfigSection);
            //var emailConfig = emailConfigSection.Get<EmailSetting>();
        }

        public static void UseApiConfiguration(this IApplicationBuilder app, IWebHostEnvironment env, ILoggerFactory loggerFactory)
        {
            if (env.IsDevelopment()) app.UseDeveloperExceptionPage();
            else app.UseGlobalErroHandler(loggerFactory);

            app.UseHttpsRedirection();

            //Localization
            var culturas = new[] { "pt-BR" };
            var localizationOptions = new RequestLocalizationOptions()
                .SetDefaultCulture(culturas[0])
                .AddSupportedCultures(culturas)
                .AddSupportedUICultures(culturas);

            app.UseRequestLocalization(localizationOptions);

            app.UseRouting();

            app.UseCors("Total");

            app.UseAuthConfiguration();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}