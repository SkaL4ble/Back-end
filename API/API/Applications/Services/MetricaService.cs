using API.Applications.Conversoes.ConversoesDeDto;
using API.Domains;
using API.DTOs.SonarQubeDto.ListagemDto;
using API.Interfaces;

namespace API.Applications.Services
{
    public class MetricaService
    {
        private readonly IMetricaRepository _repository;

        private const decimal PenalidadeAtrasoPorDia = 2m;
        private const decimal PenalidadeInatividadePorDia = 5m;
        private const int DiasToleranciaSemAtividade = 2;

        public MetricaService(
            IMetricaRepository repository)
        {
            _repository = repository;
        }

        public async Task RegistrarTicketAtribuidoAsync(Ticket ticket, CancellationToken cancellationToken = default)
        {
            if (!ticket.UsuarioResponsavel.HasValue)
                return;

            var usuarioId = ticket.UsuarioResponsavel.Value;

            var metrica = await ObterOuCriarAtualAsync(
                usuarioId,
                ticket.DataCriacao,
                cancellationToken);

            metrica.TicketsAtribuidos++;
            metrica.DataAtualizacao = DateTime.UtcNow;

            await _repository.SalvarAsync(cancellationToken);
        }

        public async Task RegistrarTicketConcluidoAsync(
            Ticket ticket,
            CancellationToken cancellationToken = default)
        {
            if (!ticket.UsuarioResponsavel.HasValue)
                return;

            var usuarioId = ticket.UsuarioResponsavel.Value;
            var dataEntrega = ticket.DataEntrega ?? DateTime.UtcNow;

            var metrica = await ObterOuCriarAtualAsync(
                usuarioId,
                dataEntrega,
                cancellationToken);

            metrica.TicketsConcluidos++;

            var pesoPrioridade =
                ObterPesoPrioridade(ticket.Prioridade);

            metrica.PontosQuantidade += pesoPrioridade;

            switch (ticket.Prioridade)
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

            if (ticket.DataPrazo.HasValue)
            {
                metrica.TicketsComPrazo++;

                var diasAtraso = CalcularDiasAtraso(
                    ticket.DataPrazo.Value,
                    dataEntrega);

                if (diasAtraso == 0)
                {
                    metrica.TicketsConcluidosNoPrazo++;
                }
                else
                {
                    metrica.TicketsConcluidosAtrasados++;
                    metrica.DiasAtrasoAcumulados += diasAtraso;
                }
            }

            metrica.UltimaAtividade = dataEntrega;
            metrica.DataAtualizacao = DateTime.UtcNow;

            RecalcularNotas(metrica);

            await _repository.SalvarAsync(cancellationToken);
        }

        public async Task AtualizarQualidadeSonarAsync(
            Guid usuarioId,
            MetricasAutorSonarDto metricasSonar,
            CancellationToken cancellationToken = default)
        {
            var qualidade = CalcularQualidade(metricasSonar);

            var metrica = await ObterOuCriarAtualAsync(
                usuarioId,
                DateTime.UtcNow,
                cancellationToken);

            metrica.QualidadeSonar = qualidade;

            metrica.QualidadeManutencao =
                LimitarNota(metricasSonar.NivelManutencao);

            metrica.QualidadeConfiabilidade =
                LimitarNota(metricasSonar.NivelConfiabilidade);

            metrica.QualidadeSeguranca =
                LimitarNota(metricasSonar.NivelSeguranca);

            metrica.IssuesManutencao =
                metricasSonar.IssuesManutencao;

            metrica.IssuesConfiabilidade =
                metricasSonar.IssuesConfiabilidade;

            metrica.IssuesSeguranca =
                metricasSonar.IssuesSeguranca;

            metrica.DataUltimaQualidadeSonar =
                DateTime.UtcNow;

            metrica.DataAtualizacao = DateTime.UtcNow;

            await _repository.SalvarAsync(cancellationToken);
        }

