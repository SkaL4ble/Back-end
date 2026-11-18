USE SkaL4ble;
GO

SET XACT_ABORT ON;
BEGIN TRANSACTION;

-- Tipos de usuario necessarios para o cadastro via API.
IF NOT EXISTS (
    SELECT 1 FROM dbo.TipoUsuario WHERE CodigoTipoUsuario = 'TECH_LEAD'
)
BEGIN
    INSERT INTO dbo.TipoUsuario (CodigoTipoUsuario, NomeTipoUsuario)
    VALUES ('TECH_LEAD', 'Tech Lead');
END;

IF NOT EXISTS (
    SELECT 1 FROM dbo.TipoUsuario WHERE CodigoTipoUsuario = 'COLABORADOR'
)
BEGIN
    INSERT INTO dbo.TipoUsuario (CodigoTipoUsuario, NomeTipoUsuario)
    VALUES ('COLABORADOR', 'Colaborador');
END;

-- Valores de referencia para testar criacao e atualizacao.
IF NOT EXISTS (
    SELECT 1 FROM dbo.Cargo WHERE NomeCargo = 'Cargo Teste A'
)
BEGIN
    INSERT INTO dbo.Cargo (NomeCargo) VALUES ('Cargo Teste A');
END;

IF NOT EXISTS (
    SELECT 1 FROM dbo.Cargo WHERE NomeCargo = 'Cargo Teste B'
)
BEGIN
    INSERT INTO dbo.Cargo (NomeCargo) VALUES ('Cargo Teste B');
END;

IF NOT EXISTS (
    SELECT 1 FROM dbo.Atuacao WHERE NomeAtuacao = 'Atuacao Teste A'
)
BEGIN
    INSERT INTO dbo.Atuacao (NomeAtuacao) VALUES ('Atuacao Teste A');
END;

IF NOT EXISTS (
    SELECT 1 FROM dbo.Atuacao WHERE NomeAtuacao = 'Atuacao Teste B'
)
BEGIN
    INSERT INTO dbo.Atuacao (NomeAtuacao) VALUES ('Atuacao Teste B');
END;

DECLARE @TipoColaborador UNIQUEIDENTIFIER = (
    SELECT TipoUsuarioId
    FROM dbo.TipoUsuario
    WHERE CodigoTipoUsuario = 'COLABORADOR'
);
DECLARE @CargoA INT = (SELECT CargoId FROM dbo.Cargo WHERE NomeCargo = 'Cargo Teste A');
DECLARE @CargoB INT = (SELECT CargoId FROM dbo.Cargo WHERE NomeCargo = 'Cargo Teste B');
DECLARE @AtuacaoA INT = (SELECT AtuacaoId FROM dbo.Atuacao WHERE NomeAtuacao = 'Atuacao Teste A');
DECLARE @AtuacaoB INT = (SELECT AtuacaoId FROM dbo.Atuacao WHERE NomeAtuacao = 'Atuacao Teste B');
DECLARE @UsuarioTeste1 UNIQUEIDENTIFIER = 'A0000000-0000-0000-0000-000000000001';
DECLARE @UsuarioTeste2 UNIQUEIDENTIFIER = 'A0000000-0000-0000-0000-000000000002';

IF @TipoColaborador IS NULL
    THROW 50001, 'Tipo COLABORADOR nao foi encontrado em TipoUsuario.', 1;

-- Fixtures existentes para testar PUT, PATCH de senha, inativacao e duplicidades.
IF NOT EXISTS (
    SELECT 1 FROM dbo.Usuario WHERE UsuarioId = @UsuarioTeste1
)
BEGIN
    INSERT INTO dbo.Usuario
        (UsuarioId, NomeUsuario, EmailPessoal, TipoUsuarioId, CargoId, AtuacaoId)
    VALUES
        (@UsuarioTeste1, 'Usuario Teste A', 'usuario.teste.a@example.com', @TipoColaborador, @CargoA, @AtuacaoA);
END;

IF NOT EXISTS (
    SELECT 1 FROM dbo.Usuario WHERE UsuarioId = @UsuarioTeste2
)
BEGIN
    INSERT INTO dbo.Usuario
        (UsuarioId, NomeUsuario, EmailPessoal, TipoUsuarioId, CargoId, AtuacaoId)
    VALUES
        (@UsuarioTeste2, 'Usuario Teste B', 'usuario.teste.b@example.com', @TipoColaborador, @CargoB, @AtuacaoB);
END;

IF NOT EXISTS (
    SELECT 1 FROM dbo.UsuarioEmailGit WHERE EmailGit = 'git.teste.a@example.com'
)
BEGIN
    INSERT INTO dbo.UsuarioEmailGit (UsuarioId, EmailGit)
    VALUES (@UsuarioTeste1, 'git.teste.a@example.com');
END;

IF NOT EXISTS (
    SELECT 1 FROM dbo.UsuarioEmailGit WHERE EmailGit = 'git.teste.b@example.com'
)
BEGIN
    INSERT INTO dbo.UsuarioEmailGit (UsuarioId, EmailGit)
    VALUES (@UsuarioTeste2, 'git.teste.b@example.com');
END;

COMMIT TRANSACTION;
GO

-- IDs e valores para usar nos testes via Swagger.
SELECT
    u.UsuarioId,
    u.NomeUsuario,
    u.EmailPessoal,
    u.TipoUsuarioId,
    tu.CodigoTipoUsuario,
    u.CargoId,
    c.NomeCargo,
    u.AtuacaoId,
    a.NomeAtuacao,
    eg.EmailGit
FROM dbo.Usuario AS u
JOIN dbo.TipoUsuario AS tu ON tu.TipoUsuarioId = u.TipoUsuarioId
JOIN dbo.Cargo AS c ON c.CargoId = u.CargoId
JOIN dbo.Atuacao AS a ON a.AtuacaoId = u.AtuacaoId
LEFT JOIN dbo.UsuarioEmailGit AS eg ON eg.UsuarioId = u.UsuarioId
WHERE u.UsuarioId IN (
    'A0000000-0000-0000-0000-000000000001',
    'A0000000-0000-0000-0000-000000000002'
)
ORDER BY u.NomeUsuario;
GO
