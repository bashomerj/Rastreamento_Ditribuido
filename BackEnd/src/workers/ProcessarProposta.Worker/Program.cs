using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using ProcessarProposta.Worker.AutoMapper;
using ProcessarProposta.Worker.Extensions;
using ProcessarProposta.Worker.External_Services;
using ProcessarProposta.Worker.Services;
using SEG.Cobranca.API.Data;
using SEG.Cobranca.API.Models.Repositories;
using SEG.Core.Mediator;
using SEG.Core.Utils;
using SEG.Email;
using SEG.MessageBus;


namespace ProcessarProposta.Worker
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureServices((hostContext, services) =>
                {

                    #region Entity
                    services.AddDbContext<ParcelaContext>(options =>
                        options.UseSqlServer(hostContext.Configuration.GetConnectionString("DefaultConnection")), ServiceLifetime.Scoped);
                    #endregion

                    #region MediatR and Bus
                    services.AddScoped<IMediatorHandler, MediatorHandler>();
                    services.AddMediatR(typeof(Program));
                    services.AddMessageBus(hostContext.Configuration.GetMessageQueueConnection("MessageBus"));
                    #endregion

                    #region AutoMapper
                    services.AddAutoMapper(typeof(AutomapperSetup));
                    #endregion

                    #region HostedServices
                    services.AddHostedService<WorkerProcessarProposta>();
                    services.AddHostedService<WorkerEnviarEmail>();
                    #endregion

                    #region Repositories
                    services.AddScoped<IFluxoProcessamentoRepository, FluxoProcessamentoRepository>();
                    #endregion

                    #region Services
                    services.AddSingleton<IPropostaService, PropostaService>();
                    #endregion

                    #region External Services
                    services.AddHttpClient<IPessoaService, PessoaService>();
                    services.AddHttpClient<ISeguroService, SeguroService>();
                    services.AddHttpClient<ICobrancaService, CobrancaService>();
                    #endregion

                    #region EMAIL
                    services.AddScoped<IEmailSender, EmailSender>();
                    services.AddScoped<ISendEmailService, SendEmailService>();
                    var emailConfigSection = hostContext.Configuration.GetSection("EmailConfig");
                    services.Configure<EmailSetting>(emailConfigSection);
                    var emailConfig = emailConfigSection.Get<EmailSetting>();
                    #endregion

                    services.Configure<AppServicesSettings>(hostContext.Configuration);
                });
    }
}
