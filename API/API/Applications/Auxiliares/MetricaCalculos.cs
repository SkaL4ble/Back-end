using API.Domains;
using API.DTOs.SonarQubeDto.ListagemDto;

namespace API.Applications.Services.Metrica;

public static class MetricaCalculos
{
    private const decimal PenalidadeAtrasoPorDia = 2m;
    private const decimal PenalidadeInatividadePorDia = 5m;
    private const int DiasToleranciaSemAtividade = 2;

    public static int ObterPesoPrioridade(int prioridade)
    {
        return prioridade switch
        {
            1 => 3,
            2 => 2,
            3 => 1,
            _ => throw new ArgumentOutOfRangeException(
                nameof(prioridade),
                prioridade,
                "A prioridade deve ser 1, 2 ou 3.")
        };
    }

    public static void RegistrarPrioridade(
        FuncionarioMetrica metrica,
        int prioridade)
    {
        switch (prioridade)
        {
            case 1:
                metrica.TicketsAltaConcluidos++;
                break;
            case 2:
                metrica.TicketsMediaConcluidos++;
                break;
            case 3:
                metrica.TicketsBaixaConcluidos++;
                break;
            default:
                throw new InvalidOperationException(
                    "A prioridade deve ser 1, 2 ou 3.");
        }
    }

    public static void RegistrarResultadoComPrazo(
        FuncionarioMetrica metrica,
        DateTime prazo,
        DateTime dataEntrega)
    {
        metrica.TicketsComPrazo++;
        int diasAtraso = CalcularDiasAtraso(prazo, dataEntrega);

        if (diasAtraso == 0)
        {
            metrica.TicketsConcluidosNoPrazo++;
            return;
        }

        metrica.TicketsConcluidosAtrasados++;
        metrica.DiasAtrasoAcumulados += diasAtraso;
    }

    public static void RecalcularNotas(FuncionarioMetrica metrica)
    {
        metrica.NotaQuantidade = CalcularNotaQuantidade(metrica);
        metrica.NotaVelocidade = CalcularNotaVelocidade(metrica);
    }

    public static decimal CalcularQualidade(MetricasAutorSonarDto metricas)
    {
        return Math.Round(
            (metricas.NivelManutencao
             + metricas.NivelConfiabilidade
             + metricas.NivelSeguranca) / 3m,
            2,
            MidpointRounding.AwayFromZero);
    }

    public static decimal CalcularPenalidadeInatividade(int diasSemAtividade)
    {
        int diasPuniveis = Math.Max(
            0,
            diasSemAtividade - DiasToleranciaSemAtividade);

        return diasPuniveis * PenalidadeInatividadePorDia;
    }

    public static int CalcularDiasSemAtividade(
        FuncionarioMetrica metrica,
        DateTime agora)
    {
        if (!metrica.UltimaAtividade.HasValue)
        {
            return 0;
        }
        return Math.Max(
            0,
            (agora.Date - metrica.UltimaAtividade.Value.Date).Days);
    }

    public static int CalcularDiasAtraso(DateTime prazo, DateTime dataFinal)
    {
        if (dataFinal <= prazo)
        {
            return 0;
        }
        return (dataFinal.Date - prazo.Date).Days;
    }

    public static bool EstaConcluido(Ticket ticket)
    {
        if (ticket.DataEntrega.HasValue)
        {
            return true;
        }

        string? status = ticket.StatusTicket?.NomeStatusTicket;
        return status is not null &&
            (status.Equals("Concluído", StringComparison.OrdinalIgnoreCase)
             || status.Equals("Concluido", StringComparison.OrdinalIgnoreCase)
             || status.Equals("Finalizado", StringComparison.OrdinalIgnoreCase)
             || status.Equals("Fechado", StringComparison.OrdinalIgnoreCase));
    }

    public static decimal LimitarNota(decimal nota) =>
        Math.Clamp(nota, 0m, 100m);

    public static FuncionarioMetrica CriarAtual(
        Guid usuarioId,
        PeriodoMetrica periodo)
    {
        return new FuncionarioMetrica
        {
            UsuarioId = usuarioId,
            PeriodoInicio = periodo.Inicio,
            PeriodoFim = periodo.Fim,
            NotaVelocidade = 100,
            DataAtualizacao = DateTime.UtcNow
        };
    }

    public static void ResetarParaNovoPeriodo(
        FuncionarioMetrica metrica,
        PeriodoMetrica proximoPeriodo)
    {
        metrica.TicketsAtribuidos = 0;
        metrica.TicketsConcluidos = 0;
        metrica.TicketsAltaConcluidos = 0;
        metrica.TicketsMediaConcluidos = 0;
        metrica.TicketsBaixaConcluidos = 0;
        metrica.PontosQuantidade = 0;
        metrica.TicketsComPrazo = 0;
        metrica.TicketsConcluidosNoPrazo = 0;
        metrica.TicketsConcluidosAtrasados = 0;
        metrica.TicketsAbertosAtrasados = 0;
        metrica.DiasAtrasoAcumulados = 0;
        metrica.DiasSemAtividade = 0;
        metrica.UltimaAtividade = null;
        metrica.NotaQuantidade = 0;
        metrica.NotaVelocidade = 100;
        metrica.PeriodoInicio = proximoPeriodo.Inicio;
        metrica.PeriodoFim = proximoPeriodo.Fim;
        metrica.DataAtualizacao = DateTime.UtcNow;
    }

    private static decimal CalcularNotaQuantidade(
        FuncionarioMetrica metrica)
    {
        int diasPeriodo = Math.Max(
            1,
            (metrica.PeriodoFim.Date - metrica.PeriodoInicio.Date).Days + 1);
        decimal meses = Math.Max(1m, diasPeriodo / 30.44m);
        decimal media = metrica.PontosQuantidade / meses;

        if (media <= 0)
        {
            return 0;
        }
        if (media <= 7)
        {
            return Math.Round(
                media / 7m * 70m,
                2,
                MidpointRounding.AwayFromZero);
        }
        if (media >= 10)
        {
            return 100;
        }

        return Math.Round(
            70m + ((media - 7m) / 3m * 30m),
            2,
            MidpointRounding.AwayFromZero);
    }

    private static decimal CalcularNotaVelocidade(
        FuncionarioMetrica metrica)
    {
        if (metrica.TicketsComPrazo == 0)
        {
            return 100;
        }

        return LimitarNota(
            100 - metrica.DiasAtrasoAcumulados * PenalidadeAtrasoPorDia);
    }
}
