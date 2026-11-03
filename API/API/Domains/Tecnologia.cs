using System;
using System.Collections.Generic;

namespace API.Domains;

public partial class Tecnologia
{
    public int TecnologiaId { get; set; }

    public string NomeTecnologia { get; set; } = null!;

    public virtual ICollection<ConsolidadoMetricaTecnologia> ConsolidadoMetricaTecnologia { get; set; } = new List<ConsolidadoMetricaTecnologia>();

    public virtual ICollection<HistoricoPontuacaoMetrica> HistoricoPontuacaoMetrica { get; set; } = new List<HistoricoPontuacaoMetrica>();

    public virtual ICollection<Usuario> Usuario { get; set; } = new List<Usuario>();
}
