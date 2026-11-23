using API.Applications.Conversoes.ConversoesDeDto;
using API.Applications.GerarYml;
using API.Applications.Services.SonarCube;
using API.Domains;
using API.DTOs.SonarQubeDto;
using API.DTOs.SonarQubeDto.CommitDto;
using API.DTOs.SonarQubeDto.ListagemDto;
using API.DTOs.SonarQubeDto.WorkFlowDto;
using API.Exceptions;
using API.Interfaces;
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

    private readonly IProjetoSonarRepository _repository;
    private readonly SonarQubeService _sonar;
    private readonly CriptografiaService _criptografia;
    private readonly string _urlBase;
    private readonly UsuarioService _usuarioService;

    public ProjetoSonarService(IProjetoSonarRepository repository, SonarQubeService sonar,
        CriptografiaService criptografia, IConfiguration config, UsuarioService usuarioService)
    {
        _repository = repository;
        _sonar = sonar;
        _criptografia = criptografia;
        _urlBase = (config["SonarQube:UrlBase"] ?? string.Empty).TrimEnd('/');
        _usuarioService = usuarioService;
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


    // Chave do SonarQube: só aceita letras, números, '-', '_', '.' e ':'.
    // O sufixo aleatório evita colisão entre projetos com o mesmo nome.
    public string GerarChave(string nome)
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
        WorkflowId = p.WorkflowId,
        UrlProjeto = $"{_urlBase}/dashboard?id={Uri.EscapeDataString(p.ChaveSonar)}",
        DataCriacao = p.DataCriacao
    };

    // ============================== CRIACAO ==============================

    public List<ListarWorkFlowDto> ListarTiposWorkflow()
    {
        List<ListarWorkFlowDto> workFlowDtos = _repository.ObterWorkflows()
            .Select(ConversaoSonarQube.ConverterWorkFlowParaDto)
            .ToList();
        return workFlowDtos;
    }

    public async Task<ListarProjetoSonarDto> CriarProjetoAsync(CriarProjetoSonarDto dto)
    {
        ValidarProjeto(dto.ProjetoId);
        var nome = NormalizarNome(dto.Nome);
        var descricao = NormalizarDescricao(dto.Descricao);
        var chave = GerarChave(nome);

        await _sonar.CriarProjetoAsync(chave, nome);

        return await RegistrarAsync(dto.ProjetoId, chave, nome, descricao, "LOCAL", dto.WorkflowId);
    }

    public async Task<ListarProjetoSonarDto> CriarProjetoSonarJuntoAProjeto(Projeto projeto, int workflowId)
    {
        ValidarProjeto(projeto.ProjetoId);
        var nome = NormalizarNome(projeto.NomeProjeto);
        if (!_repository.WorkflowExiste(workflowId))
            workflowId = 1;
        var descricao = NormalizarDescricao(projeto.Descricao);
        var chave = GerarChave(nome);

        await _sonar.CriarProjetoAsync(chave, nome);

        return await RegistrarAsync(projeto.ProjetoId, chave, nome, descricao, "LOCAL", workflowId);
    }

    // Gera o token de análise e salva tudo no banco. Se o salvamento falhar,
    // remove o projeto recém-criado no SonarQube para não deixar registro órfão.
    private async Task<ListarProjetoSonarDto> RegistrarAsync(Guid projetoId, string chave, string nome,
        string? descricao, string origem, int workflowId)
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
                WorkflowId = workflowId,
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

    public ListarProjetoSonarDto ObterPorId(Guid id) {
        var usuario = Converter(BuscarOuFalharSonar(id));
        if (usuario == null)
            throw new DomainException("ID invaliado");
        return usuario;
    }

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

    public ConfiguracaoSonarQubeDto ObterConfiguracao(Guid id)
    {
        var projetoSonar = BuscarOuFalharSonar(id);
        const string branch = "develop";

        string token;
        try
        {
            token = _criptografia.Desproteger(projetoSonar.TokenSonarCriptografado);
        }
        catch (CryptographicException)
        {
            throw new DomainException("Não foi possível ler o token salvo (a chave de criptografia mudou)");
        }

        var yaml = ModeloWorkflowSonar.Gerar(projetoSonar.WorkflowId, projetoSonar.ChaveSonar, branch);

        return new ConfiguracaoSonarQubeDto
        {
            ProjetoSonarId = projetoSonar.ProjetoSonarId,
            ChaveSonar = projetoSonar.ChaveSonar,
            TokenSonar = token,
            UrlSonarQube = _urlBase,            Branch = branch,
            ConteudoYaml = yaml,
            SecretsGithub = new List<SecretWorkFlowDto>
            {
                new()
                {
                    Nome = "SONAR_TOKEN",
                    Descricao = "Token de análise gerado para este projeto",
                    Valor = token
                },
                new()
                {
                    Nome = "SONAR_HOST_URL",
                    Descricao = "URL base do servidor SonarQube",
                    Valor = _urlBase
                }
            }
        };
    }

    public async Task<List<IssueSonarDto>> ListarIssuesAsync(Guid id) =>
        await _sonar.ListarIssuesAsync(BuscarOuFalharSonar(id).ChaveSonar);
    public async Task<List<IssueSonarDto>>
    ListarIssuesPorUsuarioAsync(Guid usuarioId)
    {
        var usuario = _usuarioService.BuscarPorId(usuarioId);
        var emailFuncionario = usuario.EmailPessoal?.Trim();

        if (string.IsNullOrWhiteSpace(emailFuncionario))
        {
            throw new DomainException(
                "O funcionário não possui e-mail cadastrado");
        }

        var idsProjetos = _repository.ListarIdsPorUsuario(usuarioId);

        if (idsProjetos.Count == 0)
        {
            throw new DomainException(
                "O funcionário não está cadastrado em nenhum projeto");
        }

        var issuesDoFuncionario = new List<IssueSonarDto>();

        foreach (var projetoId in idsProjetos)
        {
            var chavesSonar =
                _repository.ListarChavesSonarPorProjeto(projetoId);

            foreach (var chaveSonar in chavesSonar)
            {
                var issues =
                    await _sonar.ListarIssuesPorFuncionarioAsync(
                        chaveSonar,
                        emailFuncionario);

                issuesDoFuncionario.AddRange(issues);
            }
        }

        var issuesUnicas = issuesDoFuncionario
            .GroupBy(
                issue => issue.Chave,
                StringComparer.OrdinalIgnoreCase)
            .Select(grupo => grupo.First())
            .OrderBy(issue => issue.DataCriacao)
            .ToList();

        return issuesUnicas;
    }

    // ============================== METRICAS ================================

    public async Task<MetricasAutorSonarDto>
    ObterMetricasPorUsuarioAsync(Guid usuarioId)
    {
        var usuario = _usuarioService.BuscarPorId(usuarioId);
        var emailAutor = usuario.EmailPessoal?.Trim();

        if (string.IsNullOrWhiteSpace(emailAutor))
        {
            throw new DomainException(
                "O funcionário não possui e-mail cadastrado");
        }

        var issues = await ListarIssuesPorUsuarioAsync(usuarioId);

        var issuesManutencao = issues
            .Where(EhIssueDeManutencao)
            .ToList();

        var issuesConfiabilidade = issues
            .Where(EhIssueDeConfiabilidade)
            .ToList();

        var issuesSeguranca = issues
            .Where(EhIssueDeSeguranca)
            .ToList();

        var metricas = new MetricasAutorSonarDto
        {
            UsuarioId = usuarioId,
            Autor = emailAutor,
            TotalIssues = issues.Count,

            IssuesManutencao = issuesManutencao.Count,
            IssuesConfiabilidade = issuesConfiabilidade.Count,
            IssuesSeguranca = issuesSeguranca.Count,

            NivelManutencao = CalcularNivel(issuesManutencao),
            NivelConfiabilidade = CalcularNivel(issuesConfiabilidade),
            NivelSeguranca = CalcularNivel(issuesSeguranca),
        };

        metricas.NivelGeral = CalcularMediaGeral(
            metricas.NivelManutencao,
            metricas.NivelConfiabilidade,
            metricas.NivelSeguranca);

        foreach (var issue in issues)
        {
            var esforco = ConverterEsforcoParaMinutos(
                issue.EsforcoRemediacao);

            metricas.EsforcoTotalMinutos += esforco;
            AdicionarSeveridade(metricas, issue.Severidade);

            if (EhIssueDeManutencao(issue))
                metricas.EsforcoManutencaoMinutos += esforco;

            if (EhIssueDeConfiabilidade(issue))
                metricas.EsforcoConfiabilidadeMinutos += esforco;

            if (EhIssueDeSeguranca(issue))
                metricas.EsforcoSegurancaMinutos += esforco;
        }

        return metricas;
    }

    private static bool EhIssueDeManutencao(IssueSonarDto issue)
{
    return string.Equals(
        issue.Tipo,
        "CODE_SMELL",
        StringComparison.OrdinalIgnoreCase);
}

