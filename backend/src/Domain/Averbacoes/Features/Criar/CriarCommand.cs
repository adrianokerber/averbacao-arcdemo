using AverbacaoService.shared.ValueObjects;
using CSharpFunctionalExtensions;

namespace AverbacaoService.Domain.Averbacoes.Features.Criar;

public class CriarCommand
{
    public Proposta Proposta { get; }
    
    private CriarCommand(Proposta proposta)
    {
        Proposta = proposta;
    }

    public static Result<CriarCommand> Criar(Proposta proposta)
    {
        if (proposta == null)
            return Result.Failure<CriarCommand>("Proposta inválida");
        
        return new CriarCommand(proposta);
    }
}