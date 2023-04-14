using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SEG.Webhook.API.Models.Entities;

namespace SEG.Seguro.API.Data.Mappings
{
    public class ClienteIntegracaoMapping : IEntityTypeConfiguration<ClienteIntegracao>
    {
        public void Configure(EntityTypeBuilder<ClienteIntegracao> builder)
        {
            builder.ToTable("ClienteIntegracao");


            //Key
            builder.HasKey(b => new { b.id });

            builder.Property(b => b.id).HasColumnType("uniqueidentifier").IsRequired();
            builder.Property(b => b.nome).HasColumnType("varchar(200)").IsRequired();
            builder.Property(b => b.dataCriacao).HasColumnType("datetime").IsRequired();


        }
    }
}
