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

-- __________________________________TipoTicket___________________________________
IF NOT EXISTS (
    SELECT 1 FROM dbo.TipoTicket WHERE NomeTipoTicket = 'Alta Prioridade'
)
BEGIN
    INSERT INTO dbo.TipoTicket (NomeTipoTicket) VALUES ('Alta Prioridade');
END;

IF NOT EXISTS (
    SELECT 1 FROM dbo.TipoTicket WHERE NomeTipoTicket = 'Media Prioridade'
)
BEGIN
    INSERT INTO dbo.TipoTicket (NomeTipoTicket) VALUES ('Media Prioridade');
END;

IF NOT EXISTS (
    SELECT 1 FROM dbo.TipoTicket WHERE NomeTipoTicket = 'Baixa Prioridade'
)
BEGIN
    INSERT INTO dbo.TipoTicket (NomeTipoTicket) VALUES ('Baixa Prioridade');
END;

-- __________________________________StatusTicket_________________________________
IF NOT EXISTS (
    SELECT 1 FROM dbo.StatusTicket WHERE NomeStatusTicket = 'Concluido'
)
BEGIN
    INSERT INTO dbo.StatusTicket (NomeStatusTicket) VALUES ('Concluido');
END;

IF NOT EXISTS (
    SELECT 1 FROM dbo.StatusTicket WHERE NomeStatusTicket = 'Em Andamento'
)
BEGIN
    INSERT INTO dbo.StatusTicket (NomeStatusTicket) VALUES ('Em Andamento');
END;

IF NOT EXISTS (
    SELECT 1 FROM dbo.StatusTicket WHERE NomeStatusTicket = 'Pendente'
)
BEGIN
    INSERT INTO dbo.StatusTicket (NomeStatusTicket) VALUES ('Pendente');
END;

IF NOT EXISTS (
    SELECT 1 FROM dbo.StatusTicket WHERE NomeStatusTicket = 'Transferencia'
)
BEGIN
    INSERT INTO dbo.StatusTicket (NomeStatusTicket) VALUES ('Transferencia');
END;

-- =============================================================================
-- POPULACAO DE DADOS
-- =============================================================================

IF NOT EXISTS (SELECT 1 FROM dbo.TipoProjeto WHERE NomeTipoProjeto = 'Projeto Teste')
    INSERT INTO dbo.TipoProjeto (NomeTipoProjeto) VALUES ('Projeto Teste');

IF NOT EXISTS (SELECT 1 FROM dbo.StatusProjeto WHERE NomeStatusProjeto = 'Em Andamento')
    INSERT INTO dbo.StatusProjeto (NomeStatusProjeto) VALUES ('Em Andamento');

-- __________________________________Usuarios____________________________________
IF NOT EXISTS (SELECT 1 FROM dbo.Usuario WHERE UsuarioId = 'B0000000-0000-0000-0000-000000000001')
    INSERT INTO dbo.Usuario (UsuarioId, NomeUsuario, EmailPessoal, TipoUsuarioId, CargoId, AtuacaoId)
    VALUES ('B0000000-0000-0000-0000-000000000001', 'Tech Lead Teste', 'tech.lead.teste@example.com',
        (SELECT TipoUsuarioId FROM dbo.TipoUsuario WHERE CodigoTipoUsuario = 'Tech Lead'),
        (SELECT CargoId FROM dbo.Cargo WHERE NomeCargo = 'Desenvolvedor Senior'),
        (SELECT AtuacaoId FROM dbo.Atuacao WHERE NomeAtuacao = 'Fullstack'));

IF NOT EXISTS (SELECT 1 FROM dbo.Usuario WHERE UsuarioId = 'B0000000-0000-0000-0000-000000000002')
    INSERT INTO dbo.Usuario (UsuarioId, NomeUsuario, EmailPessoal, TipoUsuarioId, CargoId, AtuacaoId)
    VALUES ('B0000000-0000-0000-0000-000000000002', 'Colaborador Teste 1', 'colaborador1.teste@example.com',
        (SELECT TipoUsuarioId FROM dbo.TipoUsuario WHERE CodigoTipoUsuario = 'Colaborador'),
        (SELECT CargoId FROM dbo.Cargo WHERE NomeCargo = 'Desenvolvedor Junior'),
        (SELECT AtuacaoId FROM dbo.Atuacao WHERE NomeAtuacao = 'Backend'));

IF NOT EXISTS (SELECT 1 FROM dbo.Usuario WHERE UsuarioId = 'B0000000-0000-0000-0000-000000000003')
    INSERT INTO dbo.Usuario (UsuarioId, NomeUsuario, EmailPessoal, TipoUsuarioId, CargoId, AtuacaoId)
    VALUES ('B0000000-0000-0000-0000-000000000003', 'Colaborador Teste 2', 'colaborador2.teste@example.com',
        (SELECT TipoUsuarioId FROM dbo.TipoUsuario WHERE CodigoTipoUsuario = 'Colaborador'),
        (SELECT CargoId FROM dbo.Cargo WHERE NomeCargo = 'Desenvolvedor Junior'),
        (SELECT AtuacaoId FROM dbo.Atuacao WHERE NomeAtuacao = 'Frontend'));

