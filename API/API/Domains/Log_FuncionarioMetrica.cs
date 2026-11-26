using System;
using System.Collections.Generic;

namespace API.Domains;

public partial class Log_FuncionarioMetrica
{
    public long Log_FuncionarioMetricaId { get; set; }

    public Guid UsuarioId { get; set; }

    public DateOnly PeriodoInicio { get; set; }

    public DateOnly PeriodoFim { get; set; }

    public decimal NotaQuantidade { get; set; }

    public decimal NotaVelocidade { get; set; }

    public decimal NotaQualidade { get; set; }

    public int TicketsAtribuidos { get; set; }

    public int TicketsConcluidos { get; set; }

    public int TicketsAltaConcluidos { get; set; }

    public int TicketsMediaConcluidos { get; set; }

    public int TicketsBaixaConcluidos { get; set; }

    public decimal PontosQuantidade { get; set; }

    public int TicketsComPrazo { get; set; }

    public int TicketsConcluidosNoPrazo { get; set; }

    public int TicketsConcluidosAtrasados { get; set; }

    public int TicketsAbertosAtrasados { get; set; }

    public int DiasAtrasoAcumulados { get; set; }

    public int DiasSemAtividade { get; set; }

    public decimal QualidadeSonar { get; set; }

    public decimal QualidadeManutencao { get; set; }

    public decimal QualidadeConfiabilidade { get; set; }

    public decimal QualidadeSeguranca { get; set; }

    public int IssuesManutencao { get; set; }

    public int IssuesConfiabilidade { get; set; }

    public int IssuesSeguranca { get; set; }

    public DateTime? DataQualidadeSonar { get; set; }

    public DateTime DataFechamento { get; set; }

    public virtual Usuario Usuario { get; set; } = null!;
}
