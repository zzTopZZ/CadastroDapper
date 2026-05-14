using Cadastro.API.Models;

namespace Cadastro.API.Repositories;

public interface IClienteRepository
{
    Task<IEnumerable<Cliente>> ObterTodos();
    Task<Cliente?> ObterPorId(int id);
    Task<int> Inserir(Cliente cliente);
    Task Atualizar(Cliente cliente);
    Task Deletar(int id);
}
