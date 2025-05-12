namespace AverbacaoService.shared.ValueObjects;

public class Proposta
{
    // Protected constructor for EF Core
    protected Proposta() { }

    public Proposta(int codigo, Proponente proponente, decimal valor, Prazo prazo)
    {
        Codigo = codigo;
        Proponente = proponente;
        Valor = valor;
        Prazo = prazo;
    }

    public int Codigo { get; private set; }
    public Proponente Proponente { get; private set; }
    public decimal Valor { get; private set; }
    public Prazo Prazo { get; private set; }
}