using Cadastro.API.DTOs;

namespace Cadastro.API.Services;

public interface IClienteService
{
    Task<IEnumerable<ClienteResponse>> ObterTodos();
    Task<ClienteResponse?> ObterPorId(int id);
    Task<int> Inserir(ClienteRequest request);
    Task Atualizar(int id, ClienteRequest request);
    Task Deletar(int id);
}
