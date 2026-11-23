USE SkaL4ble;
GO

-- =============================================================================
-- CONSULTA RAPIDA DE IDS PARA CRIACAO, ATUALIZACAO E PATCHS
-- =============================================================================

SELECT
    u.UsuarioId,
    u.NomeUsuario,
    u.EmailPessoal,
    CASE
        WHEN u.Senha IS NULL THEN 'Pendente'
        ELSE 'Definida'
    END AS Senha,
    u.Ativo,

    tipo.TipoUsuarioId,
    tipo.CodigoTipoUsuario,
    tipo.NomeTipoUsuario,

    u.CargoId,
    cargo.NomeCargo,

    u.AtuacaoId,
    atuacao.NomeAtuacao,

    projeto.ProjetoId,
    projeto.NomeProjeto,

    ticket.TicketId,
    ticket.NomeTicket,
    ticket.TipoTicketId,
    tipoTicket.NomeTipoTicket,
    ticket.StatusTicketId,
    statusTicket.NomeStatusTicket,

    ticketUsuario.Autor
FROM dbo.Usuario AS u
JOIN dbo.TipoUsuario AS tipo
    ON tipo.TipoUsuarioId = u.TipoUsuarioId
JOIN dbo.Cargo AS cargo
    ON cargo.CargoId = u.CargoId
JOIN dbo.Atuacao AS atuacao
    ON atuacao.AtuacaoId = u.AtuacaoId
LEFT JOIN dbo.Ticket AS ticket
    ON ticket.UsuarioResponsavel = u.UsuarioId
LEFT JOIN dbo.Projeto AS projeto
    ON projeto.ProjetoId = ticket.ProjetoId
LEFT JOIN dbo.TipoTicket AS tipoTicket
    ON tipoTicket.TipoTicketId = ticket.TipoTicketId
LEFT JOIN dbo.StatusTicket AS statusTicket
    ON statusTicket.StatusTicketId = ticket.StatusTicketId
LEFT JOIN dbo.TicketUsuario AS ticketUsuario
    ON ticketUsuario.TicketId = ticket.TicketId
    AND ticketUsuario.UsuarioId = u.UsuarioId
WHERE u.UsuarioId IN (
    'B0000000-0000-0000-0000-000000000001',
    'B0000000-0000-0000-0000-000000000002',
    'B0000000-0000-0000-0000-000000000003'
)
ORDER BY u.NomeUsuario, ticket.TicketId;
GO