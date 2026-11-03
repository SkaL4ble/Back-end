using System;
using System.Collections.Generic;

namespace API.Domains;

public partial class TipoProjeto
{
    public int TipoProjetoId { get; set; }

    public string NomeTipoProjeto { get; set; } = null!;

    public virtual ICollection<Projeto> Projeto { get; set; } = new List<Projeto>();
}
