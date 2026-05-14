namespace Cadastro.API.DTOs;

public record ClienteRequest(
    string Nome,
    string Email,
    string? Telefone,
    string CPF,
    DateTime? DataNascimento
);

public record ClienteResponse(
    int Id,
    string Nome,
    string Email,
    string? Telefone,
    string CPF,
    DateTime? DataNascimento,
    DateTime DataCadastro,
    bool Ativo
);
