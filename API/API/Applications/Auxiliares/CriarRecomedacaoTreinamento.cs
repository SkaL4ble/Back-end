using API.Applications.Conversoes.ConversoesDeDto;
using API.Domains;
using API.DTOs;
using API.DTOs.GameficacaoDto;
using API.Interfaces;

namespace API.Applications.Auxiliares
{
    public class CriarRecomedacaoTreinamento
    {
        private readonly IGameficacaoRepository _repository;
        public CriarRecomedacaoTreinamento(IGameficacaoRepository repository)
        {
            _repository = repository;
        }

        public RecomendacaoTecnologiaDto CriarRecomendacao(DemandaTecnologiaDto demanda, int nivelAtual, string tipoRecomendacao, decimal prioridade, List<TrilhaTreinamento> trilhas)
        {
            var nivelRecomendado = Math.Min(10, nivelAtual + 3);

            var justificativa =
                tipoRecomendacao == "Aprender"
                    ? $"A tecnologia {demanda.NomeTecnologia} " +
                      $"está presente em " +
                      $"{demanda.QuantidadeProjetos} projeto(s) " +
                      $"e possui " +
                      $"{demanda.QuantidadeTickets} ticket(s) " +
                      $"relacionado(s). O funcionário está " +
                      $"no nível 0."
                    : $"A tecnologia {demanda.NomeTecnologia} " +
                      $"possui demanda em " +
                      $"{demanda.QuantidadeProjetos} projeto(s) " +
                      $"e " +
                      $"{demanda.QuantidadeTickets} ticket(s). " +
                      $"O funcionário está no nível " +
                      $"{nivelAtual} e pode melhorar.";

            return ConversaoGameficacao.DemandaParaRecomendacao(demanda, nivelAtual, tipoRecomendacao, prioridade, trilhas, justificativa, nivelRecomendado);
        }
        private static string? ObterTipoRecomendacao(int nivelAtual)
        {
            if (nivelAtual == 0)
                return "Aprender";

            if (nivelAtual < 7)
                return "Melhorar";

            return null;
        }

        private static decimal CalcularPrioridade(DemandaTecnologiaDto demanda, int nivelAtual)
        {
            return demanda.QuantidadeTickets * (11 - nivelAtual);
        }

        public List<RecomendacaoTecnologiaDto> CriarRecomendacoes(List<TecnologiaUsuario> tecnologiasUsuario)
        {
            var demandas = _repository.ListarDemandasTecnologicas();

            var tecnologiasConhecidas = tecnologiasUsuario.ToDictionary(x => x.TecnologiaId);

            var recomendacoes = new List<RecomendacaoTecnologiaDto>();

            foreach (var demanda in demandas)
            {
                tecnologiasConhecidas.TryGetValue(demanda.TecnologiaId, out var tecnologiaUsuario);

                var nivelAtual = tecnologiaUsuario?.NivelTecnologia ?? 0;

                var tipoRecomendacao = ObterTipoRecomendacao(nivelAtual);

                if (tipoRecomendacao is null)
                    continue;

                var prioridade = CalcularPrioridade(demanda, nivelAtual);

                var trilhas = _repository.ListarTrilhasPorTecnologia(demanda.TecnologiaId);

                recomendacoes.Add(CriarRecomendacao(demanda, nivelAtual, tipoRecomendacao, prioridade, trilhas));
            }

            return recomendacoes
                .OrderByDescending(x => x.Prioridade)
                .ThenByDescending(x => x.QuantidadeTicketsRelacionados)
                .Take(10)
                .ToList();
        }
    }
}
