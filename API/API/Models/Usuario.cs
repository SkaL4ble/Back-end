using System;
using System.Collections.Generic;

namespace API.Models;

public partial class Usuario
{
    public Guid UsuarioId { get; set; }

    public string NomeUsuario { get; set; } = null!;

    public string? EmailPessoal { get; set; }

    public byte[] Senha { get; set; } = null!;

    public string EmailGitHub { get; set; } = null!;

    public byte[]? Imagem { get; set; }

    public bool Ativo { get; set; }

    public Guid TipoUsuarioId { get; set; }

    public int CargoId { get; set; }

    public int AtuacaoId { get; set; }

    public virtual Atuacao Atuacao { get; set; } = null!;

    public virtual Cargo Cargo { get; set; } = null!;

    public virtual ICollection<ConsolidadoMetricaTecnologium> ConsolidadoMetricaTecnologia { get; set; } = new List<ConsolidadoMetricaTecnologium>();

    public virtual ICollection<HistoricoPontuacaoMetrica> HistoricoPontuacaoMetricas { get; set; } = new List<HistoricoPontuacaoMetrica>();

    public virtual ICollection<Projeto> Projetos { get; set; } = new List<Projeto>();

    public virtual ICollection<Ticket> Tickets { get; set; } = new List<Ticket>();

    public virtual TipoUsuario TipoUsuario { get; set; } = null!;

    public virtual ICollection<UsuarioEmailGit> UsuarioEmailGits { get; set; } = new List<UsuarioEmailGit>();

    public virtual ICollection<UsuarioIdeToken> UsuarioIdeTokens { get; set; } = new List<UsuarioIdeToken>();

    public virtual ICollection<Projeto> ProjetosNavigation { get; set; } = new List<Projeto>();

    public virtual ICollection<Tecnologium> Tecnologia { get; set; } = new List<Tecnologium>();

    public virtual ICollection<Ticket> Tickets1 { get; set; } = new List<Ticket>();

    public virtual ICollection<Ticket> TicketsNavigation { get; set; } = new List<Ticket>();
}
