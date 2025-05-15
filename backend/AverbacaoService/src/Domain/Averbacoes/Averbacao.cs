using AverbacaoService.Domain.shared.ValueObjects;
using AverbacaoService.shared.Extensions;
using CSharpFunctionalExtensions;

namespace AverbacaoService.Domain.Averbacoes;

public class Averbacao {
    // Constructor created only for EF Core
    private Averbacao(){}
    
    public Guid Id { get; private set; }
    public Status Status { get; private set; }
    public Proposta Proposta { get; private set; }
    public Formalizacao? Formalizacao { get; private set; }

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
        if (proposta.Convenio == null)
            return Result.Failure<Averbacao>("Convênio não informado");
        if (proposta.Proponente == null)
            return Result.Failure<Averbacao>("Proponente não informado para criar averbação");
        if (proposta.Proponente.Nome.IsNullOrEmpty() || proposta.Proponente.Sobrenome.IsNullOrEmpty())
            return Result.Failure<Averbacao>("Nome e sobrenome do proponente devem ser informados");
        if (proposta.Valor <= 0)
            return Result.Failure<Averbacao>("Valor da proposta inválido");
        if (proposta.Prazo.Meses <= 0)
            return Result.Failure<Averbacao>("Prazo da proposta inválido");

        var id = Guid.NewGuid();
        var status = Status.Criada;
        
        return new Averbacao(id, status, proposta);
    }

    public Result Formalizar(Formalizacao formalizacao)
    {
        if (formalizacao == null || formalizacao.CodigoIntegracao <= 0)
            return Result.Failure("Formalização inválida");
        if (EstaFormalizada())
            return Result.Failure("Averbação formalizada");

        Formalizacao = formalizacao;
        Status = Status.Formalizada;
        
        return Result.Success();
    }

    public bool EstaFormalizada() => Formalizacao != null;
}

public record Formalizacao(int CodigoIntegracao, DateTime Data, string Detalhes);

public record Status
{
    public string Label { get; }
    
    private Status(string label)
    {
        Label = label.ToUpper();
    }
    
    public static Status Criada
        => new Status("CRIADA");
    
    public static Status Formalizada
        => new Status("FORMALIZADA");
    
    public static Status Cancelada
        => new Status("CANCELADA");
}