
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
    private readonly CriarRecomedacaoTreinamento _recomendacao;

    public GameficacaoService(IGameficacaoRepository repository, CriarRecomedacaoTreinamento recomendacao)
    {
        _repository = repository;
        _recomendacao = recomendacao;
    }

        // ================================ VALIDAÇÕES ================================

    private static void ValidarQuantidadeExperiencia(int quantidade)
    {
        if (quantidade <= 0)
        {
            throw new DomainException("Quantidade não pode ser menor ou igual a zero.");
        }
    }

    // ================================ LISTAGEM ================================

    public ListarGameficacaoDto ListarPorFuncionario(Guid usuarioId)
    {
        var gameficacao = ObterOuCriarGameficacao(usuarioId);

        var tecnologias = _repository.ListarTecnologiasPorUsuarioId(usuarioId);

        var recomendacoes = _recomendacao.CriarRecomendacoes(tecnologias);

        return ConversaoGameficacao.ConverterParaDto(gameficacao, tecnologias, recomendacoes);
    }

    // ================================ EXPERIÊNCIA ================================

    public void AdicionarExperienciaGeral(Guid usuarioId, int quantidade)
    {
        ValidarQuantidadeExperiencia(quantidade);

        var gameficacao = ObterOuCriarGameficacao(usuarioId);

        gameficacao.ExperienciaTotal += quantidade;

        gameficacao.DataAtualizacao = DateTime.UtcNow;

        AtualizarNivelGeral(gameficacao, usuarioId);

        _repository.AtualizarGameficacao(gameficacao);

        _repository.Salvar();
    }

    public void AdicionarExperienciaTecnologia(Guid usuarioId, int tecnologiaId, int quantidade, decimal? pontuacao = null)
    {
        ValidarQuantidadeExperiencia(quantidade);

        var tecnologiaUsuario = _repository.ObterTecnologiaDoUsuario(usuarioId, tecnologiaId);

        if (tecnologiaUsuario is null)
        {
            tecnologiaUsuario = ConversaoGameficacao.CriarTecnologiaUsuario(usuarioId, tecnologiaId, quantidade, pontuacao);

            _repository.AdicionarTecnologiaUsuario(tecnologiaUsuario);
        }
        else
        {
            AtualizarTecnologiaUsuario(tecnologiaUsuario, quantidade, pontuacao);

            _repository.AtualizarTecnologiaUsuario(tecnologiaUsuario);
        }

        var gameficacao = ObterOuCriarGameficacao(usuarioId);

        AtualizarNivelGeral(gameficacao, usuarioId);

        gameficacao.DataAtualizacao = DateTime.UtcNow;

        _repository.AtualizarGameficacao(gameficacao);

        _repository.Salvar();
    }

    // ================================ FUNÇÕES ================================

    private UsuarioGameficacao ObterOuCriarGameficacao(Guid usuarioId)
    {
        var gameficacao = _repository.ObterPorUsuarioId(usuarioId);

        if (gameficacao is not null)
            return gameficacao;

        gameficacao = new UsuarioGameficacao
        {
            UsuarioId = usuarioId,

            ExperienciaTotal = 0,

            NivelGeral = 0,

            DataAtualizacao = DateTime.UtcNow
        };

        _repository.AdicionarGameficacao(gameficacao);

        _repository.Salvar();

        return gameficacao;
    }

    private void AtualizarNivelGeral(UsuarioGameficacao gameficacao, Guid usuarioId)
    {
        var tecnologias = _repository.ListarTecnologiasPorUsuarioId(usuarioId);

        if (tecnologias.Count == 0)
        {
            gameficacao.NivelGeral = 0;
            return;
        }

        var media = tecnologias.Average(x => x.NivelTecnologia);

        gameficacao.NivelGeral = Math.Clamp( (int)Math.Round(media, MidpointRounding.AwayFromZero), 0, 10);
    }

    private static void AtualizarTecnologiaUsuario(TecnologiaUsuario tecnologia, int quantidade, decimal? pontuacao)
    {
        checked
        {
            tecnologia.ExperienciaTecnologia += quantidade;
        }

        if (pontuacao.HasValue)
        {
            tecnologia.NumeroAtribuicoes++;

            tecnologia.ValorAtribuicoes += pontuacao.Value;

            tecnologia.AfinidadeMedia = Math.Round(
                tecnologia.ValorAtribuicoes / tecnologia.NumeroAtribuicoes,
                2,
                MidpointRounding.AwayFromZero);
        }

        tecnologia.NivelTecnologia = CalcularNivelGameficacao.CalcularNivel(tecnologia.ExperienciaTecnologia);

        tecnologia.DataAtualizacao = DateTime.UtcNow;
    }
}
