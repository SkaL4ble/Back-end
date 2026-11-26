using System;
using System.Collections.Generic;

namespace API.Domains;

public partial class Usuario
{
    public Guid UsuarioId { get; set; }

    public string NomeUsuario { get; set; } = null!;

    public string EmailPessoal { get; set; } = null!;

    public byte[]? Senha { get; set; }

    public DateTime? DataAdmissao { get; set; }

    public byte[]? Imagem { get; set; }

    public bool Ativo { get; set; }

    public Guid TipoUsuarioId { get; set; }

    public int CargoId { get; set; }

    public int AtuacaoId { get; set; }

    public virtual Atuacao Atuacao { get; set; } = null!;

    public virtual Cargo Cargo { get; set; } = null!;

    public virtual ICollection<ConsolidadoMetricaTecnologia> ConsolidadoMetricaTecnologia { get; set; } = new List<ConsolidadoMetricaTecnologia>();

    public virtual FuncionarioGamificacao? FuncionarioGamificacao { get; set; }

    public virtual FuncionarioMetrica? FuncionarioMetrica { get; set; }

    public virtual FuncionarioMetricaAtual? FuncionarioMetricaAtual { get; set; }

    public virtual ICollection<FuncionarioMetricaHistorico> FuncionarioMetricaHistorico { get; set; } = new List<FuncionarioMetricaHistorico>();

    public virtual ICollection<HistoricoPontuacaoMetrica> HistoricoPontuacaoMetrica { get; set; } = new List<HistoricoPontuacaoMetrica>();

    public virtual ICollection<Log_FuncionarioMetrica> Log_FuncionarioMetrica { get; set; } = new List<Log_FuncionarioMetrica>();

    public virtual ICollection<Projeto> Projeto { get; set; } = new List<Projeto>();

    public virtual ICollection<TecnologiaUsuario> TecnologiaUsuario { get; set; } = new List<TecnologiaUsuario>();

    public virtual ICollection<Ticket> Ticket { get; set; } = new List<Ticket>();

    public virtual ICollection<TicketUsuario> TicketUsuario { get; set; } = new List<TicketUsuario>();

    public virtual TipoUsuario TipoUsuario { get; set; } = null!;

    public virtual ICollection<TokenRedefinicaoSenha> TokenRedefinicaoSenha { get; set; } = new List<TokenRedefinicaoSenha>();

    public virtual ICollection<UsuarioEmailGit> UsuarioEmailGit { get; set; } = new List<UsuarioEmailGit>();

    public virtual ICollection<UsuarioIdeToken> UsuarioIdeToken { get; set; } = new List<UsuarioIdeToken>();

    public virtual ICollection<UsuarioProjetoTecnologia> UsuarioProjetoTecnologia { get; set; } = new List<UsuarioProjetoTecnologia>();

    public virtual ICollection<Projeto> ProjetoNavigation { get; set; } = new List<Projeto>();
}
