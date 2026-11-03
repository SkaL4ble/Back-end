using System;
using System.Collections.Generic;

namespace API.Domains;

public partial class Projeto
{
    public Guid ProjetoId { get; set; }

    public string NomeProjeto { get; set; } = null!;

    public DateTime? Prazo { get; set; }

    public Guid Autor { get; set; }

    public string? Descricao { get; set; }

    public string LinkRepositorio { get; set; } = null!;

    public bool Ativo { get; set; }

    public int StatusProjetoId { get; set; }

    public int TipoProjetoId { get; set; }

    public virtual Usuario AutorNavigation { get; set; } = null!;

    public virtual StatusProjeto StatusProjeto { get; set; } = null!;

    public virtual ICollection<Ticket> Ticket { get; set; } = new List<Ticket>();

    public virtual TipoProjeto TipoProjeto { get; set; } = null!;

    public virtual ICollection<Usuario> Usuario { get; set; } = new List<Usuario>();
}
