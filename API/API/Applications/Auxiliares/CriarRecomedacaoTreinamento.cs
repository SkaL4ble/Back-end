using API.Applications.Conversoes.ConversoesDeDto;
using API.Domains;
using API.DTOs.GameficacaoDto;
using API.Interfaces;

namespace API.Applications.Auxiliares;

public class CriarRecomendacaoTreinamento
{
    private readonly IGameficacaoRepository
        _repository;

    public CriarRecomendacaoTreinamento(
        IGameficacaoRepository repository)
    {
        _repository = repository;
    }

    public List<RecomendacaoTipoTicketDto>
        CriarRecomendacoes(
            List<TipoTicketUsuario>
                tiposTicketUsuario)
    {
        var demandas =
            _repository.ListarDemandasPorTipoTicket();

        var tiposConhecidos =
            tiposTicketUsuario
                .ToDictionary(
                    x => x.TipoTicketId);

        var recomendacoes =
            new List<RecomendacaoTipoTicketDto>();

        foreach (var demanda in demandas)
        {
            tiposConhecidos.TryGetValue(
                demanda.TipoTicketId,
                out var tipoTicketUsuario);

            var nivelAtual =
                tipoTicketUsuario?
                    .NivelTipoTicket ?? 0;

            var tipoRecomendacao =
                ObterTipoRecomendacao(
                    nivelAtual);

            if (tipoRecomendacao is null)
                continue;

            var prioridade =
                CalcularPrioridade(
                    demanda,
                    nivelAtual);

            var trilhas =
                _repository.ListarTrilhasPorTipoTicket(
                    demanda.TipoTicketId);

            recomendacoes.Add(
                CriarRecomendacao(
                    demanda,
                    nivelAtual,
                    tipoRecomendacao,
                    prioridade,
                    trilhas));
        }

        return recomendacoes
            .OrderByDescending(
                x => x.Prioridade)
            .ThenByDescending(
                x => x.QuantidadeTicketsRelacionados)
            .Take(10)
            .ToList();
    }

    private static string?
        ObterTipoRecomendacao(
            int nivelAtual)
    {
        if (nivelAtual == 0)
            return "Aprender";

        if (nivelAtual < 7)
            return "Melhorar";

        return null;
    }

    private static decimal CalcularPrioridade(
        DemandaTipoTicketDto demanda,
        int nivelAtual)
    {
        return demanda.QuantidadeTickets
            * (11 - nivelAtual);
    }

    private static RecomendacaoTipoTicketDto
        CriarRecomendacao(
            DemandaTipoTicketDto demanda,
            int nivelAtual,
            string tipoRecomendacao,
            decimal prioridade,
            List<TrilhaTreinamento> trilhas)
    {
        var nivelRecomendado =
            Math.Min(
                10,
                nivelAtual + 3);

        var justificativa =
            tipoRecomendacao == "Aprender"
                ? $"O tipo de ticket " +
                  $"{demanda.NomeTipoTicket} possui " +
                  $"{demanda.QuantidadeTickets} " +
                  $"ticket(s) em " +
                  $"{demanda.QuantidadeProjetos} " +
                  $"projeto(s). O funcionário " +
                  $"está no nível 0."
                : $"O tipo de ticket " +
                  $"{demanda.NomeTipoTicket} possui " +
                  $"{demanda.QuantidadeTickets} " +
                  $"ticket(s) em " +
                  $"{demanda.QuantidadeProjetos} " +
                  $"projeto(s). O funcionário " +
                  $"está no nível {nivelAtual} " +
                  $"e pode melhorar.";

        return new RecomendacaoTipoTicketDto
        {
            TipoTicketId =
                demanda.TipoTicketId,

            NomeTipoTicket =
                demanda.NomeTipoTicket,

            TipoRecomendacao =
                tipoRecomendacao,

            Justificativa =
                justificativa,

            NivelAtual =
                nivelAtual,

            NivelRecomendado =
                nivelRecomendado,

            QuantidadeTicketsRelacionados =
                demanda.QuantidadeTickets,

            QuantidadeProjetosRelacionados =
                demanda.QuantidadeProjetos,

            Prioridade =
                prioridade,

            Trilhas =
                trilhas
                    .Where(
                        x =>
                            x.NivelFinal >=
                            nivelAtual)
                    .Select(
                        x => new TrilhaRecomendadaDto
                        {
                            TrilhaTreinamentoId =
                                x.TrilhaTreinamentoId,

                            NomeTrilha =
                                x.NomeTrilha,

                            Descricao =
                                x.Descricao,

                            NivelInicial =
                                x.NivelInicial,

                            NivelFinal =
                                x.NivelFinal
                        })
                    .ToList()
        };
    }
}