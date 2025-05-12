namespace AverbacaoService.shared.ValueObjects;

public record Prazo
{
    // Protected constructor for EF Core
    private Prazo(){}
    public Prazo(int meses)
    {
        Meses = meses;
    }
    
    public int Meses { get; private set; }
}