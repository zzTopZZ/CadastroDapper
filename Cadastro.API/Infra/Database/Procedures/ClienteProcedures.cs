namespace Cadastro.API.Infra.Database.Procedures;

public static class ClienteProcedures
{
    public const string ObterTodos = "sp_Cliente_Listar";
    public const string ObterPorId = "sp_Cliente_Obter";
    public const string Inserir    = "sp_Cliente_Inserir";
    public const string Atualizar  = "sp_Cliente_Atualizar";
    public const string Deletar    = "sp_Cliente_Deletar";
}
