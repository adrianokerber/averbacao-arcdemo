using CSharpFunctionalExtensions;

namespace AverbacaoService.Domain.shared.ValueObjects;

public record Convenio
{
    // Constructor created only for EF Core
    private Convenio(){}
    
    public string Nome { get; }
    
    private Convenio(string nome)
    {
        Nome = nome.ToUpper();
    }

    public static Result<Convenio> Criar(string nome)
    {
        return nome switch
        {
            "INSS" => Inss,
            _ => Result.Failure<Convenio>("Convenio inexistente")
        };
    }
    
    public static Convenio Inss
        => new Convenio("Inss");
    
    public static Convenio Siape
        => new Convenio("Siape");
    
    public static Convenio CreditoTrabalhador
        => new Convenio("CreditoTrabalhador");
}