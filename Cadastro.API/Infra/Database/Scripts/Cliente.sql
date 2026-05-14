IF NOT EXISTS (SELECT name FROM sys.databases WHERE name = 'DapperCliente')
    CREATE DATABASE DapperCliente;
GO

USE DapperCliente;
GO

-- =============================================
-- Tabela Cliente
-- =============================================
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Cliente')
BEGIN
    CREATE TABLE Cliente (
        Id              INT PRIMARY KEY IDENTITY(1,1),
        Nome            VARCHAR(100)  NOT NULL,
        Email           VARCHAR(150)  NOT NULL,
        Telefone        VARCHAR(20),
        CPF             CHAR(11)      NOT NULL,
        DataNascimento  DATE,
        DataCadastro    DATETIME      DEFAULT GETDATE(),
        Ativo           BIT           DEFAULT 1
    );
END
GO

-- =============================================
-- sp_Cliente_Listar
-- =============================================
IF NOT EXISTS (SELECT * FROM sys.procedures WHERE name = 'sp_Cliente_Listar')
BEGIN
    EXEC('CREATE PROCEDURE sp_Cliente_Listar AS BEGIN SET NOCOUNT ON; SELECT Id, Nome, Email, Telefone, CPF, DataNascimento, DataCadastro, Ativo FROM Cliente WHERE Ativo = 1; END');
END
GO

-- =============================================
-- sp_Cliente_Obter
-- =============================================
IF NOT EXISTS (SELECT * FROM sys.procedures WHERE name = 'sp_Cliente_Obter')
BEGIN
    EXEC('CREATE PROCEDURE sp_Cliente_Obter @Id INT AS BEGIN SET NOCOUNT ON; SELECT Id, Nome, Email, Telefone, CPF, DataNascimento, DataCadastro, Ativo FROM Cliente WHERE Id = @Id; END');
END
GO

-- =============================================
-- sp_Cliente_Inserir
-- =============================================
IF NOT EXISTS (SELECT * FROM sys.procedures WHERE name = 'sp_Cliente_Inserir')
BEGIN
    EXEC('CREATE PROCEDURE sp_Cliente_Inserir @Nome VARCHAR(100), @Email VARCHAR(150), @Telefone VARCHAR(20), @CPF CHAR(11), @DataNascimento DATE AS BEGIN SET NOCOUNT ON; INSERT INTO Cliente (Nome, Email, Telefone, CPF, DataNascimento) VALUES (@Nome, @Email, @Telefone, @CPF, @DataNascimento); SELECT SCOPE_IDENTITY(); END');
END
GO

-- =============================================
-- sp_Cliente_Atualizar
-- =============================================
IF NOT EXISTS (SELECT * FROM sys.procedures WHERE name = 'sp_Cliente_Atualizar')
BEGIN
    EXEC('CREATE PROCEDURE sp_Cliente_Atualizar @Id INT, @Nome VARCHAR(100), @Email VARCHAR(150), @Telefone VARCHAR(20), @CPF CHAR(11), @DataNascimento DATE AS BEGIN SET NOCOUNT ON; UPDATE Cliente SET Nome = @Nome, Email = @Email, Telefone = @Telefone, CPF = @CPF, DataNascimento = @DataNascimento WHERE Id = @Id; END');
END
GO

-- =============================================
-- sp_Cliente_Deletar (soft delete)
-- =============================================
IF NOT EXISTS (SELECT * FROM sys.procedures WHERE name = 'sp_Cliente_Deletar')
BEGIN
    EXEC('CREATE PROCEDURE sp_Cliente_Deletar @Id INT AS BEGIN SET NOCOUNT ON; UPDATE Cliente SET Ativo = 0 WHERE Id = @Id; END');
END
GO
