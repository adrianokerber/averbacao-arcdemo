using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AverbacaoService.Domain.Averbacoes.EfMapping;

public class AverbacoesEfMapping : IEntityTypeConfiguration<Averbacao>
{
        public void Configure(EntityTypeBuilder<Averbacao> builder)
        {
            builder.ToTable("Averbacoes", "AverbacaoService")
                    .HasKey(x => x.Id);

            builder.Property(x => x.Status)
                .IsRequired()
                .HasColumnName("Status");
            
            builder.OwnsOne(x => x.Proposta, proposta =>
            {
                proposta.Property(x => x.Codigo)
                        .IsRequired()
                        .HasColumnType("VARCHAR(15)");
                
                proposta.Property(x => x.Valor)
                    .IsRequired()
                    .HasColumnType("DECIMAL(18,2)");

                proposta.OwnsOne(x => x.Proponente, proponente =>
                {
                    proponente.Property(x => x.Cpf)
                        .IsRequired()
                        .HasColumnType("VARCHAR(15)");
                    
                    proponente.Property(x => x.DataNascimento)
                        .IsRequired()
                        .HasColumnType("DATETIME");
                });
            });
        }
}