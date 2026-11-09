USE master;
GO

IF EXISTS (SELECT name FROM sys.databases WHERE name = N'SkaL4ble')
BEGIN
    ALTER DATABASE SkaL4ble SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
    DROP DATABASE SkaL4ble;
END
GO

CREATE DATABASE SkaL4ble;
GO

USE SkaL4ble;
GO

-- =============================================================================
-- TABELAS DE DOMINIO
-- =============================================================================

CREATE TABLE TipoUsuario (
    TipoUsuarioId UNIQUEIDENTIFIER NOT NULL DEFAULT NEWID(),
    NomeTipoUsuario VARCHAR(30) NOT NULL,
    CONSTRAINT PK_TipoUsuario PRIMARY KEY (TipoUsuarioId),
    CONSTRAINT UQ_TipoUsuario_Nome UNIQUE (NomeTipoUsuario)
);
GO

CREATE TABLE Cargo (
    CargoId INT IDENTITY(1,1) NOT NULL,
    NomeCargo VARCHAR(30) NOT NULL,
    CONSTRAINT PK_Cargo PRIMARY KEY (CargoId),
    CONSTRAINT UQ_Cargo_Nome UNIQUE (NomeCargo)
);
GO

CREATE TABLE Atuacao (
    AtuacaoId INT IDENTITY(1,1) NOT NULL,
    NomeAtuacao VARCHAR(30) NOT NULL,
    CONSTRAINT PK_Atuacao PRIMARY KEY (AtuacaoId),
    CONSTRAINT UQ_Atuacao_Nome UNIQUE (NomeAtuacao)
);
GO

CREATE TABLE Tecnologia (
    TecnologiaId INT IDENTITY(1,1) NOT NULL,
    NomeTecnologia VARCHAR(50) NOT NULL,
    CONSTRAINT PK_Tecnologia PRIMARY KEY (TecnologiaId),
    CONSTRAINT UQ_Tecnologia_Nome UNIQUE (NomeTecnologia)
);
GO

CREATE TABLE TipoProjeto (
    TipoProjetoId INT IDENTITY(1,1) NOT NULL,
    NomeTipoProjeto VARCHAR(30) NOT NULL,
    CONSTRAINT PK_TipoProjeto PRIMARY KEY (TipoProjetoId),
    CONSTRAINT UQ_TipoProjeto_Nome UNIQUE (NomeTipoProjeto)
);
GO

CREATE TABLE StatusProjeto (
    StatusProjetoId INT IDENTITY(1,1) NOT NULL,
    NomeStatusProjeto VARCHAR(30) NOT NULL,
    CONSTRAINT PK_StatusProjeto PRIMARY KEY (StatusProjetoId),
    CONSTRAINT UQ_StatusProjeto_Nome UNIQUE (NomeStatusProjeto)
);
GO

CREATE TABLE TipoTicket (
    TipoTicketId INT IDENTITY(1,1) NOT NULL,
    NomeTipoTicket VARCHAR(30) NOT NULL,
    CONSTRAINT PK_TipoTicket PRIMARY KEY (TipoTicketId),
    CONSTRAINT UQ_TipoTicket_Nome UNIQUE (NomeTipoTicket)
);
GO

CREATE TABLE StatusTicket (
    StatusTicketId INT IDENTITY(1,1) NOT NULL,
    NomeStatusTicket VARCHAR(30) NOT NULL,
    CONSTRAINT PK_StatusTicket PRIMARY KEY (StatusTicketId),
    CONSTRAINT UQ_StatusTicket_Nome UNIQUE (NomeStatusTicket)
);
GO

CREATE TABLE TipoMetrica (
    TipoMetricaId INT IDENTITY(1,1) NOT NULL,
    NomeMetrica VARCHAR(50) NOT NULL,
    CONSTRAINT PK_TipoMetrica PRIMARY KEY (TipoMetricaId),
    CONSTRAINT UQ_TipoMetrica_Nome UNIQUE (NomeMetrica)
);
GO

-- =============================================================================
-- TABELAS PRINCIPAIS
-- =============================================================================

