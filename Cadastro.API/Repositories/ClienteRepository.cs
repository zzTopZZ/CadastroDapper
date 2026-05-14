using System.Data;
using Cadastro.API.Infra.Database;
using Cadastro.API.Infra.Database.Procedures;
using Cadastro.API.Models;
using Dapper;

namespace Cadastro.API.Repositories;

public class ClienteRepository : IClienteRepository
{
    private readonly IConnectionFactory _factory;

    public ClienteRepository(IConnectionFactory factory)
    {
        _factory = factory;
    }

    public async Task<IEnumerable<Cliente>> ObterTodos()
    {
        using var connection = _factory.CreateConnection();
        return await connection.QueryAsync<Cliente>(
            ClienteProcedures.ObterTodos,
            commandType: CommandType.StoredProcedure);
    }

    public async Task<Cliente?> ObterPorId(int id)
    {
        using var connection = _factory.CreateConnection();
        return await connection.QueryFirstOrDefaultAsync<Cliente>(
            ClienteProcedures.ObterPorId,
            new { Id = id },
            commandType: CommandType.StoredProcedure);
    }

    public async Task<int> Inserir(Cliente cliente)
    {
        using var connection = _factory.CreateConnection();
        return await connection.ExecuteScalarAsync<int>(
            ClienteProcedures.Inserir,
            new { cliente.Nome, cliente.Email, cliente.Telefone, cliente.CPF, cliente.DataNascimento },
            commandType: CommandType.StoredProcedure);
    }

    public async Task Atualizar(Cliente cliente)
    {
        using var connection = _factory.CreateConnection();
        await connection.ExecuteAsync(
            ClienteProcedures.Atualizar,
            new { cliente.Id, cliente.Nome, cliente.Email, cliente.Telefone, cliente.CPF, cliente.DataNascimento },
            commandType: CommandType.StoredProcedure);
    }

    public async Task Deletar(int id)
    {
        using var connection = _factory.CreateConnection();
        await connection.ExecuteAsync(
            ClienteProcedures.Deletar,
            new { Id = id },
            commandType: CommandType.StoredProcedure);
    }
}
