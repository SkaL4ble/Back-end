using System;
using System.Collections.Generic;

namespace API.Domains;

public partial class StatusProjeto
{
    public int StatusProjetoId { get; set; }

    public string NomeStatusProjeto { get; set; } = null!;

    public virtual ICollection<Projeto> Projeto { get; set; } = new List<Projeto>();
}
