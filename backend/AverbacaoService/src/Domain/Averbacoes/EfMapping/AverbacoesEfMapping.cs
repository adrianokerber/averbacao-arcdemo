using AverbacaoService.shared.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AverbacaoService.Domain.Averbacoes.EfMapping;

public class AverbacoesEfMapping : IEntityTypeConfiguration<Averbacao>
{
    public void Configure(EntityTypeBuilder<Averbacao> builder)
    {
        builder.ToTable("Averbacoes", "AverbacaoService");
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Status)
            .IsRequired()
            .HasColumnName("Status")
            .HasConversion(
                v => v.Label,
                v => ConvertToStatus(v)
            );
        
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
                    .HasColumnType("VARCHAR(15)")
                    .HasConversion(
                        cpf => cpf.Valor,
                        valor => Cpf.Criar(valor).Value
                    );
                
                proponente.Property(x => x.DataNascimento)
                    .IsRequired()
                    .HasColumnType("DATETIME");

                proponente.Property(x => x.Nome)
                    .IsRequired()
                    .HasColumnType("VARCHAR(100)");

                proponente.Property(x => x.Sobrenome)
                    .IsRequired()
                    .HasColumnType("VARCHAR(100)");
            });

            proposta.OwnsOne(x => x.Prazo, prazo =>
            {
                prazo.Property(x => x.Meses)
                    .IsRequired()
                    .HasColumnType("INT");
            });
        });
    }

    private static Status ConvertToStatus(string value)
    {
        return value switch
        {
            "CRIADA" => Status.Criada,
            "CANCELADA" => Status.Cancelada,
            "PROCESSADA" => Status.Processada,
            _ => throw new ArgumentException($"Unknown status value: {value}")
        };
    }
}