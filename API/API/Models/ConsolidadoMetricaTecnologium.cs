using System;
using System.Collections.Generic;

namespace API.Models;

public partial class ConsolidadoMetricaTecnologium
{
    public int ConsolidadoId { get; set; }

    public Guid UsuarioId { get; set; }

    public int TecnologiaId { get; set; }

    public int TipoMetricaId { get; set; }

    public decimal PontuacaoAtual { get; set; }

    public int TotalEventos { get; set; }

    public DateTime UltimaAtualizacao { get; set; }

    public virtual Tecnologium Tecnologia { get; set; } = null!;

    public virtual TipoMetrica TipoMetrica { get; set; } = null!;

    public virtual Usuario Usuario { get; set; } = null!;
}
