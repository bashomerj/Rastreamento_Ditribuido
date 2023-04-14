using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.ApplicationInsights.Extensibility.PerfCounterCollector.QuickPulse;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Events;
using Serilog.Exceptions;
using Serilog.Filters;

namespace Bff.Web
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json")
            .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")}.json", true)
            .AddEnvironmentVariables()
            .Build();

            SerilogExtensions.AddSerilogApi(configuration);

            try
            {
                Log.Information("...Iniciando Aplicação...");
                CreateHostBuilder(args).Build().Run();
            }
            catch (Exception e)
            {
                Log.Fatal(e, "Erro na inicialização da aplicação");
                throw;
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.ConfigureLogging(builder =>
                     {
                         builder.ClearProviders();
                         builder.AddSerilog();
                     });
                    webBuilder.UseStartup<Startup>();
                });
            //.UseSerilog();
    }





    public static class SerilogExtensions
    {
        public static void AddSerilogApi(IConfiguration configuration)
        {
            var appInsightsConfig = TelemetryConfiguration.CreateDefault();
            appInsightsConfig.ConnectionString = configuration.GetSection("ApplicationInsights")["ConnectionString"];
            appInsightsConfig.AddLiveMetrisApplicationInsights();

            Log.Logger = new LoggerConfiguration()
                .ReadFrom.Configuration(configuration)
                .Enrich.FromLogContext()
                .Enrich.WithExceptionDetails()
                .Enrich.WithProperty("ApplicationName", $"{typeof(Program).Namespace} - {Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")}")
                //.Filter.ByExcluding(Matching.FromSource("Microsoft.AspNetCore.StaticFiles"))
                //.Filter.ByExcluding(Matching.FromSource("Microsoft.AspNetCore.Mvc.Infrastructure.ControllerActionInvoker"))
                //.Filter.ByExcluding(Matching.FromSource("Microsoft.AspNetCore.Hosting.Diagnostics"))
                //.Filter.ByExcluding(Matching.FromSource("Serilog.AspNetCore.RequestLoggingMiddleware"))
                .WriteTo.ApplicationInsights(appInsightsConfig, TelemetryConverter.Traces)
                //.WriteTo.Console(outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj} {Properties:j}{NewLine}{Exception}")
                .CreateLogger();

         }
    }

    public static class ApplicationInsightsExtensions
    {
        public static void AddLiveMetrisApplicationInsights(this TelemetryConfiguration appInsightsConfig)
        {
            var builder = appInsightsConfig.TelemetryProcessorChainBuilder;
            QuickPulseTelemetryProcessor quickPulseProcessor = null;
            builder.Use((next) =>
            {
                quickPulseProcessor = new QuickPulseTelemetryProcessor(next);
                return quickPulseProcessor;
            });
            builder.Build();

            var quickPulse = new QuickPulseTelemetryModule();
            quickPulse.Initialize(appInsightsConfig);
            quickPulse.RegisterTelemetryProcessor(quickPulseProcessor);
        }
    }


}