-- Define a senha "senha" nos tres usuarios, inclusive se ja existirem.
UPDATE dbo.Usuario
SET Senha = HASHBYTES('SHA2_256', 'senha')
WHERE UsuarioId IN (
    'B0000000-0000-0000-0000-000000000001',
    'B0000000-0000-0000-0000-000000000002',
    'B0000000-0000-0000-0000-000000000003'
);

-- __________________________________Projeto______________________________________
IF NOT EXISTS (SELECT 1 FROM dbo.Projeto WHERE ProjetoId = 'C0000000-0000-0000-0000-000000000001')
    INSERT INTO dbo.Projeto (ProjetoId, NomeProjeto, Autor, LinkRepositorio, StatusProjetoId, TipoProjetoId)
    VALUES ('C0000000-0000-0000-0000-000000000001', 'Projeto Teste',
        'B0000000-0000-0000-0000-000000000002', 'https://example.com/skal4ble-projeto-teste',
        (SELECT StatusProjetoId FROM dbo.StatusProjeto WHERE NomeStatusProjeto = 'Em Andamento'),
        (SELECT TipoProjetoId FROM dbo.TipoProjeto WHERE NomeTipoProjeto = 'Projeto Teste'));

-- __________________________________Tickets______________________________________
IF NOT EXISTS (SELECT 1 FROM dbo.Ticket WHERE NomeTicket = 'Ticket teste - Autor 0' AND ProjetoId = 'C0000000-0000-0000-0000-000000000001')
    INSERT INTO dbo.Ticket (NomeTicket, UsuarioResponsavel, ProjetoId, TipoTicketId, StatusTicketId)
    VALUES ('Ticket teste - Autor 0', 'B0000000-0000-0000-0000-000000000002',
        'C0000000-0000-0000-0000-000000000001',
        (SELECT TipoTicketId FROM dbo.TipoTicket WHERE NomeTipoTicket = 'Alta Prioridade'),
        (SELECT StatusTicketId FROM dbo.StatusTicket WHERE NomeStatusTicket = 'Pendente'));

IF NOT EXISTS (SELECT 1 FROM dbo.Ticket WHERE NomeTicket = 'Ticket teste - Autor 1' AND ProjetoId = 'C0000000-0000-0000-0000-000000000001')
    INSERT INTO dbo.Ticket (NomeTicket, UsuarioResponsavel, ProjetoId, TipoTicketId, StatusTicketId)
    VALUES ('Ticket teste - Autor 1', 'B0000000-0000-0000-0000-000000000003',
        'C0000000-0000-0000-0000-000000000001',
        (SELECT TipoTicketId FROM dbo.TipoTicket WHERE NomeTipoTicket = 'Media Prioridade'),
        (SELECT StatusTicketId FROM dbo.StatusTicket WHERE NomeStatusTicket = 'Em Andamento'));

-- _______________________________TicketUsuario__________________________________
IF NOT EXISTS (
    SELECT 1 FROM dbo.TicketUsuario tu JOIN dbo.Ticket t ON t.TicketId = tu.TicketId
    WHERE t.NomeTicket = 'Ticket teste - Autor 0' AND t.ProjetoId = 'C0000000-0000-0000-0000-000000000001'
      AND tu.UsuarioId = 'B0000000-0000-0000-0000-000000000002'
)
    INSERT INTO dbo.TicketUsuario (TicketId, UsuarioId, Autor)
    SELECT TicketId, 'B0000000-0000-0000-0000-000000000002', 0 FROM dbo.Ticket
    WHERE NomeTicket = 'Ticket teste - Autor 0' AND ProjetoId = 'C0000000-0000-0000-0000-000000000001';

IF NOT EXISTS (
    SELECT 1 FROM dbo.TicketUsuario tu JOIN dbo.Ticket t ON t.TicketId = tu.TicketId
    WHERE t.NomeTicket = 'Ticket teste - Autor 1' AND t.ProjetoId = 'C0000000-0000-0000-0000-000000000001'
      AND tu.UsuarioId = 'B0000000-0000-0000-0000-000000000003'
)
    INSERT INTO dbo.TicketUsuario (TicketId, UsuarioId, Autor)
    SELECT TicketId, 'B0000000-0000-0000-0000-000000000003', 1 FROM dbo.Ticket
    WHERE NomeTicket = 'Ticket teste - Autor 1' AND ProjetoId = 'C0000000-0000-0000-0000-000000000001';
GO