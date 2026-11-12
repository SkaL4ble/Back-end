using API.Applications.Services.SonarCube;
using API.Domains;
using API.DTOs.SonarQubeDto;
using API.DTOs.SonarQubeDto.CommitDto;
using API.Exceptions;
using API.Repositories;
using System.Globalization;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

namespace API.Applications.Services;

public class ProjetoSonarService
{
    private const int TamanhoMinimoNome = 3;
    private const int TamanhoMaximoNome = 255;
    private const int TamanhoMaximoDescricao = 500;

    // Aceita: https://github.com/dono/repo, https://github.com/dono/repo.git, github.com/dono/repo,
    // git@github.com:dono/repo.git e o formato curto dono/repo. Qualquer outro host é recusado.
    private static readonly Regex PadraoRepositorioGithub = new(
        @"^(?:(?:https?://)?(?:www\.)?github\.com/|git@github\.com:)?(?<dono>[A-Za-z0-9-]+)/(?<repo>[A-Za-z0-9._-]+?)(?:\.git)?(?:[/?#].*)?$",
        RegexOptions.IgnoreCase);

    private readonly ProjetoSonarRepository _repository;
    private readonly SonarQubeService _sonar;
    private readonly CriptografiaService _criptografia;
    private readonly string _urlBase;
    private readonly GitHubService _github;
    private readonly ProjetoService _serviceProjeto;

    public ProjetoSonarService(ProjetoSonarRepository repository, SonarQubeService sonar,
        CriptografiaService criptografia, IConfiguration config, GitHubService github, ProjetoService serviceProjeto)
    {
        _repository = repository;
        _sonar = sonar;
        _criptografia = criptografia;
        _urlBase = (config["SonarQube:UrlBase"] ?? string.Empty).TrimEnd('/');
        _github = github;
        _serviceProjeto = serviceProjeto;
    }

    // ============================== VALIDACOES ==============================

    private ProjetoSonar BuscarOuFalharSonar(Guid id) =>
        _repository.ObterPorId(id)
        ?? throw new DomainException("Projeto do SonarQube não encontrado");

    private void ValidarProjeto(Guid projetoId)
    {
        if (!_repository.ProjetoExiste(projetoId))
            throw new DomainException("Projeto não encontrado ou inativo");
    }

    private static string NormalizarNome(string? nome)
    {
        if (string.IsNullOrWhiteSpace(nome))
            throw new DomainException("O nome do projeto é obrigatório");

        var normalizado = nome.Trim();
        if (normalizado.Length < TamanhoMinimoNome)
            throw new DomainException($"O nome do projeto não pode ter menos de {TamanhoMinimoNome} caracteres");
        if (normalizado.Length > TamanhoMaximoNome)
            throw new DomainException($"O nome do projeto não pode ter mais de {TamanhoMaximoNome} caracteres");

        return normalizado;
    }

    private static string? NormalizarDescricao(string? descricao)
    {
        if (string.IsNullOrWhiteSpace(descricao))
            return null;

        var normalizada = descricao.Trim();
        if (normalizada.Length > TamanhoMaximoDescricao)
            throw new DomainException($"A descrição não pode ter mais de {TamanhoMaximoDescricao} caracteres");

        return normalizada;
    }

    private static string ExigirTexto(string? valor, string campo)
    {
        if (string.IsNullOrWhiteSpace(valor))
            throw new DomainException($"{campo} é obrigatório");

        return valor.Trim();
    }

    // Chave do SonarQube: só aceita letras, números, '-', '_', '.' e ':'.
    // O sufixo aleatório evita colisão entre projetos com o mesmo nome.
    private static string GerarChave(string nome)
    {
        var semAcento = new string(nome.Normalize(NormalizationForm.FormD)
            .Where(c => CharUnicodeInfo.GetUnicodeCategory(c) != UnicodeCategory.NonSpacingMark)
            .ToArray());

        var slug = Regex.Replace(semAcento.ToLowerInvariant(), @"[^a-z0-9._:-]+", "-").Trim('-');
        if (slug.Length == 0)
            slug = "projeto";
        if (slug.Length > 100)
            slug = slug[..100];

        return $"{slug}-{Guid.NewGuid().ToString("N")[..8]}";
    }

    private ListarProjetoSonarDto Converter(ProjetoSonar p) => new()
    {
        ProjetoSonarId = p.ProjetoSonarId,
        ProjetoId = p.ProjetoId,
        ChaveSonar = p.ChaveSonar,
        NomeSonar = p.NomeSonar,
        Descricao = p.Descricao,
        Origem = p.Origem,
        Plataforma = p.Plataforma,
        UrlProjeto = $"{_urlBase}/dashboard?id={Uri.EscapeDataString(p.ChaveSonar)}",
        DataCriacao = p.DataCriacao
    };

    // ============================== CRIACAO ==============================

    public async Task<List<PlataformaSonarDto>> ListarPlataformasAsync() =>
        await _sonar.ListarPlataformasAsync();

