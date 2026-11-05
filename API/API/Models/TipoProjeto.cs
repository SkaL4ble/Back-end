using System;
using System.Collections.Generic;

namespace API.Models;

public partial class TipoProjeto
{
    public int TipoProjetoId { get; set; }

    public string NomeTipoProjeto { get; set; } = null!;

    public virtual ICollection<Projeto> Projetos { get; set; } = new List<Projeto>();
}
