USE SkaL4ble;
GO

-- =============================================================================
-- CONSULTA RAPIDA DE IDS PARA CRIACAO, ATUALIZACAO E PATCHS
-- =============================================================================
SELECT TipoUsuarioId, CodigoTipoUsuario, NomeTipoUsuario
FROM dbo.TipoUsuario
WHERE CodigoTipoUsuario IN ('Tech Lead', 'Colaborador')
ORDER BY CodigoTipoUsuario;

SELECT AtuacaoId, NomeAtuacao
FROM dbo.Atuacao
WHERE NomeAtuacao IN ('Backend', 'Frontend', 'Fullstack')
ORDER BY AtuacaoId;

SELECT CargoId, NomeCargo
FROM dbo.Cargo
WHERE NomeCargo IN (
    'Estagiario',
    'Desenvolvedor Junior',
    'Desenvolvedor Pleno',
    'Desenvolvedor Senior'
)
ORDER BY CargoId;
GO