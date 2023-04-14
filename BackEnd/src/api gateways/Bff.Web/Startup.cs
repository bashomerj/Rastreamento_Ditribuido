using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Bff.Web.Configuration;
using Bff.Web.Configuration.Configuration;
using Bff.Web.Extensions;
using WebAPI.Core.Identidade;
using System.Text.Json.Serialization;
using Microsoft.ApplicationInsights.DependencyCollector;

namespace Bff.Web
{
    public class Startup
    {
        public IConfiguration Configuration { get; }
        private readonly IWebHostEnvironment _hostingEnv;

        public Startup(IHostEnvironment hostEnvironment, IWebHostEnvironment env)
        {
            _hostingEnv = env;

            var builder = new ConfigurationBuilder()
                .SetBasePath(hostEnvironment.ContentRootPath)
                .AddJsonFile("appsettings.json", true, true)
                .AddJsonFile($"appsettings.{hostEnvironment.EnvironmentName}.json", true, true)
                .AddEnvironmentVariables();

            if (hostEnvironment.IsDevelopment())
            {
                builder.AddUserSecrets<Startup>();
            }

            Configuration = builder.Build();
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllersWithViews()
                //.AddJsonOptions(options =>
                //{
                //    options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
                //    options.JsonSerializerOptions.IgnoreNullValues = true;
                //    options.JsonSerializerOptions.PropertyNameCaseInsensitive = true;

                //})
                .AddNewtonsoftJson(options =>
                 {
                     //Ignorar o Looping de referência
                     options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
                     //Ignorar as propriedades nulas
                     options.SerializerSettings.NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore;
                     //Tratar enum como número e como texto
                     options.SerializerSettings.Converters.Add(new Newtonsoft.Json.Converters.StringEnumConverter { CamelCaseText = true });
                 });
            


            services.ConfigureGlobalErroHandler();

            services.AddApiConfiguration(Configuration);

            services.AddJwtConfiguration(Configuration);

            services.AddSwaggerConfiguration(_hostingEnv);

            services.RegisterServices();

            services.AddMessageBusConfiguration(Configuration);

            services.AddApplicationInsightsTelemetry(Configuration)
                .ConfigureTelemetryModule<DependencyTrackingTelemetryModule>((module, o) =>
                {
                    module.EnableSqlCommandTextInstrumentation= true;
                    //module.EnableLegacyCorrelationHeadersInjection= true;
                });

            
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILoggerFactory loggerFactory)
        {
            //app.UseMiddleware<RequestLoggingMiddleware>();
            app.UseSwaggerConfiguration();
            app.UseApiConfiguration(env, loggerFactory);
        }
    }
}
