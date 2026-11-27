using API.Applications.Auxiliares;
using API.Applications.Conversoes.ConversoesDeDto;
using API.Domains;
using API.DTOs;
using API.DTOs.GameficacaoDto;
using API.Exceptions;
using API.Interfaces;

namespace API.Applications.Services;

public class GameficacaoService
{
    private readonly IGameficacaoRepository _repository;
    private readonly CriarRecomendacaoTreinamento _recomendacao;

    public GameficacaoService(
        IGameficacaoRepository repository,
        CriarRecomendacaoTreinamento recomendacao)
    {
        _repository = repository;
        _recomendacao = recomendacao;
    }

    // ================================ VALIDAÇÕES ================================

    private static void ValidarQuantidadeExperiencia(
        int quantidade)
    {
        if (quantidade <= 0)
        {
            throw new DomainException(
                "Quantidade não pode ser menor ou igual a zero.");
        }
    }

    private static void ValidarPontuacao(
        decimal? pontuacao)
    {
        if (!pontuacao.HasValue)
            return;

        if (pontuacao.Value < 0 ||
            pontuacao.Value > 100)
        {
            throw new DomainException(
                "A pontuação deve estar entre 0 e 100.");
        }
    }

    // ================================ LISTAGEM ================================

    public ListarGameficacaoDto ListarPorFuncionario(
        Guid usuarioId)
    {
        var gameficacao =
            ObterOuCriarGameficacao(usuarioId);

        var tiposTicket =
            _repository.ListarTiposTicketPorUsuarioId(
                usuarioId);

        var recomendacoes =
            _recomendacao.CriarRecomendacoes(
                tiposTicket);

        return ConversaoGameficacao.ConverterParaDto(
            gameficacao,
            tiposTicket,
            recomendacoes);
    }

    // ================================ EXPERIÊNCIA ================================

    public void AdicionarExperienciaGeral(
        Guid usuarioId,
        int quantidade)
    {
        ValidarQuantidadeExperiencia(
            quantidade);

        var gameficacao =
            ObterOuCriarGameficacao(usuarioId);

        checked
        {
            gameficacao.ExperienciaTotal +=
                quantidade;
        }

        gameficacao.DataAtualizacao =
            DateTime.UtcNow;

        _repository.AtualizarGameficacao(
            gameficacao);

        _repository.Salvar();
    }

    public void AdicionarExperienciaTipoTicket(
        Guid usuarioId,
        int tipoTicketId,
        int quantidade,
        decimal? pontuacao = null)
    {
        ValidarQuantidadeExperiencia(
            quantidade);

        ValidarPontuacao(
            pontuacao);

        var tipoTicketUsuario =
            _repository.ObterTipoTicketDoUsuario(
                usuarioId,
                tipoTicketId);

        if (tipoTicketUsuario is null)
        {
            tipoTicketUsuario =
                ConversaoGameficacao
                    .CriarTipoTicketUsuario(
                        usuarioId,
                        tipoTicketId,
                        quantidade,
                        pontuacao);

            _repository.AdicionarTipoTicketUsuario(
                tipoTicketUsuario);
        }
        else
        {
            AtualizarTipoTicketUsuario(
                tipoTicketUsuario,
                quantidade,
                pontuacao);

            _repository.AtualizarTipoTicketUsuario(
                tipoTicketUsuario);
        }

        _repository.Salvar();

        var gameficacao =
            ObterOuCriarGameficacao(
                usuarioId);

        AtualizarNivelGeral(
            gameficacao,
            usuarioId);

        gameficacao.DataAtualizacao =
            DateTime.UtcNow;

        _repository.AtualizarGameficacao(
            gameficacao);

        _repository.Salvar();
    }

    public void RegistrarTicketConcluido(
        Ticket ticket)
    {
        if (!ticket.UsuarioResponsavel.HasValue)
            return;

        var usuarioId =
            ticket.UsuarioResponsavel.Value;

        AdicionarExperienciaGeral(
            usuarioId,
            10);

        AdicionarExperienciaTipoTicket(
            usuarioId,
            ticket.TipoTicketId,
            10,
            CalcularPontuacaoTicket(
                ticket));
    }

    public void RegistrarTicketEntregueNoPrazo(
        Ticket ticket)
    {
        if (!ticket.UsuarioResponsavel.HasValue)
            return;

        if (!ticket.DataPrazo.HasValue ||
            !ticket.DataEntrega.HasValue)
        {
            return;
        }

        if (ticket.DataEntrega.Value >
            ticket.DataPrazo.Value)
        {
            return;
        }

        AdicionarExperienciaGeral(
            ticket.UsuarioResponsavel.Value,
            15);
    }

    // ================================ FUNÇÕES ================================

    private UsuarioGameficacao
        ObterOuCriarGameficacao(
            Guid usuarioId)
    {
        var gameficacao =
            _repository.ObterPorUsuarioId(
                usuarioId);

        if (gameficacao is not null)
            return gameficacao;

        gameficacao =
            new UsuarioGameficacao
            {
                UsuarioId =
                    usuarioId,

                ExperienciaTotal =
                    0,

                NivelGeral =
                    0,

                DataAtualizacao =
                    DateTime.UtcNow
            };

        _repository.AdicionarGameficacao(
            gameficacao);

        _repository.Salvar();

        return gameficacao;
    }

    private void AtualizarNivelGeral(
        UsuarioGameficacao gameficacao,
        Guid usuarioId)
    {
        var tiposTicket =
            _repository.ListarTiposTicketPorUsuarioId(
                usuarioId);

        if (tiposTicket.Count == 0)
        {
            gameficacao.NivelGeral = 0;
            return;
        }

        var media =
            tiposTicket.Average(
                x => x.NivelTipoTicket);

        gameficacao.NivelGeral =
            Math.Clamp(
                (int)Math.Round(
                    media,
                    MidpointRounding.AwayFromZero),
                0,
                10);
    }

    private static void AtualizarTipoTicketUsuario(
        TipoTicketUsuario tipoTicketUsuario,
        int quantidade,
        decimal? pontuacao)
    {
        checked
        {
            tipoTicketUsuario
                .ExperienciaTipoTicket +=
                quantidade;
        }

        if (pontuacao.HasValue)
        {
            tipoTicketUsuario
                .NumeroAtribuicoes++;

            tipoTicketUsuario
                .ValorAtribuicoes +=
                pontuacao.Value;

            tipoTicketUsuario
                .AfinidadeMedia =
                Math.Round(
                    tipoTicketUsuario
                        .ValorAtribuicoes
                    / tipoTicketUsuario
                        .NumeroAtribuicoes,
                    2,
                    MidpointRounding.AwayFromZero);
        }

        tipoTicketUsuario
            .NivelTipoTicket =
            CalcularNivelGameficacao
                .CalcularNivel(
                    tipoTicketUsuario
                        .ExperienciaTipoTicket);

        tipoTicketUsuario.DataAtualizacao =
            DateTime.UtcNow;
    }

    private static decimal CalcularPontuacaoTicket(
        Ticket ticket)
    {
        decimal pontuacao = 50;

        if (ticket.DataPrazo.HasValue &&
            ticket.DataEntrega.HasValue)
        {
            if (ticket.DataEntrega.Value <=
                ticket.DataPrazo.Value)
            {
                pontuacao += 30;
            }
            else
            {
                pontuacao -= 20;
            }
        }

        if (ticket.Prioridade == 1)
        {
            pontuacao += 20;
        }
        else if (ticket.Prioridade == 2)
        {
            pontuacao += 10;
        }

        return Math.Clamp(
            pontuacao,
            0,
            100);
    }
}