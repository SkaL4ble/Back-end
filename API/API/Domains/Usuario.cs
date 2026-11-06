using System;
using System.Collections.Generic;

namespace API.Domains;

public partial class Usuario
{
    public Guid UsuarioId { get; set; }

    public string NomeUsuario { get; set; } = null!;

    public string? EmailPessoal { get; set; }

    public byte[] Senha { get; set; } = null!;

    public byte[]? Imagem { get; set; }

    public bool Ativo { get; set; }

    public Guid TipoUsuarioId { get; set; }

    public int CargoId { get; set; }

    public int AtuacaoId { get; set; }

    public virtual Atuacao Atuacao { get; set; } = null!;

    public virtual Cargo Cargo { get; set; } = null!;

    public virtual ICollection<ConsolidadoMetricaTecnologia> ConsolidadoMetricaTecnologia { get; set; } = new List<ConsolidadoMetricaTecnologia>();

    public virtual ICollection<HistoricoPontuacaoMetrica> HistoricoPontuacaoMetrica { get; set; } = new List<HistoricoPontuacaoMetrica>();

    public virtual ICollection<Projeto> Projeto { get; set; } = new List<Projeto>();

    public virtual ICollection<Ticket> Ticket { get; set; } = new List<Ticket>();

    public virtual ICollection<TicketUsuario> TicketUsuario { get; set; } = new List<TicketUsuario>();

    public virtual TipoUsuario TipoUsuario { get; set; } = null!;

    public virtual ICollection<UsuarioEmailGit> UsuarioEmailGit { get; set; } = new List<UsuarioEmailGit>();

    public virtual ICollection<UsuarioIdeToken> UsuarioIdeToken { get; set; } = new List<UsuarioIdeToken>();

    public virtual ICollection<Projeto> ProjetoNavigation { get; set; } = new List<Projeto>();

    public virtual ICollection<Tecnologia> Tecnologia { get; set; } = new List<Tecnologia>();
}