CREATE TABLE Usuario (
    UsuarioId UNIQUEIDENTIFIER NOT NULL DEFAULT NEWID(),
    NomeUsuario VARCHAR(50) NOT NULL,
    EmailPessoal VARCHAR(100) NULL,
    Senha VARBINARY(MAX) NOT NULL,
    EmailGitHub VARCHAR(100) NOT NULL,
    Imagem VARBINARY(MAX) NULL,
    Ativo BIT NOT NULL DEFAULT 1,
    TipoUsuarioId UNIQUEIDENTIFIER NOT NULL,
    CargoId INT NOT NULL,
    AtuacaoId INT NOT NULL,
    CONSTRAINT PK_Usuario PRIMARY KEY (UsuarioId),

    CONSTRAINT UQ_Usuario_EmailPessoal UNIQUE (EmailPessoal),
    CONSTRAINT UQ_Usuario_EmailGitHub UNIQUE (EmailGitHub),

    CONSTRAINT FK_Usuario_TipoUsuario FOREIGN KEY (TipoUsuarioId) 
        REFERENCES TipoUsuario (TipoUsuarioId),
    CONSTRAINT FK_Usuario_Cargo FOREIGN KEY (CargoId)
        REFERENCES Cargo (CargoId),
    CONSTRAINT FK_Usuario_Atuacao FOREIGN KEY (AtuacaoId)
        REFERENCES Atuacao (AtuacaoId)
);
GO

CREATE TABLE Projeto (
    ProjetoId UNIQUEIDENTIFIER NOT NULL DEFAULT NEWID(),
    NomeProjeto VARCHAR(50) NOT NULL,
    Prazo DATETIME2 NULL,
    Autor UNIQUEIDENTIFIER NOT NULL,
    Descricao VARCHAR(MAX) NULL,
    LinkRepositorio VARCHAR(150) NOT NULL,
    Ativo BIT NOT NULL DEFAULT 1,
    StatusProjetoId INT NOT NULL,
    TipoProjetoId INT NOT NULL,
    CONSTRAINT PK_Projeto PRIMARY KEY (ProjetoId),

    CONSTRAINT UQ_Projeto_LinkRepositorio UNIQUE (LinkRepositorio),

    CONSTRAINT FK_Projeto_StatusProjeto FOREIGN KEY (StatusProjetoId) 
        REFERENCES StatusProjeto (StatusProjetoId),
    CONSTRAINT FK_Projeto_Autor FOREIGN KEY (Autor) 
        REFERENCES Usuario (UsuarioId),
    CONSTRAINT FK_Projeto_TipoProjeto FOREIGN KEY (TipoProjetoId) 
        REFERENCES TipoProjeto (TipoProjetoId)
);
GO

CREATE TABLE Ticket (
    TicketId INT IDENTITY(1,1) NOT NULL,
    NomeTicket VARCHAR(125) NOT NULL,
    Prioridade INT NOT NULL DEFAULT 1,
    DataCriacao DATETIME2 NOT NULL DEFAULT GETDATE(),
    DataPrazo DATETIME2 NULL,
    DataEntrega DATETIME2 NULL,
    Ativo BIT NOT NULL DEFAULT 1,
    UsuarioResponsavel UNIQUEIDENTIFIER NULL,
    ProjetoId UNIQUEIDENTIFIER NOT NULL,
    TipoTicketId INT NOT NULL,
    StatusTicketId INT NOT NULL,
    CONSTRAINT PK_Ticket PRIMARY KEY (TicketId),

    CONSTRAINT FK_Ticket_UsuarioResponsavel FOREIGN KEY (UsuarioResponsavel) 
        REFERENCES Usuario (UsuarioId),
    CONSTRAINT FK_Ticket_Projeto FOREIGN KEY (ProjetoId) 
        REFERENCES Projeto (ProjetoId),
    CONSTRAINT FK_Ticket_TipoTicket FOREIGN KEY (TipoTicketId) 
        REFERENCES TipoTicket (TipoTicketId),
    CONSTRAINT FK_Ticket_StatusTicket FOREIGN KEY (StatusTicketId)
        REFERENCES StatusTicket (StatusTicketId)
);
GO