private static bool EhIssueDeConfiabilidade(IssueSonarDto issue)
{
    return string.Equals(
        issue.Tipo,
        "BUG",
        StringComparison.OrdinalIgnoreCase);
}

private static bool EhIssueDeSeguranca(IssueSonarDto issue)
{
    return string.Equals(
               issue.Tipo,
               "VULNERABILITY",
               StringComparison.OrdinalIgnoreCase)
           ||
           string.Equals(
               issue.Tipo,
               "SECURITY_HOTSPOT",
               StringComparison.OrdinalIgnoreCase);
}


private static decimal CalcularNivel(
    IReadOnlyCollection<IssueSonarDto> issues)
{
    if (issues.Count == 0)
        return 100m;

    var media = issues
        .Select(issue => PontuacaoDaIssue(issue.Severidade))
        .Average();

    return Math.Round(
        Math.Clamp((decimal)media, 0m, 100m),
        2,
        MidpointRounding.AwayFromZero);
}

private static decimal CalcularMediaGeral(
    decimal manutencao,
    decimal confiabilidade,
    decimal seguranca)
{
    return Math.Round(
        (manutencao + confiabilidade + seguranca) / 3m,
        2,
        MidpointRounding.AwayFromZero);
}

private static decimal PontuacaoDaIssue(string? severidade)
{
    var penalidade = severidade?.Trim().ToUpperInvariant() switch
    {
        "INFO" => 2m,
        "MINOR" => 10m,
        "MAJOR" => 25m,
        "CRITICAL" => 50m,
        "BLOCKER" => 80m,
        _ => 25m
    };

    return 100m - penalidade;
}

