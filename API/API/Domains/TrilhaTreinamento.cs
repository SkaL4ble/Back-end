using System;
using System.Collections.Generic;

namespace API.Domains;

public partial class TrilhaTreinamento
{
    public int TrilhaTreinamentoId { get; set; }

    public string NomeTrilha { get; set; } = null!;

    public string? Descricao { get; set; }

    public int NivelInicial { get; set; }

    public int NivelFinal { get; set; }

    public bool Ativa { get; set; }

    public virtual ICollection<TrilhaTreinamentoTecnologia> TrilhaTreinamentoTecnologia { get; set; } = new List<TrilhaTreinamentoTecnologia>();
}