-- REDUNDÂNCIA DE GIT
CREATE TABLE UsuarioEmailGit (
    UsuarioEmailGitId INT IDENTITY(1,1) NOT NULL,
    UsuarioId UNIQUEIDENTIFIER NOT NULL,
    EmailGit VARCHAR(100) NOT NULL,
    Ativo BIT NOT NULL DEFAULT 1,
    CONSTRAINT PK_UsuarioEmailGit PRIMARY KEY (UsuarioEmailGitId),

    CONSTRAINT UQ_UsuarioEmailGit_Email UNIQUE (EmailGit),

    CONSTRAINT FK_UsuarioEmailGit_Usuario FOREIGN KEY (UsuarioId) 
        REFERENCES Usuario (UsuarioId) ON DELETE CASCADE
);
GO

-- REDUNDÂNCIA DE IDE
CREATE TABLE UsuarioIdeToken (
    TokenId UNIQUEIDENTIFIER NOT NULL DEFAULT NEWID(),
    UsuarioId UNIQUEIDENTIFIER NOT NULL,
    NomeDispositivo VARCHAR(100) NOT NULL,
    ApiKeyHash VARCHAR(255) NOT NULL,
    UltimoUso DATETIME2 NULL,
    Ativo BIT NOT NULL DEFAULT 1,
    CONSTRAINT PK_UsuarioIdeToken PRIMARY KEY (TokenId),
    CONSTRAINT FK_UsuarioIdeToken_Usuario FOREIGN KEY (UsuarioId) 
        REFERENCES Usuario (UsuarioId) ON DELETE CASCADE
);
GO

-- HISTÓRICO DE INCREMENTOS DAS MÉTRICAS
CREATE TABLE HistoricoPontuacaoMetrica (
    HistoricoId BIGINT IDENTITY(1,1) NOT NULL,
    UsuarioId UNIQUEIDENTIFIER NOT NULL,
    TecnologiaId INT NOT NULL,
    TipoMetricaId INT NOT NULL,
    IncrementoAnterior DECIMAL(5,2) NOT NULL,
    ValorIncremento DECIMAL(5,2) NOT NULL,
    OrigemEvento VARCHAR(50) NOT NULL,
    Justificativa VARCHAR(MAX) NULL,
    DataRegistro DATETIME2 NOT NULL DEFAULT GETDATE(),
    CONSTRAINT PK_HistoricoPontuacaoMetrica PRIMARY KEY (HistoricoId),
    CONSTRAINT FK_HistPont_Usuario FOREIGN KEY (UsuarioId) 
        REFERENCES Usuario (UsuarioId) ON DELETE CASCADE,
    CONSTRAINT FK_HistPont_Tecnologia FOREIGN KEY (TecnologiaId) 
        REFERENCES Tecnologia (TecnologiaId),
    CONSTRAINT FK_HistPont_TipoMetrica FOREIGN KEY (TipoMetricaId) 
        REFERENCES TipoMetrica (TipoMetricaId)
);
GO

-- MÉDIA DAS MÉTRICAS
CREATE TABLE ConsolidadoMetricaTecnologia (
    ConsolidadoId INT IDENTITY(1,1) NOT NULL,
    UsuarioId UNIQUEIDENTIFIER NOT NULL,
    TecnologiaId INT NOT NULL,
    TipoMetricaId INT NOT NULL,
    PontuacaoAtual DECIMAL(5,2) NOT NULL DEFAULT 0.00,
    TotalEventos INT NOT NULL DEFAULT 0,
    UltimaAtualizacao DATETIME2 NOT NULL DEFAULT GETDATE(),
    CONSTRAINT PK_ConsolidadoMetricaTecnologia PRIMARY KEY (ConsolidadoId),

    CONSTRAINT FK_ConsTec_Usuario FOREIGN KEY (UsuarioId) 
        REFERENCES Usuario (UsuarioId) ON DELETE CASCADE,
    CONSTRAINT FK_ConsTec_Tecnologia FOREIGN KEY (TecnologiaId) 
        REFERENCES Tecnologia (TecnologiaId) ON DELETE CASCADE,
    CONSTRAINT FK_ConsTec_TipoMetrica FOREIGN KEY (TipoMetricaId) 
        REFERENCES TipoMetrica (TipoMetricaId),

    CONSTRAINT UQ_Usuario_Tecnologia_Metrica UNIQUE (UsuarioId, TecnologiaId, TipoMetricaId)
);
GO

