using Cadastro.API.DTOs;
using Cadastro.API.Models;
using Cadastro.API.Repositories;

namespace Cadastro.API.Services;

public class ClienteService : IClienteService
{
    private readonly IClienteRepository _repository;

    public ClienteService(IClienteRepository repository)
    {
        _repository = repository;
    }

    public async Task<IEnumerable<ClienteResponse>> ObterTodos()
    {
        var clientes = await _repository.ObterTodos();
        return clientes.Select(ToResponse);
    }

    public async Task<ClienteResponse?> ObterPorId(int id)
    {
        var cliente = await _repository.ObterPorId(id);
        return cliente is null ? null : ToResponse(cliente);
    }

    public Task<int> Inserir(ClienteRequest request)
    {
        return _repository.Inserir(ToModel(request));
    }

    public Task Atualizar(int id, ClienteRequest request)
    {
        var cliente = ToModel(request);
        cliente.Id = id;
        return _repository.Atualizar(cliente);
    }

    public Task Deletar(int id) => _repository.Deletar(id);

    private static Cliente ToModel(ClienteRequest r) => new()
    {
        Nome = r.Nome,
        Email = r.Email,
        Telefone = r.Telefone,
        CPF = r.CPF,
        DataNascimento = r.DataNascimento
    };

    private static ClienteResponse ToResponse(Cliente c) => new(
        c.Id, c.Nome, c.Email, c.Telefone, c.CPF, c.DataNascimento, c.DataCadastro, c.Ativo);
}
