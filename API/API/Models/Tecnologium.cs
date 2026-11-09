using System;
using System.Collections.Generic;

namespace API.Models;

public partial class Tecnologium
{
    public int TecnologiaId { get; set; }

    public string NomeTecnologia { get; set; } = null!;

    public virtual ICollection<ConsolidadoMetricaTecnologium> ConsolidadoMetricaTecnologia { get; set; } = new List<ConsolidadoMetricaTecnologium>();

    public virtual ICollection<HistoricoPontuacaoMetrica> HistoricoPontuacaoMetricas { get; set; } = new List<HistoricoPontuacaoMetrica>();

    public virtual ICollection<Usuario> Usuarios { get; set; } = new List<Usuario>();
}