-- =============================================================================
-- TABELAS INTERMEDIARIAS
-- =============================================================================

CREATE TABLE TecnologiaUsuario (
    TecnologiaId INT NOT NULL,
    UsuarioId UNIQUEIDENTIFIER NOT NULL,
    CONSTRAINT PK_TecnologiaUsuario PRIMARY KEY (UsuarioId, TecnologiaId),

    CONSTRAINT FK_TecnologiaUsuario_Tecnologia FOREIGN KEY (TecnologiaId) 
        REFERENCES Tecnologia (TecnologiaId) ON DELETE CASCADE,
    CONSTRAINT FK_TecnologiaUsuario_Usuario FOREIGN KEY (UsuarioId) 
        REFERENCES Usuario (UsuarioId) ON DELETE CASCADE
);
GO

CREATE TABLE ProjetoUsuario (
    ProjetoId UNIQUEIDENTIFIER NOT NULL,
    UsuarioId UNIQUEIDENTIFIER NOT NULL,
    CONSTRAINT PK_ProjetoUsuario PRIMARY KEY (ProjetoId, UsuarioId),

    CONSTRAINT FK_ProjetoUsuario_Projeto FOREIGN KEY (ProjetoId) 
        REFERENCES Projeto (ProjetoId) ON DELETE CASCADE,
    CONSTRAINT FK_ProjetoUsuario_Usuario FOREIGN KEY (UsuarioId) 
        REFERENCES Usuario (UsuarioId) ON DELETE CASCADE
);
GO

CREATE TABLE TicketUsuario (
    TicketId INT NOT NULL,
    UsuarioId UNIQUEIDENTIFIER NOT NULL,
    CONSTRAINT PK_TicketColaboradores PRIMARY KEY (TicketId, UsuarioId),

    CONSTRAINT FK_TicketUsuario_Ticket FOREIGN KEY (TicketId) 
        REFERENCES Ticket (TicketId) ON DELETE CASCADE,
    CONSTRAINT FK_TicketUsuario_Usuario FOREIGN KEY (UsuarioId) 
        REFERENCES Usuario (UsuarioId) ON DELETE CASCADE
);
GO

CREATE TABLE TicketColaborador(
    TicketId INT IDENTITY,
    ColaboradorId UNIQUEIDENTIFIER,

        CONSTRAINT PK_TicketColaborador PRIMARY KEY (TicketId, ColaboradorId),

    CONSTRAINT FK_TickeColaborador_Tecnologia FOREIGN KEY (ColaboradorId) 
        REFERENCES Usuario (UsuarioId),
    CONSTRAINT FK_TickeColaborador_Colaborador FOREIGN KEY (TicketId) 
        REFERENCES Ticket (TicketId) 
);
GO

-- =============================================================================
-- TRIGGERS
-- =============================================================================

-- INATIVA O PROJETO E SEUS TICKETS CORRESPONDENTES
CREATE TRIGGER TR_Projeto_SoftDeleteCascade 
ON Projeto
INSTEAD OF DELETE
AS
BEGIN
    SET NOCOUNT ON;

    -- Inativa o projeto
    UPDATE t
    SET t.Ativo = 0
    FROM Ticket t
    INNER JOIN deleted d ON t.ProjetoId = d.ProjetoId;

    -- Inativa os tickets
    UPDATE p
    SET p.Ativo = 0
    FROM Projeto p
    INNER JOIN deleted d ON p.ProjetoId = d.ProjetoId;
END;
GO

-- INATIVA O USUARIO E DESATRELA SEUS TICKETS CORRESPONDENTES
CREATE OR ALTER TRIGGER TR_Usuario_SoftDeleteCascade 
ON Usuario
INSTEAD OF DELETE
AS
BEGIN
    SET NOCOUNT ON;

    -- Desatrela os tickets
    UPDATE t
    SET t.UsuarioResponsavel = NULL
    FROM Ticket t
    INNER JOIN deleted d ON t.UsuarioResponsavel = d.UsuarioId
    WHERE t.DataEntrega IS NULL;

    -- Inativa o usuario
    UPDATE u
    SET u.Ativo = 0
    FROM Usuario u
    INNER JOIN deleted d ON u.UsuarioId = d.UsuarioId;
END;
GO

SELECT * FROM Cargo