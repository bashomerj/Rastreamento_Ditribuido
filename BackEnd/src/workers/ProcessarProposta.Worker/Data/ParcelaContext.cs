using FluentValidation.Results;
using Microsoft.EntityFrameworkCore;
using ProcessarProposta.Worker.Model.Entities;
using Core.Data;
using Core.DomainObjects;
using Core.Mediator;
using Core.Messages;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace SEG.Cobranca.API.Data
{
    public sealed class ParcelaContext : DbContext, IUnitOfWork
    {
        private readonly IMediatorHandler _mediatorHandler;

        public ParcelaContext(DbContextOptions<ParcelaContext> options, IMediatorHandler mediatorHandler)
            : base(options)
        {
            _mediatorHandler = mediatorHandler;
            ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
            ChangeTracker.AutoDetectChangesEnabled = false;
        }

        public DbSet<FluxoProcessamento> FluxoProcessamento { get; set; }



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

            modelBuilder.ApplyConfigurationsFromAssembly(typeof(ParcelaContext).Assembly);

            //modelBuilder.ApplyConfiguration(new PessoaMapping());
            //modelBuilder.ApplyConfiguration(new NumeradorDocMapping());
        }

        public async Task<bool> Commit()
        {
            var sucesso = await base.SaveChangesAsync() > 0;

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


