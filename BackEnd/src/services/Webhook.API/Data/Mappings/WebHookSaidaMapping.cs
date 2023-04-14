using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SEG.Webhook.API.Models.Entities;

namespace SEG.Seguro.API.Data.Mappings
{
    public class WebHookSaidaMapping : IEntityTypeConfiguration<WebHookSaida>
    {
        public void Configure(EntityTypeBuilder<WebHookSaida> builder)
        {
            builder.ToTable("WebHookSaida");


            //Key
            builder.HasKey(b => new { b.id });

            builder
               .HasOne(c => c.AssinaturaWebhook)
               .WithMany()
               .HasForeignKey(c => c.idAssinatura);


            builder.Property(b => b.id).HasColumnType("uniqueidentifier").IsRequired();
            builder.Property(b => b.idAssinatura).HasColumnType("uniqueidentifier").IsRequired();
            builder.Property(b => b.tipo).HasColumnType("varchar(200)").IsRequired();
            builder.Property(b => b.dataEnvio).HasColumnType("datetime").IsRequired();
            builder.Property(b => b.urlDestino).HasColumnType("varchar(400)").IsRequired();
            builder.Property(b => b.token).HasColumnType("varchar(2000)");
            builder.Property(b => b.payload).HasColumnType("varchar(max)");
            builder.Property(b => b.codigoRetorno).HasColumnType("int");
            

        }
    }
}
