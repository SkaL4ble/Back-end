using API.Domains;
using API.Interfaces;
using API.Repositories;

namespace API.Applications.Services;

public sealed class GameficacaoService
{
    private readonly IGameficacaoRepository _repository;

    public GameficacaoService(IGameficacaoRepository repository)
    {
        _repository = repository;
    }

    public async Task AdicionarExperienciaAsync(
        Guid usuarioId,
        int quantidade,
        CancellationToken cancellationToken = default)
    {
        if (quantidade <= 0)
        {
            throw new ArgumentException(
                "A experiência deve ser maior que zero.",
                nameof(quantidade));
        }

        var gamificacao =
            await ObterOuCriarGamificacaoAsync(
                usuarioId,
                cancellationToken);

        checked
        {
            gamificacao.ExperienciaTotal += quantidade;
        }

        gamificacao.Nivel =
            CalcularNivel(
                gamificacao.ExperienciaTotal);

        gamificacao.DataAtualizacao =
            DateTime.UtcNow;

        await _repository.SalvarAsync(
            cancellationToken);
    }

    public async Task RegistrarTicketConcluidoAsync(
    Ticket ticket,
    CancellationToken cancellationToken = default)
    {
        if (!ticket.UsuarioResponsavel.HasValue)
            return;

        await AdicionarExperienciaAsync(
            ticket.UsuarioResponsavel.Value,
            quantidade: 10,
            cancellationToken);
    }

    private async Task<FuncionarioGamificacao>
    ObterOuCriarGamificacaoAsync(
        Guid usuarioId,
        CancellationToken cancellationToken)
    {
        var gamificacao =
            await _repository.ObterGamificacaoAsync(
                usuarioId,
                cancellationToken);

        if (gamificacao is not null)
            return gamificacao;

        gamificacao = new FuncionarioGamificacao
        {
            UsuarioId = usuarioId,
            ExperienciaTotal = 0,
            Nivel = 1,
            DataAtualizacao = DateTime.UtcNow
        };

        _repository.AdicionarGamificacao(
            gamificacao);

        return gamificacao;
    }

    public async Task RegistrarTicketNoPrazoAsync(Ticket ticket, CancellationToken cancellationToken = default)
    {
        if (!ticket.UsuarioResponsavel.HasValue)
            return;

        if (!ticket.DataPrazo.HasValue || !ticket.DataEntrega.HasValue)
            return;

        if (ticket.DataEntrega.Value > ticket.DataPrazo.Value)
            return;
        

        await AdicionarExperienciaAsync(ticket.UsuarioResponsavel.Value, quantidade: 15, cancellationToken);
    }

    public async Task RegistrarParticipacaoTecnologiaAsync(Guid usuarioId, CancellationToken cancellationToken = default)
    {
        await AdicionarExperienciaAsync(usuarioId, quantidade: 40, cancellationToken);
    }

    public async Task<FuncionarioGamificacao>
        ObterPerfilAsync(
            Guid usuarioId,
            CancellationToken cancellationToken = default)
    {
        var gamificacao =
            await _repository.ObterGamificacaoAsync(
                usuarioId,
                cancellationToken);

        if (gamificacao is null)
        {
            gamificacao = new FuncionarioGamificacao
            {
                UsuarioId = usuarioId,
                ExperienciaTotal = 0,
                Nivel = 1,
                DataAtualizacao = DateTime.UtcNow
            };

            _repository.AdicionarGamificacao(
                gamificacao);

            await _repository.SalvarAsync(
                cancellationToken);
        }

        return gamificacao;
    }

    private static int CalcularNivel(
        int experienciaTotal)
    {
        if (experienciaTotal >= 6000)
            return 10;

        if (experienciaTotal >= 4200)
            return 9;

        if (experienciaTotal >= 3000)
            return 8;

        if (experienciaTotal >= 2100)
            return 7;

        if (experienciaTotal >= 1400)
            return 6;

        if (experienciaTotal >= 900)
            return 5;

        if (experienciaTotal >= 500)
            return 4;

        if (experienciaTotal >= 250)
            return 3;

        if (experienciaTotal >= 100)
            return 2;

        return 1;
    }
}