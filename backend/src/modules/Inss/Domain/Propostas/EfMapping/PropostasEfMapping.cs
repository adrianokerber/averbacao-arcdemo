using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Averbacao.modules.Inss.Domain.Propostas.EfMapping;

public class PropostasEfMapping : IEntityTypeConfiguration<Proposta>
{
        public void Configure(EntityTypeBuilder<Proposta> builder)
        {
            builder.ToTable("Propostas", "Averbacao")
                    .HasKey(x => x.Id);

            builder.Property(x => x.Valor)
                    .IsRequired()
                    .HasColumnName("Valor");
            
            builder.OwnsOne(x => x.Proponente, proponente =>
            {
                proponente.Property(x => x.Cpf)
                        .IsRequired()
                        .HasColumnType("VARCHAR(15)");
                
                proponente.Property(x => x.DataNascimento)
                        .IsRequired()
                        .HasColumnType("DATETIME");
            });
        }
}