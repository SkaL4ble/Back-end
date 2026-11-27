using API.Applications.Auxiliares;
using API.Domains;
using API.DTOs;
using API.DTOs.GameficacaoDto;

namespace API.Applications.Conversoes.ConversoesDeDto
{
    public class ConversaoGameficacao
    {
        public static ListarGameficacaoDto ConverterParaDto(UsuarioGameficacao gameficacao, List<TecnologiaUsuario> tecnologias, List<RecomendacaoTecnologiaDto> recomendacoes)
        {
            return new ListarGameficacaoDto
            {
                UsuarioId = gameficacao.UsuarioId,

                ExperienciaTotal = gameficacao.ExperienciaTotal,

                NivelGeral = gameficacao.NivelGeral,

                DataAtualizacao = gameficacao.DataAtualizacao,

                Tecnologias = tecnologias
                    .Select(ConverterTecnologia)
                    .ToList(),

                Recomendacoes = recomendacoes
            };
        }

        public static ListarTecnologiaGameficacaoDto ConverterTecnologia(TecnologiaUsuario tecnologia)
        {
            return new ListarTecnologiaGameficacaoDto
            {
                TecnologiaId = tecnologia.TecnologiaId,

                NomeTecnologia = tecnologia.Tecnologia.NomeTecnologia,

                AfinidadeMedia = tecnologia.AfinidadeMedia,

                ExperienciaTecnologia = tecnologia.ExperienciaTecnologia,

                NivelTecnologia = tecnologia.NivelTecnologia,

                NumeroAtribuicoes = tecnologia.NumeroAtribuicoes
            };
        }

        public static RecomendacaoTecnologiaDto DemandaParaRecomendacao(DemandaTecnologiaDto demanda, int nivelAtual, string tipoRecomendacao, 
            decimal prioridade, List<TrilhaTreinamento> trilhas, string justificativa, int nivelRecomendado)
        {
            return new RecomendacaoTecnologiaDto
            {
                TecnologiaId = demanda.TecnologiaId,

                NomeTecnologia = demanda.NomeTecnologia,

                TipoRecomendacao = tipoRecomendacao,

                Justificativa = justificativa,

                NivelAtual = nivelAtual,

                NivelRecomendado = nivelRecomendado,

                QuantidadeTicketsRelacionados = demanda.QuantidadeTickets,

                QuantidadeProjetosRelacionados = demanda.QuantidadeProjetos,

                Prioridade = prioridade,

                Trilhas = trilhas
                    .Where(x => x.NivelFinal >= nivelAtual)
                    .Select(x => new TrilhaRecomendadaDto
                    {
                        TrilhaTreinamentoId = x.TrilhaTreinamentoId,

                        NomeTrilha = x.NomeTrilha,

                        Descricao = x.Descricao,

                        NivelInicial = x.NivelInicial,

                        NivelFinal = x.NivelFinal
                    })
                    .ToList()
            };
        }

        public static TecnologiaUsuario CriarTecnologiaUsuario(Guid usuarioId, int tecnologiaId, int quantidade, decimal? pontuacao)
        {
            var pontuacaoInicial = CalcularNivelGameficacao.LimitarNota(pontuacao ?? 0);

            return new TecnologiaUsuario
            {
                UsuarioId = usuarioId,

                TecnologiaId = tecnologiaId,

                AfinidadeMedia = pontuacaoInicial,

                ExperienciaTecnologia = quantidade,

                NivelTecnologia = CalcularNivelGameficacao.CalcularNivel(quantidade),

                NumeroAtribuicoes = pontuacao.HasValue ? 1 : 0,

                ValorAtribuicoes = pontuacao ?? 0,

                DataAtualizacao = DateTime.UtcNow
            };
        }
    }
}
