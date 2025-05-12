using AverbacaoService.shared.ValueObjects;
using AverbacaoService.shared.Extensions;
using CSharpFunctionalExtensions;

namespace AverbacaoService.Domain.Averbacoes;

public class Averbacao {
    public Guid Id { get; private set; }
    public Status Status { get; private set; }
    public Proposta Proposta { get; private set; }

    // Protected constructor for EF Core
    protected Averbacao() { }

    private Averbacao(Guid id, Status status, Proposta proposta)
    {
        Id = id;
        Status = status;
        Proposta = proposta;
    }

    public static Result<Averbacao> Criar(Proposta proposta)
    {
        if (proposta == null)
            return Result.Failure<Averbacao>("Proposta não informada para criar averbação");
        if (proposta.Codigo <= 0)
            return Result.Failure<Averbacao>("Código da proposta inválido");
        if (proposta.Proponente == null)
            return Result.Failure<Averbacao>("Proponente não informado para criar averbação");
        if (proposta.Proponente.Nome.IsNullOrEmpty() || proposta.Proponente.Sobrenome.IsNullOrEmpty())
            return Result.Failure<Averbacao>("Nome e sobrenome do proponente devem ser informados");
        if (proposta.Valor <= 0)
            return Result.Failure<Averbacao>("Valor da proposta inválido");
        if (proposta.Prazo.Meses <= 0)
            return Result.Failure<Averbacao>("Prazo da proposta inválido");

        var id = Guid.NewGuid();
        var status = Status.Recebida;
        
        return new Averbacao(id, status, proposta);
    }
}

public record Status
{
    public string Label { get; }
    
    private Status(string label)
    {
        Label = label.ToUpper();
    }
    
    public static Status Recebida
        => new Status("Recebida");
    
    public static Status Recusada
        => new Status("Recusada");
}