    public async Task<ListarProjetoSonarDto> CriarProjetoAsync(CriarProjetoSonarDto dto)
    {
        ValidarProjeto(dto.ProjetoId);
        var nome = NormalizarNome(dto.Nome);
        var descricao = NormalizarDescricao(dto.Descricao);
        var chave = GerarChave(nome);

        await _sonar.CriarProjetoAsync(chave, nome);

        return await RegistrarAsync(dto.ProjetoId, chave, nome, descricao, "LOCAL", null);
    }

    // Gera o token de análise e salva tudo no banco. Se o salvamento falhar,
    // remove o projeto recém-criado no SonarQube para não deixar registro órfão.
    private async Task<ListarProjetoSonarDto> RegistrarAsync(Guid projetoId, string chave, string nome,
        string? descricao, string origem, string? plataforma)
    {
        try
        {
            var tokenSonar = await _sonar.GerarTokenAnaliseAsync(chave);

            var entidade = new ProjetoSonar
            {
                ProjetoId = projetoId,
                ChaveSonar = chave,
                NomeSonar = nome,
                Descricao = descricao,
                Origem = origem,
                Plataforma = plataforma,
                TokenSonarCriptografado = _criptografia.Proteger(tokenSonar),
                DataCriacao = DateTime.UtcNow
            };

            _repository.Adicionar(entidade);

            var dto = Converter(entidade);
            dto.TokenAnalise = new TokenSonarDto { Token = tokenSonar };
            return dto;
        }
        catch
        {
            try { await _sonar.ExcluirProjetoAsync(chave); } catch { /* melhor esforço */ }
            throw;
        }
    }

    // ============================== CONSULTAS E TOKENS ==============================

    public ListarProjetoSonarDto ObterPorId(Guid id) => Converter(BuscarOuFalharSonar(id));

    public string ObterTokenSonar(Guid id)
    {
        var projetoSonar = BuscarOuFalharSonar(id);

        try
        {
            return _criptografia.Desproteger(projetoSonar.TokenSonarCriptografado);
        }
        catch (CryptographicException)
        {
            throw new DomainException("Não foi possível ler o token salvo (a chave de criptografia mudou)");
        }
    }

    public void AtualizarTokenGithub(Guid id, string? novoToken)
    {
        var projetoSonar = BuscarOuFalharSonar(id);
        var token = ExigirTexto(novoToken, "O token do GitHub");

        projetoSonar.TokenGithubCriptografado = _criptografia.Proteger(token);
        _repository.Atualizar(projetoSonar);
    }

    public async Task<List<IssueSonarDto>> ListarIssuesAsync(Guid id) =>
        await _sonar.ListarIssuesAsync(BuscarOuFalharSonar(id).ChaveSonar);

    public async Task<List<AtividadeSonarDto>> ListarAtividadesAsync(Guid id) =>
        await _sonar.ListarAtividadesAsync(BuscarOuFalharSonar(id).ChaveSonar);

    // ============================== INTEGRACAO COM GITHUB ==============================

    // O token do GitHub fica no ProjetoSonar.
    private string LerTokenGithub(ProjetoSonar projetoSonar)
    {
        if (projetoSonar.TokenGithubCriptografado is null)
            throw new DomainException("Este projeto não tem token do GitHub cadastrado");

        try
        {
            return _criptografia.Desproteger(projetoSonar.TokenGithubCriptografado);
        }
        catch (CryptographicException)
        {
            throw new DomainException("Não foi possível ler o token do GitHub salvo (a chave de criptografia mudou)");
        }
    }

    // Transforma o LinkRepositorio do Projeto (uma URL) no formato "dono/repo" que a API do GitHub usa.
    private static string ExtrairRepositorio(string? link)
    {
        if (string.IsNullOrWhiteSpace(link))
            throw new DomainException("Este projeto não tem o link do repositório cadastrado");

        var resultado = PadraoRepositorioGithub.Match(link.Trim());
        if (!resultado.Success)
            throw new DomainException("O link do repositório não é um repositório válido do GitHub");

        return $"{resultado.Groups["dono"].Value}/{resultado.Groups["repo"].Value}";
    }

    // Repositório vem do Projeto (LinkRepositorio); o token vem do ProjetoSonar.
    private (string Repositorio, string Token) ObterAcessoGithub(ProjetoSonar projetoSonar)
    {
        var projeto = _serviceProjeto.BuscarOuFalhar(projetoSonar.ProjetoId);

        var repositorio = ExtrairRepositorio(projeto.LinkRepositorio);
        var token = LerTokenGithub(projetoSonar);

        return (repositorio, token);
    }

    public async Task<CommitIssueDto> ObterCommitDaIssueAsync(Guid id, string chaveIssue)
    {
        var projetoSonar = BuscarOuFalharSonar(id);
        var (repositorio, tokenGithub) = ObterAcessoGithub(projetoSonar);

        var issue = await _sonar.ObterIssueAsync(projetoSonar.ChaveSonar, chaveIssue);
        if (issue.Linha is null)
            throw new DomainException("Esta issue não está ligada a uma linha de código");

        var scm = await _sonar.ObterScmLinhaAsync(issue.Componente, issue.Linha.Value);
        if (scm is null || string.IsNullOrWhiteSpace(scm.Revisao))
            throw new DomainException("O SonarQube não tem informação de commit para esta linha");

        return await _github.ObterCommitAsync(repositorio, scm.Revisao, tokenGithub);
    }
}