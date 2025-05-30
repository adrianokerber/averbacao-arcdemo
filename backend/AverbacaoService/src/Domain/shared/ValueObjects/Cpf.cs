using CSharpFunctionalExtensions;

namespace AverbacaoService.Domain.shared.ValueObjects;

public record Cpf
{
    // Constructor created only for EF Core
    private Cpf(){}
    
    public string Valor { get; private set; }

    private Cpf(string valor)
    {
        Valor = valor;
    }
    
    public static Result<Cpf> Criar(string valor)
    {
        if (valor == null)
            return Result.Failure<Cpf>("Valor não informado");
        if (valor.Length != 11)
            return Result.Failure<Cpf>("Quantidade de caracteres inválida");
        
        return new Cpf(valor);
    }
}