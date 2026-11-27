using System;
using System.Collections.Generic;

namespace API.Domains;

public partial class TrilhaTreinamentoTecnologia
{
    public int TrilhaTreinamentoId { get; set; }

    public int TecnologiaId { get; set; }

    public int Ordem { get; set; }

    public bool Obrigatoria { get; set; }

    public virtual Tecnologia Tecnologia { get; set; } = null!;

    public virtual TrilhaTreinamento TrilhaTreinamento { get; set; } = null!;
}
