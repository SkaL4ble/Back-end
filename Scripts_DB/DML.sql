USE SkaL4ble;
GO

-- =============================================================================
-- ATRIBUTOS INERENTES DO SISTEMA
-- =============================================================================

-- __________________________________TipoUsuario__________________________________
IF NOT EXISTS (
    SELECT 1 FROM dbo.TipoUsuario WHERE CodigoTipoUsuario = 'Tech Lead'
)
BEGIN
    INSERT INTO dbo.TipoUsuario (CodigoTipoUsuario, NomeTipoUsuario)
    VALUES ('Tech Lead', 'Tech Lead');
END;

IF NOT EXISTS (
    SELECT 1 FROM dbo.TipoUsuario WHERE CodigoTipoUsuario = 'Colaborador'
)
BEGIN
    INSERT INTO dbo.TipoUsuario (CodigoTipoUsuario, NomeTipoUsuario)
    VALUES ('Colaborador', 'Colaborador');
END;

-- __________________________________Atuacao______________________________________
IF NOT EXISTS (
    SELECT 1 FROM dbo.Atuacao WHERE NomeAtuacao = 'Backend'
)
BEGIN
    INSERT INTO dbo.Atuacao (NomeAtuacao) VALUES ('Backend');
END;

IF NOT EXISTS (
    SELECT 1 FROM dbo.Atuacao WHERE NomeAtuacao = 'Frontend'
)
BEGIN
    INSERT INTO dbo.Atuacao (NomeAtuacao) VALUES ('Frontend');
END;

IF NOT EXISTS (
    SELECT 1 FROM dbo.Atuacao WHERE NomeAtuacao = 'Fullstack'
)
BEGIN
    INSERT INTO dbo.Atuacao (NomeAtuacao) VALUES ('Fullstack');
END;

-- __________________________________Cargo________________________________________
IF NOT EXISTS (
    SELECT 1 FROM dbo.Cargo WHERE NomeCargo = 'Estagiario'
)
BEGIN
    INSERT INTO dbo.Cargo (NomeCargo) VALUES ('Estagiario');
END;

IF NOT EXISTS (
    SELECT 1 FROM dbo.Cargo WHERE NomeCargo = 'Desenvolvedor Junior'
)
BEGIN
    INSERT INTO dbo.Cargo (NomeCargo) VALUES ('Desenvolvedor Junior');
END;

IF NOT EXISTS (
    SELECT 1 FROM dbo.Cargo WHERE NomeCargo = 'Desenvolvedor Pleno'
)
BEGIN
    INSERT INTO dbo.Cargo (NomeCargo) VALUES ('Desenvolvedor Pleno');
END;

IF NOT EXISTS (
    SELECT 1 FROM dbo.Cargo WHERE NomeCargo = 'Desenvolvedor Senior'
)
BEGIN
    INSERT INTO dbo.Cargo (NomeCargo) VALUES ('Desenvolvedor Senior');
END;