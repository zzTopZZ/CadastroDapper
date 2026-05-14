using Cadastro.API.DTOs;
using Cadastro.API.Infra.Common;

namespace Cadastro.API.Services;

public interface IClienteService
{
    Task<Result<IEnumerable<ClienteResponse>>> ObterTodos();
    Task<Result<ClienteResponse>> ObterPorId(int id);
    Task<Result<int>> Inserir(ClienteRequest request);
    Task<Result> Atualizar(int id, ClienteRequest request);
    Task<Result> Deletar(int id);
}
