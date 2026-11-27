using System;
using System.Collections.Generic;

namespace API.Domains;

public partial class Tecnologia
{
    public int TecnologiaId { get; set; }

    public string NomeTecnologia { get; set; } = null!;

    public virtual ICollection<ConsolidadoMetricaTecnologia> ConsolidadoMetricaTecnologia { get; set; } = new List<ConsolidadoMetricaTecnologia>();

    public virtual ICollection<HistoricoPontuacaoMetrica> HistoricoPontuacaoMetrica { get; set; } = new List<HistoricoPontuacaoMetrica>();

    public virtual ICollection<ProjetoTecnologia> ProjetoTecnologia { get; set; } = new List<ProjetoTecnologia>();

    public virtual ICollection<TecnologiaUsuario> TecnologiaUsuario { get; set; } = new List<TecnologiaUsuario>();

    public virtual ICollection<TrilhaTreinamentoTecnologia> TrilhaTreinamentoTecnologia { get; set; } = new List<TrilhaTreinamentoTecnologia>();

    public virtual ICollection<UsuarioProjetoTecnologia> UsuarioProjetoTecnologia { get; set; } = new List<UsuarioProjetoTecnologia>();
}
