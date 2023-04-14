using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Cliente.API.Models.Entities;

namespace Cliente.API.Models.Mappings
{
    public class ClienteMapping : IEntityTypeConfiguration<Cliente.API.Models.Entities.Cliente>
    {
        public void Configure(EntityTypeBuilder<Cliente.API.Models.Entities.Cliente> builder)
        {
            builder.ToTable("Cliente");

            builder.HasKey(p => p.id);

            
            builder.Property(m => m.id).HasColumnName("id").HasColumnType("uniqueidentifier").IsRequired();
            builder.Property(m => m.nome).HasColumnName("nome").HasColumnType("varchar(50)").IsRequired();
            builder.Property(m => m.numeroCpf).HasColumnName("numeroCpf").HasColumnType("varchar(11)").IsRequired();
            builder.Property(m => m.dataNascimento).HasColumnName("dataNascimento").HasColumnType("datetime").IsRequired();
            builder.Property(m => m.sexo).HasColumnName("sexo").HasColumnType("char(1)").IsRequired();

        }
    }
}
