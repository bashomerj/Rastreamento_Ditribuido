using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Catalogo.API.Models.Entities;

namespace Catalogo.API.Data.Mappings
{
    public class CertificadoMapping : IEntityTypeConfiguration<Certificado>
    {
        public void Configure(EntityTypeBuilder<Certificado> builder)
        {
            builder.ToTable("Certificado");


            builder.HasKey(c => c.id);

            builder.Property(c => c.id).HasColumnType("uniqueidentifier").IsRequired();
            builder.Property(c => c.idProposta).HasColumnType("uniqueidentifier").IsRequired();
            builder.Property(c => c.certificado).HasColumnType("int").IsRequired();



        }

    }
}
