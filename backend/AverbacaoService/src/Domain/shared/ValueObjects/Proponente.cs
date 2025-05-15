namespace AverbacaoService.Domain.shared.ValueObjects;

public record Proponente(Cpf Cpf, string Nome, string Sobrenome, DateTime DataNascimento);