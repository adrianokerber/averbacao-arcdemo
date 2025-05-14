using CSharpFunctionalExtensions;

namespace AverbacaoService.Domain.Averbacoes.Features.Formalizar;

public class FormalizarCommand
{
    public int CodigoProposta { get; }
    
    private FormalizarCommand(int codigoProposta)
    {
        CodigoProposta = codigoProposta;
    }

    public static Result<FormalizarCommand> Criar(int codigoProposta)
    {
        if (codigoProposta <= 0)
            return Result.Failure<FormalizarCommand>("Código da proposta inválido");
        
        return new FormalizarCommand(codigoProposta);
    }
}