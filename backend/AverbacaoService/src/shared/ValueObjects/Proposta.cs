namespace AverbacaoService.shared.ValueObjects;

public record Proposta(int Codigo, Proponente Proponente, decimal Valor, Prazo Prazo);