private static void AdicionarSeveridade(
    MetricasAutorSonarDto metricas,
    string? severidade)
{
    switch (severidade?.Trim().ToUpperInvariant())
    {
        case "INFO":
            metricas.IssuesInfo++;
            break;

        case "MINOR":
            metricas.IssuesBaixa++;
            break;

        case "MAJOR":
            metricas.IssuesMedia++;
            break;

        case "CRITICAL":
            metricas.IssuesAlta++;
            break;

        case "BLOCKER":
            metricas.IssuesBloqueadora++;
            break;
    }
}

private static int ConverterEsforcoParaMinutos(
    string? esforco)
{
    if (string.IsNullOrWhiteSpace(esforco))
        return 0;

    var valor = esforco.Trim().ToLowerInvariant();

    if (valor.EndsWith("min") &&
        decimal.TryParse(
            valor[..^3].Trim(),
            System.Globalization.NumberStyles.Any,
            System.Globalization.CultureInfo.InvariantCulture,
            out var minutos))
    {
        return (int)Math.Round(minutos);
    }

    if (valor.EndsWith("h") &&
        decimal.TryParse(
            valor[..^1].Trim(),
            System.Globalization.NumberStyles.Any,
            System.Globalization.CultureInfo.InvariantCulture,
            out var horas))
    {
        return (int)Math.Round(horas * 60m);
    }

    if (valor.EndsWith("d") &&
        decimal.TryParse(
            valor[..^1].Trim(),
            System.Globalization.NumberStyles.Any,
            System.Globalization.CultureInfo.InvariantCulture,
            out var dias))
    {
        // Um dia de trabalho do SonarQube equivale a 8 horas.
        return (int)Math.Round(dias * 8m * 60m);
    }

    return 0;
}
}
