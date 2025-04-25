using Averbacao.shared.ValueObjects;
using CSharpFunctionalExtensions;

namespace Averbacao.modules.Inss.Domain.Propostas;

public class Proposta {
    public Guid Id { get; private set; }
    public Proponente Proponente { get; private set; }
    public decimal Valor { get; private set; }
    public List<Averbacao> Averbacoes { get; private set; }

    public Result IncluirAverbacao()
    {
        throw new NotImplementedException();
    }

    public Result RemoverAverbacao()
    {
        throw new NotImplementedException();
    }
}

public record Averbacao(int Codigo, string Resultado);

public record Proponente(Cpf Cpf, string Nome, string Sobrenome, DateTime DataNascimento);