using API.Applications.Conversoes.ConversoesDeDto;
using API.Applications.Services.Metrica;
using API.Domains;
using API.DTOs.SonarQubeDto.ListagemDto;
using API.Interfaces;

namespace API.Applications.Services;

public class MetricaService
{
    private readonly IMetricaRepository _repository;

    public MetricaService(IMetricaRepository repository)
    {
        _repository = repository;
    }

    public async Task RegistrarTicketAtribuidoAsync(
        Ticket ticket,
        CancellationToken cancellationToken = default)
    {
        if (!ticket.UsuarioResponsavel.HasValue)
        {
            return;
        }

        FuncionarioMetrica metrica = await ObterOuCriarAtualAsync(
            ticket.UsuarioResponsavel.Value,
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
        {
            return;
        }

        DateTime dataEntrega = ticket.DataEntrega ?? DateTime.UtcNow;
        FuncionarioMetrica metrica = await ObterOuCriarAtualAsync(
            ticket.UsuarioResponsavel.Value,
            dataEntrega,
            cancellationToken);

        int pesoPrioridade =
            MetricaCalculos.ObterPesoPrioridade(ticket.Prioridade);

        metrica.TicketsConcluidos++;
        metrica.PontosQuantidade += pesoPrioridade;
        MetricaCalculos.RegistrarPrioridade(metrica, ticket.Prioridade);

        if (ticket.DataPrazo.HasValue)
        {
            MetricaCalculos.RegistrarResultadoComPrazo(
                metrica,
                ticket.DataPrazo.Value,
                dataEntrega);
        }

        metrica.UltimaAtividade = dataEntrega;
        metrica.DataAtualizacao = DateTime.UtcNow;
        MetricaCalculos.RecalcularNotas(metrica);
        await _repository.SalvarAsync(cancellationToken);
    }

    public async Task AtualizarQualidadeSonarAsync(
        Guid usuarioId,
        MetricasAutorSonarDto metricasSonar,
        CancellationToken cancellationToken = default)
    {
        DateTime agora = DateTime.UtcNow;
        FuncionarioMetrica metrica = await ObterOuCriarAtualAsync(
            usuarioId,
            agora,
            cancellationToken);

        metrica.QualidadeSonar = MetricaCalculos.CalcularQualidade(metricasSonar);
        metrica.QualidadeManutencao =
            MetricaCalculos.LimitarNota(metricasSonar.NivelManutencao);
        metrica.QualidadeConfiabilidade =
            MetricaCalculos.LimitarNota(metricasSonar.NivelConfiabilidade);
        metrica.QualidadeSeguranca =
            MetricaCalculos.LimitarNota(metricasSonar.NivelSeguranca);
        metrica.IssuesManutencao = metricasSonar.IssuesManutencao;
        metrica.IssuesConfiabilidade = metricasSonar.IssuesConfiabilidade;
        metrica.IssuesSeguranca = metricasSonar.IssuesSeguranca;
        metrica.DataUltimaQualidadeSonar = agora;
        metrica.DataAtualizacao = agora;

        await _repository.SalvarAsync(cancellationToken);
    }

    public async Task FecharTrimestreAsync(
        DateTime dataReferencia,
        CancellationToken cancellationToken = default)
    {
        PeriodoMetrica periodo = PeriodoMetrica.Criar(dataReferencia);
        PeriodoMetrica proximoPeriodo = periodo.Proximo;
        List<FuncionarioMetrica> metricas =
            await _repository.ListarAtuaisPorPeriodoAsync(
                periodo.Inicio,
                periodo.Fim,
                cancellationToken);

        await using var transacao =
            await _repository.IniciarTransacaoAsync(cancellationToken);

        try
        {
            DateTime agora = DateTime.UtcNow;

            foreach (FuncionarioMetrica metrica in metricas)
            {
                if (await _repository.HistoricoExisteAsync(
                    metrica.UsuarioId,
                    periodo.Inicio,
                    periodo.Fim,
                    cancellationToken))
                {
                    continue;
                }

                await AtualizarAtrasosAsync(
                    metrica,
                    periodo,
                    agora,
                    cancellationToken);

                int diasSemAtividade = MetricaCalculos.CalcularDiasSemAtividade(
                    metrica,
                    agora);
                metrica.NotaVelocidade = MetricaCalculos.LimitarNota(
                    metrica.NotaVelocidade
                    - MetricaCalculos.CalcularPenalidadeInatividade(
                        diasSemAtividade));
                metrica.DiasSemAtividade = diasSemAtividade;

                _repository.AdicionarHistorico(
                    ConversaoMetrica.ConverterParaHistorico(metrica));
                MetricaCalculos.ResetarParaNovoPeriodo(
                    metrica,
                    proximoPeriodo);
            }

            await _repository.SalvarAsync(cancellationToken);
            await transacao.CommitAsync(cancellationToken);
        }
        catch
        {
            await transacao.RollbackAsync(cancellationToken);
            throw;
        }
    }

    public async Task<FuncionarioMetrica> ObterMetricasPorFuncionarioAsync(
        Guid usuarioId,
        CancellationToken cancellationToken = default)
    {
        FuncionarioMetrica? metrica =
            await _repository.ObterAtualAsync(usuarioId, cancellationToken);

        return metrica ?? throw new KeyNotFoundException(
            "Não existem métricas atuais para este funcionário.");
    }

    private async Task<FuncionarioMetrica> ObterOuCriarAtualAsync(
        Guid usuarioId,
        DateTime dataReferencia,
        CancellationToken cancellationToken)
    {
        FuncionarioMetrica? existente =
            await _repository.ObterAtualAsync(usuarioId, cancellationToken);

        if (existente is not null)
        {
            return existente;
        }

        FuncionarioMetrica novaMetrica = MetricaCalculos.CriarAtual(
            usuarioId,
            PeriodoMetrica.Criar(dataReferencia));
        _repository.AdicionarAtual(novaMetrica);
        return novaMetrica;
    }

    private async Task AtualizarAtrasosAsync(
        FuncionarioMetrica metrica,
        PeriodoMetrica periodo,
        DateTime agora,
        CancellationToken cancellationToken)
    {
        List<Ticket> tickets =
            await _repository.ListarTicketsDoPeriodoAsync(
                metrica.UsuarioId,
                periodo.Inicio,
                periodo.Fim,
                cancellationToken);

        metrica.TicketsAbertosAtrasados = 0;
        int diasAtraso = 0;

        foreach (Ticket ticket in tickets)
        {
            if (!ticket.DataPrazo.HasValue ||
                MetricaCalculos.EstaConcluido(ticket) ||
                agora <= ticket.DataPrazo.Value)
            {
                continue;
            }

            metrica.TicketsAbertosAtrasados++;
            diasAtraso += MetricaCalculos.CalcularDiasAtraso(
                ticket.DataPrazo.Value,
                agora);
        }

        metrica.DiasAtrasoAcumulados += diasAtraso;
        MetricaCalculos.RecalcularNotas(metrica);
    }
}