        public async Task FecharTrimestreAsync(
            DateTime dataReferencia,
            CancellationToken cancellationToken = default)
        {
            var periodoInicio =
                ObterInicioTrimestre(dataReferencia);

            var periodoFim =
                periodoInicio
                    .AddMonths(3)
                    .AddDays(-1);

            var proximoPeriodoInicio =
                periodoInicio.AddMonths(3);

            var proximoPeriodoFim =
                proximoPeriodoInicio
                    .AddMonths(3)
                    .AddDays(-1);

            var metricas =
                await _repository.ListarAtuaisPorPeriodoAsync(
                    periodoInicio,
                    periodoFim,
                    cancellationToken);

            await using var transacao =
                await _repository.IniciarTransacaoAsync(
                    cancellationToken);

            try
            {
                foreach (var metrica in metricas)
                {
                    var historicoExiste =
                        await _repository.HistoricoExisteAsync(
                            metrica.UsuarioId,
                            periodoInicio,
                            periodoFim,
                            cancellationToken);

                    if (historicoExiste)
                        continue;

                    await AtualizarAtrasosDosTicketsAsync(
                        metrica,
                        periodoInicio,
                        periodoFim,
                        cancellationToken);

                    var diasSemAtividade =
                        CalcularDiasSemAtividade(
                            metrica,
                            DateTime.UtcNow);

                    var penalidadeInatividade =
                        CalcularPenalidadeInatividade(
                            diasSemAtividade);

                    metrica.NotaVelocidade =
                        LimitarNota(
                            metrica.NotaVelocidade
                            - penalidadeInatividade);

                    metrica.DiasSemAtividade =
                        diasSemAtividade;

                    var historico =
                        ConversaoMetrica.ConverterParaHistorico(metrica);

                    _repository.AdicionarHistorico(historico);

                    ResetarDadosDoTrimestre(
                        metrica,
                        proximoPeriodoInicio,
                        proximoPeriodoFim);
                }

                await _repository.SalvarAsync(
                    cancellationToken);

                await transacao.CommitAsync(
                    cancellationToken);
            }
            catch
            {
                await transacao.RollbackAsync(
                    cancellationToken);

                throw;
            }
        }

        private async Task<FuncionarioMetricaAtual>
            ObterOuCriarAtualAsync(
                Guid usuarioId,
                DateTime dataReferencia,
                CancellationToken cancellationToken)
        {
            var metrica =
                await _repository.ObterAtualAsync(
                    usuarioId,
                    cancellationToken);

            if (metrica is not null)
                return metrica;

            var periodoInicio =
                ObterInicioTrimestre(dataReferencia);

            var periodoFim =
                periodoInicio
                    .AddMonths(3)
                    .AddDays(-1);

            metrica = new FuncionarioMetricaAtual
            {
                UsuarioId = usuarioId,
                PeriodoInicio = periodoInicio,
                PeriodoFim = periodoFim,

                TicketsAtribuidos = 0,
                TicketsConcluidos = 0,
                PontosQuantidade = 0,

                NotaQuantidade = 0,
                NotaVelocidade = 100,

                QualidadeSonar = 0,
                DataAtualizacao = DateTime.UtcNow
            };

            _repository.AdicionarAtual(metrica);

            return metrica;
        }

        private async Task AtualizarAtrasosDosTicketsAsync(
            FuncionarioMetricaAtual metrica,
            DateTime periodoInicio,
            DateTime periodoFim,
            CancellationToken cancellationToken)
        {
            var tickets =
                await _repository.ListarTicketsDoPeriodoAsync(
                    metrica.UsuarioId,
                    periodoInicio,
                    periodoFim,
                    cancellationToken);

            var agora = DateTime.UtcNow;

            var ticketsAbertosAtrasados = 0;
            var diasAtraso = 0;

            foreach (var ticket in tickets)
            {
                if (!ticket.DataPrazo.HasValue)
                    continue;

                var concluido = EstaConcluido(ticket);

                if (!concluido && agora > ticket.DataPrazo.Value)
                {
                    ticketsAbertosAtrasados++;

                    diasAtraso += CalcularDiasAtraso(
                        ticket.DataPrazo.Value,
                        agora);
                }
            }

            metrica.TicketsAbertosAtrasados =
                ticketsAbertosAtrasados;

            metrica.DiasAtrasoAcumulados +=
                diasAtraso;

            RecalcularNotas(metrica);
        }

      

        private static void ResetarDadosDoTrimestre(
            FuncionarioMetricaAtual metrica,
            DateTime novoPeriodoInicio,
            DateTime novoPeriodoFim)
        {
            // Quantidade
            metrica.TicketsAtribuidos = 0;
            metrica.TicketsConcluidos = 0;

            metrica.TicketsAltaConcluidos = 0;
            metrica.TicketsMediaConcluidos = 0;
            metrica.TicketsBaixaConcluidos = 0;

            metrica.PontosQuantidade = 0;

            // Velocidade
            metrica.TicketsComPrazo = 0;
            metrica.TicketsConcluidosNoPrazo = 0;
            metrica.TicketsConcluidosAtrasados = 0;
            metrica.TicketsAbertosAtrasados = 0;

            metrica.DiasAtrasoAcumulados = 0;
            metrica.DiasSemAtividade = 0;
            metrica.UltimaAtividade = null;

            metrica.NotaQuantidade = 0;
            metrica.NotaVelocidade = 100;

            /*
             * Qualidade não é resetada.
             */
            metrica.PeriodoInicio = novoPeriodoInicio;
            metrica.PeriodoFim = novoPeriodoFim;
            metrica.DataAtualizacao = DateTime.UtcNow;
        }

