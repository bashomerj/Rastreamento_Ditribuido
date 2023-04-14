using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ProcessarProposta.Worker.Model.Entities;

namespace ProcessarProposta.Worker.Data.Mappings
{
    public class FluxoProcessamentoMapping : IEntityTypeConfiguration<FluxoProcessamento>
    {
        public void Configure(EntityTypeBuilder<FluxoProcessamento> builder)
        {
            builder.ToTable("FluxoProcessamento");


            builder.HasKey(f => new { f.id });

            builder.Property(f => f.id).HasColumnType("uniqueidentifier").HasColumnName("id").IsRequired();
            builder.Property(f => f.dataInicio).HasColumnType("datetime").HasColumnName("dataInicio").IsRequired();
            builder.Property(f => f.cadastroSegurados).HasColumnType("char(1)").HasColumnName("cadastroSegurados");
            builder.Property(f => f.jsonResultCadastroSegurados).HasColumnType("nvarchar(2000)").HasColumnName("jsonResultCadastroSegurados");
            builder.Property(f => f.aceitacaoRiscos).HasColumnType("char(1)").HasColumnName("aceitacaoRiscos");
            builder.Property(f => f.jsonResultAceitacaoRiscos).HasColumnType("nvarchar(2000)").HasColumnName("jsonResultAceitacaoRiscos");
            builder.Property(f => f.parcela).HasColumnType("char(1)").HasColumnName("parcela");
            builder.Property(f => f.jsonResultParcela).HasColumnType("nvarchar(2000)").HasColumnName("jsonResultParcela");
            builder.Property(f => f.comunicacaoWebhook).HasColumnType("char(1)").HasColumnName("comunicacaoWebhook");
            builder.Property(f => f.jsonResultcomunicacaoWebhook).HasColumnType("nvarchar(2000)").HasColumnName("jsonResultcomunicacaoWebhook");
            builder.Property(f => f.dataConclusao).HasColumnType("datetime").HasColumnName("dataConclusao");
            builder.Property(f => f.mensagem).HasColumnType("nvarchar(max)").HasColumnName("mensagem");
            builder.Property(f => f.proposta).HasColumnType("int").HasColumnName("proposta");
            builder.Property(f => f.contratoTitular).HasColumnType("int").HasColumnName("contratoTitular");
            builder.Property(f => f.emissaoTitular).HasColumnType("int").HasColumnName("emissaoTitular");
            builder.Property(f => f.certificadoTitular).HasColumnType("int").HasColumnName("certificadoTitular");
            builder.Property(f => f.itemTitular).HasColumnType("int").HasColumnName("itemTitular");
            builder.Property(f => f.canceladoProcessamento).HasColumnType("char(1)").HasColumnName("canceladoProcessamento");
            builder.Property(f => f.motivoCancelamento).HasColumnType("nvarchar(2000)").HasColumnName("motivoCancelamento");

            
        }

    }
}
