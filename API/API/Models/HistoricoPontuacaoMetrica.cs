using System;
using System.Collections.Generic;

namespace API.Models;

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

    public virtual Tecnologium Tecnologia { get; set; } = null!;

    public virtual TipoMetrica TipoMetrica { get; set; } = null!;

    public virtual Usuario Usuario { get; set; } = null!;
}