        private static void RecalcularNotas(
            FuncionarioMetricaAtual metrica)
        {
            metrica.NotaQuantidade =
                CalcularNotaQuantidade(metrica);

            metrica.NotaVelocidade =
                CalcularNotaVelocidade(metrica);
        }

        private static decimal CalcularNotaQuantidade(
            FuncionarioMetricaAtual metrica)
        {
            var diasPeriodo =
                Math.Max(
                    1,
                    (
                        metrica.PeriodoFim.Date
                        - metrica.PeriodoInicio.Date
                    ).Days + 1);

            var meses =
                Math.Max(1m, diasPeriodo / 30.44m);

            var media =
                metrica.PontosQuantidade / meses;

            if (media <= 0)
                return 0;

            if (media <= 7)
            {
                return Math.Round(
                    media / 7m * 70m,
                    2,
                    MidpointRounding.AwayFromZero);
            }

            if (media >= 10)
                return 100;

            return Math.Round(
                70m + ((media - 7m) / 3m * 30m),
                2,
                MidpointRounding.AwayFromZero);
        }

        private static decimal CalcularNotaVelocidade(
            FuncionarioMetricaAtual metrica)
        {
            if (metrica.TicketsComPrazo == 0)
                return 100;

            var penalidade =
                metrica.DiasAtrasoAcumulados
                * PenalidadeAtrasoPorDia;

            return LimitarNota(100 - penalidade);
        }

        private static decimal CalcularQualidade(
            MetricasAutorSonarDto metricas)
        {
            /*
             * Complexidade não é utilizada.
             */
            return Math.Round(
                (
                    metricas.NivelManutencao
                    + metricas.NivelConfiabilidade
                    + metricas.NivelSeguranca
                ) / 3m,
                2,
                MidpointRounding.AwayFromZero);
        }

        private static decimal
            CalcularPenalidadeInatividade(
                int diasSemAtividade)
        {
            var diasPuniveis =
                Math.Max(
                    0,
                    diasSemAtividade
                    - DiasToleranciaSemAtividade);

            return diasPuniveis
                * PenalidadeInatividadePorDia;
        }

        private static int CalcularDiasSemAtividade(
            FuncionarioMetricaAtual metrica,
            DateTime agora)
        {
            /*
             * Se nunca houve atividade, não punimos
             * automaticamente o funcionário.
             */
            if (!metrica.UltimaAtividade.HasValue)
                return 0;

            var dias =
                (
                    agora.Date
                    - metrica.UltimaAtividade.Value.Date
                ).Days;

            return Math.Max(0, dias);
        }

        private static int ObterPesoPrioridade(
            int prioridade)
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

        private static int CalcularDiasAtraso(
            DateTime prazo,
            DateTime dataFinal)
        {
            if (dataFinal <= prazo)
                return 0;

            return (
                dataFinal.Date
                - prazo.Date
            ).Days;
        }

        private static bool EstaConcluido(
            Ticket ticket)
        {
            if (ticket.DataEntrega.HasValue)
                return true;

            var status =
                ticket.StatusTicket?.NomeStatusTicket;

            return status is not null &&
                (
                    status.Equals(
                        "Concluído",
                        StringComparison.OrdinalIgnoreCase)
                    || status.Equals(
                        "Concluido",
                        StringComparison.OrdinalIgnoreCase)
                    || status.Equals(
                        "Finalizado",
                        StringComparison.OrdinalIgnoreCase)
                    || status.Equals(
                        "Fechado",
                        StringComparison.OrdinalIgnoreCase)
                );
        }

        private static DateTime ObterInicioTrimestre(
            DateTime data)
        {
            var primeiroMes =
                ((data.Month - 1) / 3) * 3 + 1;

            return new DateTime(
                data.Year,
                primeiroMes,
                1,
                0,
                0,
                0,
                DateTimeKind.Utc);
        }

        private static decimal LimitarNota(
            decimal nota)
        {
            return Math.Clamp(nota, 0m, 100m);
        }

        public async Task<FuncionarioMetricaAtual>
        ObterMetricasPorFuncionarioAsync(
        Guid usuarioId,
        CancellationToken cancellationToken = default)
        {
            var metrica =
                await _repository.ObterAtualAsync(
                    usuarioId,
                    cancellationToken);

            if (metrica is null)
            {
                throw new KeyNotFoundException(
                    "Não existem métricas atuais para este funcionário.");
            }

            return metrica;
        }
    }
}
