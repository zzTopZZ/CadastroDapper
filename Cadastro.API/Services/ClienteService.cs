using Cadastro.API.DTOs;
using Cadastro.API.Infra.Common;
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

    public async Task<Result<IEnumerable<ClienteResponse>>> ObterTodos()
    {
        var clientes = await _repository.ObterTodos();
        return Result<IEnumerable<ClienteResponse>>.Success(clientes.Select(ToResponse));
    }

    public async Task<Result<ClienteResponse>> ObterPorId(int id)
    {
        var cliente = await _repository.ObterPorId(id);
        if (cliente is null)
            return Result<ClienteResponse>.Failure($"Cliente {id} não encontrado.");

        return Result<ClienteResponse>.Success(ToResponse(cliente));
    }

    public async Task<Result<int>> Inserir(ClienteRequest request)
    {
        var id = await _repository.Inserir(ToModel(request));
        return Result<int>.Success(id);
    }

    public async Task<Result> Atualizar(int id, ClienteRequest request)
    {
        var cliente = await _repository.ObterPorId(id);
        if (cliente is null)
            return Result.Failure($"Cliente {id} não encontrado.");

        var model = ToModel(request);
        model.Id = id;
        await _repository.Atualizar(model);
        return Result.Success();
    }

    public async Task<Result> Deletar(int id)
    {
        var cliente = await _repository.ObterPorId(id);
        if (cliente is null)
            return Result.Failure($"Cliente {id} não encontrado.");

        await _repository.Deletar(id);
        return Result.Success();
    }

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
