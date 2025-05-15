namespace AverbacaoService.Domain.shared.ValueObjects;

public class Proposta
{
    // Constructor created only for EF Core
    private Proposta(){}

    public Proposta(int codigo, Convenio convenio, Proponente proponente, decimal valor, Prazo prazo)
    {
        Codigo = codigo;
        Convenio = convenio;
        Proponente = proponente;
        Valor = valor;
        Prazo = prazo;
    }

    public int Codigo { get; private set; }
    public Convenio Convenio { get; private set; }
    public Proponente Proponente { get; private set; }
    public decimal Valor { get; private set; }
    public Prazo Prazo { get; private set; }
}