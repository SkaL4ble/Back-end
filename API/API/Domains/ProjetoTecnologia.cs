using System;
using System.Collections.Generic;

namespace API.Domains;

public partial class ProjetoTecnologia
{
    public Guid ProjetoId { get; set; }

    public int TecnologiaId { get; set; }

    public decimal PercentualCodigo { get; set; }

    public int NumeroAtribuicoes { get; set; }

    public long ValorAtribuicoes { get; set; }

    public virtual Projeto Projeto { get; set; } = null!;

    public virtual Tecnologia Tecnologia { get; set; } = null!;
}
