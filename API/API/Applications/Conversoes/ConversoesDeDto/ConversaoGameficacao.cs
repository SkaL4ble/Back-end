using API.Domains;
using API.DTOs;
using API.DTOs.GameficacaoDto;

namespace API.Applications.Conversoes.ConversoesDeDto;

public class ConversaoGameficacao
{
    public static ListarGameficacaoDto ConverterParaDto(
        UsuarioGameficacao gameficacao,
        List<TipoTicketUsuario> tiposTicket,
        List<RecomendacaoTipoTicketDto> recomendacoes)
    {
        return new ListarGameficacaoDto
        {
            UsuarioId =
                gameficacao.UsuarioId,

            ExperienciaTotal =
                gameficacao.ExperienciaTotal,

            NivelGeral =
                gameficacao.NivelGeral,

            DataAtualizacao =
                gameficacao.DataAtualizacao,

            TiposTicket =
                tiposTicket
                    .Select(
                        ConverterTipoTicket)
                    .ToList(),

            Recomendacoes =
                recomendacoes
        };
    }

    public static ListarTipoTicketGameficacaoDto
        ConverterTipoTicket(
            TipoTicketUsuario tipoTicket)
    {
        return new ListarTipoTicketGameficacaoDto
        {
            TipoTicketId =
                tipoTicket.TipoTicketId,

            NomeTipoTicket =
                tipoTicket.TipoTicket
                    .NomeTipoTicket,

            AfinidadeMedia =
                tipoTicket.AfinidadeMedia,

            ExperienciaTipoTicket =
                tipoTicket
                    .ExperienciaTipoTicket,

            NivelTipoTicket =
                tipoTicket
                    .NivelTipoTicket,

            NumeroAtribuicoes =
                tipoTicket
                    .NumeroAtribuicoes
        };
    }

    public static TipoTicketUsuario
        CriarTipoTicketUsuario(
            Guid usuarioId,
            int tipoTicketId,
            int quantidade,
            decimal? pontuacao)
    {
        var nota =
            Math.Clamp(
                pontuacao ?? 0,
                0,
                100);

        return new TipoTicketUsuario
        {
            UsuarioId =
                usuarioId,

            TipoTicketId =
                tipoTicketId,

            AfinidadeMedia =
                nota,

            ExperienciaTipoTicket =
                quantidade,

            NivelTipoTicket =
                Applications.Auxiliares
                    .CalcularNivelGameficacao
                    .CalcularNivel(
                        quantidade),

            NumeroAtribuicoes =
                pontuacao.HasValue
                    ? 1
                    : 0,

            ValorAtribuicoes =
                pontuacao ?? 0,

            DataAtualizacao =
                DateTime.UtcNow
        };
    }
}