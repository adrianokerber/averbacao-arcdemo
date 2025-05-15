using AverbacaoService.Domain.shared.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AverbacaoService.Domain.Averbacoes.Infraestructure.EfMapping;

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
        
        builder.OwnsOne(x => x.Formalizacao, formalizacao =>
        {
            formalizacao.Property(x => x.CodigoIntegracao)
                .IsRequired()
                .HasColumnName("FormalizacaoCodigoIntegracao")
                .HasColumnType("INT");

            formalizacao.Property(x => x.Data)
                .IsRequired()
                .HasColumnName("FormalizacaoData")
                .HasColumnType("DATETIME");

            formalizacao.Property(x => x.Detalhes)
                .IsRequired()
                .HasColumnName("FormalizacaoDetalhes")
                .HasColumnType("VARCHAR(500)");
        });
        
        builder.OwnsOne(x => x.Proposta, proposta =>
        {
            proposta.Property(x => x.Codigo)
                    .IsRequired()
                    .HasColumnType("VARCHAR(15)");
            
            proposta.Property(x => x.Convenio)
                .IsRequired()
                .HasColumnName("Convenio")
                .HasConversion(
                    v => v.Nome,
                    v => Convenio.Criar(v).Value
                );
            
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
            "FORMALIZADA" => Status.Formalizada,
            "CANCELADA" => Status.Cancelada,
            _ => throw new ArgumentException($"Unknown status value: {value}")
        };
    }
}