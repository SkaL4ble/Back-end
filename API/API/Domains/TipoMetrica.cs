using System;
using System.Collections.Generic;

namespace API.Domains;

public partial class TipoMetrica
{
    public int TipoMetricaId { get; set; }

    public string NomeMetrica { get; set; } = null!;

    public virtual ICollection<ConsolidadoMetricaTecnologia> ConsolidadoMetricaTecnologia { get; set; } = new List<ConsolidadoMetricaTecnologia>();

    public virtual ICollection<HistoricoPontuacaoMetrica> HistoricoPontuacaoMetrica { get; set; } = new List<HistoricoPontuacaoMetrica>();
}
