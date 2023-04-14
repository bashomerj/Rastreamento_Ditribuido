using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SEG.Webhook.API.Models.Entities;

namespace SEG.Seguro.API.Data.Mappings
{
    public class AssinaturaWebhookMapping : IEntityTypeConfiguration<AssinaturaWebhook>
    {
        public void Configure(EntityTypeBuilder<AssinaturaWebhook> builder)
        {
            builder.ToTable("AssinaturaWebhook");


            //Key
            builder.HasKey(b => new { b.id });

            builder
               .HasOne(c => c.ClienteIntegracao)
               .WithMany()
               .HasForeignKey(c => c.idCliente);


            builder.Property(b => b.id).HasColumnType("uniqueidentifier").IsRequired();
            builder.Property(b => b.idCliente).HasColumnType("uniqueidentifier").IsRequired();
            builder.Property(b => b.tipo).HasColumnType("varchar(200)").IsRequired();
            builder.Property(b => b.dataCriacao).HasColumnType("datetime").IsRequired();
            builder.Property(b => b.urlDestino).HasColumnType("varchar(400)").IsRequired();
            builder.Property(b => b.token).HasColumnType("varchar(2000)");

        }
    }
}
