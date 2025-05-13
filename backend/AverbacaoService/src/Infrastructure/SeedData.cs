using AverbacaoService.Domain.Averbacoes;
using AverbacaoService.shared.DatabaseDetails;
using AverbacaoService.shared.ValueObjects;
using CSharpFunctionalExtensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace AverbacaoService.Infrastructure;

public static class SeedData
{
    public static async Task Initialize(IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<AverbacaoDbContext>();

        // Ensure database is created and migrated
        await context.Database.MigrateAsync();

        // Check if we already have data
        if (await context.Averbacoes.AnyAsync())
            return;

        // Create sample data
        var averbacoes = new[]
        {
            CreateAverbacao(
                codigoProposta: 12345,
                cpf: "12345678900",
                nome: "João",
                sobrenome: "Silva",
                dataNascimento: new DateTime(1980, 1, 1),
                valor: 50000.00m,
                prazoMeses: 36
            ),
            CreateAverbacao(
                codigoProposta: 12346,
                cpf: "98765432100",
                nome: "Maria",
                sobrenome: "Santos",
                dataNascimento: new DateTime(1985, 6, 15),
                valor: 75000.00m,
                prazoMeses: 48
            ),
            CreateAverbacao(
                codigoProposta: 12347,
                cpf: "45678912300",
                nome: "Pedro",
                sobrenome: "Oliveira",
                dataNascimento: new DateTime(1990, 12, 31),
                valor: 100000.00m,
                prazoMeses: 60
            )
        };

        await context.Averbacoes.AddRangeAsync(averbacoes.Select(a => a.Value));
        await context.SaveChangesAsync();
    }

    private static Result<Averbacao> CreateAverbacao(
        int codigoProposta,
        string cpf,
        string nome,
        string sobrenome,
        DateTime dataNascimento,
        decimal valor,
        int prazoMeses)
    {
        var cpfObj = Cpf.Criar(cpf).Value;
        var proponente = new Proponente(cpfObj, nome, sobrenome, dataNascimento);
        var prazo = new Prazo(prazoMeses);
        var proposta = new Proposta(codigoProposta, proponente, valor, prazo);
        
        return Averbacao.Criar(proposta);
    }
}
