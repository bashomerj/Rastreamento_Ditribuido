﻿using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Cliente.API.Automapper;
using Cliente.API.Data;
using Email;
using WebAPI.Core.Identidade;
using Microsoft.ApplicationInsights.DependencyCollector;

namespace Cliente.API.Configuration
{
    public static class ApiConfig
    {
        public static void AddApiConfiguration(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<ParcelaContext>(options =>
                options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

            services.AddDbContext<CobrancaContext>(options =>
                options.UseSqlServer(configuration.GetConnectionString("DefaultConnectionCobranca")));

            services.AddControllers();

            services.AddCors(options =>
            {
                options.AddPolicy("Total",
                    builder =>
                        builder
                            .AllowAnyOrigin()
                            .AllowAnyMethod()
                            .AllowAnyHeader());
            });

            //EMAIL
            var emailConfigSection = configuration.GetSection("EmailConfig");
            services.Configure<EmailSetting>(emailConfigSection);
            var emailConfig = emailConfigSection.Get<EmailSetting>();

            services.AddTransient<IEmailSender, EmailSender>();

            services.AddAutoMapper(typeof(AutomapperSetup));

            services.AddApplicationInsightsTelemetry(configuration)
            .ConfigureTelemetryModule<DependencyTrackingTelemetryModule>((module, o) =>
            {
                module.EnableSqlCommandTextInstrumentation = true;
                //module.EnableLegacyCorrelationHeadersInjection= true;
            });
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