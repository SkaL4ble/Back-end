using System;
using System.Collections.Generic;

namespace API.Domains;

public partial class ConsolidadoMetricaTecnologia
{
    public int ConsolidadoId { get; set; }

    public Guid UsuarioId { get; set; }

    public int TecnologiaId { get; set; }

    public int TipoMetricaId { get; set; }

    public decimal PontuacaoAtual { get; set; }

    public int TotalEventos { get; set; }

    public DateTime UltimaAtualizacao { get; set; }

    public virtual Tecnologia Tecnologia { get; set; } = null!;

    public virtual TipoMetrica TipoMetrica { get; set; } = null!;

    public virtual Usuario Usuario { get; set; } = null!;
}
