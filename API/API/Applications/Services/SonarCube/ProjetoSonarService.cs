using API.Applications.GerarYml;
using API.Applications.Services.SonarCube;
using API.Domains;
using API.DTOs.SonarQubeDto;
using API.DTOs.SonarQubeDto.CommitDto;
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
    private readonly ProjetoService _serviceProjeto;

    public ProjetoSonarService(IProjetoSonarRepository repository, SonarQubeService sonar,
        CriptografiaService criptografia, IConfiguration config, ProjetoService serviceProjeto)
    {
        _repository = repository;
        _sonar = sonar;
        _criptografia = criptografia;
        _urlBase = (config["SonarQube:UrlBase"] ?? string.Empty).TrimEnd('/');
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
        WorkflowId = p.WorkflowId,
        UrlProjeto = $"{_urlBase}/dashboard?id={Uri.EscapeDataString(p.ChaveSonar)}",
        DataCriacao = p.DataCriacao
    };

    // ============================== CRIACAO ==============================

    public IReadOnlyList<TipoWorkFlowDto> ListarTiposWorkflow() => ModeloWorkflowSonar.Tipos;

    private static string NormalizarTipoWorkflow(string? tipo)
    {
        var valor = string.IsNullOrWhiteSpace(tipo) ? "dotnet" : tipo.Trim().ToLowerInvariant();

        if (!ModeloWorkflowSonar.Tipos.Any(t => t.Valor == valor))
            throw new DomainException("Tipo de workflow inválido");

        return valor;
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

    public async Task<List<AtividadeSonarDto>> ListarAtividadesAsync(Guid id) =>
        await _sonar.ListarAtividadesAsync(BuscarOuFalharSonar(id).ChaveSonar);
}
