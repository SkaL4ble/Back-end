using System;
using System.Collections.Generic;

namespace API.Models;

public partial class TipoMetrica
{
    public int TipoMetricaId { get; set; }

    public string NomeMetrica { get; set; } = null!;

    public virtual ICollection<ConsolidadoMetricaTecnologium> ConsolidadoMetricaTecnologia { get; set; } = new List<ConsolidadoMetricaTecnologium>();

    public virtual ICollection<HistoricoPontuacaoMetrica> HistoricoPontuacaoMetricas { get; set; } = new List<HistoricoPontuacaoMetrica>();
}
