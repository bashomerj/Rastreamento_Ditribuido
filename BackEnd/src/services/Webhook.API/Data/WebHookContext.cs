using FluentValidation.Results;
using Microsoft.EntityFrameworkCore;
using SEG.Core.Data;
using SEG.Core.DomainObjects;
using SEG.Core.Mediator;
using SEG.Core.Messages;
using SEG.Webhook.API.Models.Entities;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace SEG.Webhook.API.Data
{
    public class WebHookContext : DbContext, IUnitOfWork
    {
        private readonly IMediatorHandler _mediatorHandler;

        public WebHookContext(DbContextOptions<WebHookContext> options, IMediatorHandler mediatorHandler) : base(options)
        {
            _mediatorHandler = mediatorHandler;
            ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
            ChangeTracker.AutoDetectChangesEnabled = false;
        }

        public DbSet<ClienteIntegracao> ClienteIntegracao { get; set; }
        public DbSet<AssinaturaWebhook> AssinaturaWebhook { get; set; }
        public DbSet<WebHookSaida> WebHookSaida { get; set; }



        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Ignore<ValidationResult>();
            modelBuilder.Ignore<Event>();

            foreach (var property in modelBuilder.Model.GetEntityTypes().SelectMany(
                e => e.GetProperties().Where(p => p.ClrType == typeof(DateTime))))
                property.SetColumnType("Datetime");

            foreach (var property in modelBuilder.Model.GetEntityTypes().SelectMany(
                e => e.GetProperties().Where(p => p.ClrType == typeof(Nullable<DateTime>))))
                property.SetColumnType("Datetime");

            foreach (var relationship in modelBuilder.Model.GetEntityTypes()
                .SelectMany(e => e.GetForeignKeys())) relationship.DeleteBehavior = DeleteBehavior.ClientSetNull;

            modelBuilder.ApplyConfigurationsFromAssembly(typeof(WebHookContext).Assembly);

        }

        public async Task<bool> Commit()
        {
            var sucesso = await base.SaveChangesAsync() > 0;

            //var changedEntriesCopy = base.ChangeTracker.Entries<NumeradorDoc>()
            //  .Where(e => e.State == EntityState.Unchanged)
            //  .ToList();

            //foreach (var entry in changedEntriesCopy)
            //    entry.State = EntityState.Detached;



            //TODO: AVALIAR MELHOR ISSO (FOI COLOCADO POR CAUSA DO PROCESSO ACEITAÇÃO DEPOIS DE ROLAR A GRAVAÇÃO DAS INFORMAÇÕES DO CERTIFICADO)
            var all = base.ChangeTracker.Entries()
                .Where(e => e.State == EntityState.Unchanged);
            foreach (var entry in all)
                entry.State = EntityState.Detached;



            if (sucesso) await _mediatorHandler.PublicarEventos(this);

            return sucesso;
        }

        public void BeginTran()
        {
            base.Database.BeginTransactionAsync();
        }

        public async void CommitTran()
        {
            await this.Commit();
            base.Database.CommitTransaction();
        }

        public void RollbackTran()
        {
            base.Database.RollbackTransaction();
        }
    }

    public static class MediatorExtension
    {
        public static async Task PublicarEventos<T>(this IMediatorHandler mediator, T ctx) where T : DbContext
        {
            var domainEntities = ctx.ChangeTracker
                .Entries<Entity>()
                .Where(x => x.Entity.Notificacoes != null && x.Entity.Notificacoes.Any());

            var domainEvents = domainEntities
                .SelectMany(x => x.Entity.Notificacoes)
                .ToList();

            domainEntities.ToList()
                .ForEach(entity => entity.Entity.LimparEventos());

            var tasks = domainEvents
                .Select(async (domainEvent) => {
                    await mediator.PublicarEvento(domainEvent);
                });

            await Task.WhenAll(tasks);
        }
    }
}
