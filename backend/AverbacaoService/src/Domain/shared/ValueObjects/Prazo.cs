namespace AverbacaoService.Domain.shared.ValueObjects;

public record Prazo
{
    // Constructor created only for EF Core
    private Prazo(){}
    public Prazo(int meses)
    {
        Meses = meses;
    }
    
    public int Meses { get; private set; }
}