using System;
using System.Collections.Generic;

namespace API.Models;

public partial class StatusProjeto
{
    public int StatusProjetoId { get; set; }

    public string NomeStatusProjeto { get; set; } = null!;

    public virtual ICollection<Projeto> Projetos { get; set; } = new List<Projeto>();
}
