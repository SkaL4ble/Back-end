using System;
using System.Collections.Generic;

namespace API.Domains;

public partial class HistoricoPontuacaoMetrica
{
    public long HistoricoId { get; set; }

    public Guid UsuarioId { get; set; }

    public int TecnologiaId { get; set; }

    public int TipoMetricaId { get; set; }

    public decimal IncrementoAnterior { get; set; }

    public decimal ValorIncremento { get; set; }

    public string OrigemEvento { get; set; } = null!;

    public string? Justificativa { get; set; }

    public DateTime DataRegistro { get; set; }

    public virtual Tecnologia Tecnologia { get; set; } = null!;

    public virtual TipoMetrica TipoMetrica { get; set; } = null!;

    public virtual Usuario Usuario { get; set; } = null!;
}
