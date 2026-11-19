USE SkaL4ble;
GO

IF NOT EXISTS (
    SELECT 1
    FROM TipoUsuario
    WHERE CodigoTipoUsuario = 'TECH_LEAD'
)
BEGIN
    INSERT INTO TipoUsuario (CodigoTipoUsuario, NomeTipoUsuario)
    VALUES ('TECH_LEAD', 'Tech Lead');
END;
GO

IF NOT EXISTS (
    SELECT 1
    FROM TipoUsuario
    WHERE CodigoTipoUsuario = 'COLABORADOR'
)
BEGIN
    INSERT INTO TipoUsuario (CodigoTipoUsuario, NomeTipoUsuario)
    VALUES ('COLABORADOR', 'Colaborador');
END;
GO