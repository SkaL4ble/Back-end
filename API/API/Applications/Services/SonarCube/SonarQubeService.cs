using API.Applications.Conversoes.ConversoesDeDto;
using API.DTOs.SonarQubeDto.ListagemDto;
using API.Exceptions;
using System.Globalization;
using System.Net.Http.Headers;
using System.Text.Json;

namespace API.Applications.Services;

// Cliente da Web API do SonarQube. Não conhece o banco: só fala HTTP.
public class SonarQubeService
{
    private const int TamanhoPagina = 500;
    private const int LimiteBuscaSonar = 10_000;

    private readonly HttpClient _http;



    // Construtor que recebe um HttpClient configurado para se comunicar com a API do SonarQube
    public SonarQubeService(HttpClient http, IConfiguration config, UsuarioService usuarioService)
    {
        var urlBase = config["SonarQube:UrlBase"]
            ?? throw new InvalidOperationException("SonarQube:UrlBase não configurado");
        var token = config["SonarQube:Token"]
            ?? throw new InvalidOperationException("SonarQube:Token não configurado");

        _http = http;
        _http.BaseAddress = new Uri(urlBase.TrimEnd('/') + "/");
        _http.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", token);
    }

    // ============================== HTTP ==============================
    // Dictionary guarda pares de chaves
    private async Task<JsonDocument> PostAsync(string caminho, Dictionary<string, string> campos)
    {
        try
        {
            var resposta = await _http.PostAsync(caminho, new FormUrlEncodedContent(campos));
            return await LerRespostaAsync(resposta);
        }
        catch (HttpRequestException)
        {
            throw new DomainException("Não foi possível conectar ao SonarQube");
        }
    }

    private async Task<JsonDocument> GetAsync(string caminho)
    {
        try
        {
            var resposta = await _http.GetAsync(caminho);
            return await LerRespostaAsync(resposta);
        }
        catch (HttpRequestException)
        {
            throw new DomainException("Não foi possível conectar ao SonarQube");
        }
    }

    private static async Task<JsonDocument> LerRespostaAsync(HttpResponseMessage resposta)
    {
        var corpo = await resposta.Content.ReadAsStringAsync();

        if (!resposta.IsSuccessStatusCode)
            throw new DomainException($"SonarQube: {ExtrairErro(corpo)}");

        return JsonDocument.Parse(string.IsNullOrWhiteSpace(corpo) ? "{}" : corpo);
    }

    private static string ExtrairErro(string corpo)
    {
        try
        {
            using var doc = JsonDocument.Parse(corpo);
            return doc.RootElement.GetProperty("errors")[0].GetProperty("msg").GetString() ?? corpo;
        }
        catch
        {
            return string.IsNullOrWhiteSpace(corpo) ? "erro desconhecido" : corpo;
        }
    }


    // ============================== PROJETOS ==============================

    public async Task CriarProjetoAsync(string chave, string nome)
    {
        (await PostAsync("api/projects/create", new()
        {
            ["project"] = chave,
            ["name"] = nome
        })).Dispose();
    }

    public async Task ExcluirProjetoAsync(string chave)
    {
        (await PostAsync("api/projects/delete", new() { ["project"] = chave })).Dispose();
    }

    // Token escopado ao projeto (PROJECT_ANALYSIS_TOKEN): só serve para analisar esse projeto
    // O SonarQube só mostra o valor nesta resposta, por isso quem chama deve guardá-lo
    public async Task<string> GerarTokenAnaliseAsync(string chave)
    {
        var nome = $"skal4ble-{chave}";
        if (nome.Length > 100)
            nome = nome[..100];

        using var doc = await PostAsync("api/user_tokens/generate", new()
        {
            ["name"] = nome,
            ["type"] = "PROJECT_ANALYSIS_TOKEN",
            ["projectKey"] = chave
        });

        return doc.RootElement.GetProperty("token").GetString()!;
    }

    // ============================== ISSUES ==============================

    public async Task<List<IssueSonarDto>> ListarIssuesAsync(string chave)
    {
        var resultado = new Dictionary<string, IssueSonarDto>();

        await BuscarJanelaAsync(chave,
            new DateTime(2000, 1, 1, 0, 0, 0, DateTimeKind.Utc),
            DateTime.UtcNow.AddMinutes(1),
            resultado);

        var issues = resultado.Values
        .OrderBy(i => i.DataCriacao)
        .ToList();

        foreach (var issue in issues)
        {
            await EnriquecerIssueComDetalhesDaRegraAsync(issue);
        }

        return issues;

    }

    public async Task<List<IssueSonarDto>>
    ListarIssuesPorFuncionarioAsync(
        string chaveSonar,
        string emailFuncionario)
    {
        var resultado =
            new Dictionary<string, IssueSonarDto>();

        await BuscarJanelaAsync(
            chaveSonar,
            new DateTime(
                2000,
                1,
                1,
                0,
                0,
                0,
                DateTimeKind.Utc),
            DateTime.UtcNow.AddMinutes(1),
            resultado);

        return resultado.Values
            .Where(issue =>
                string.Equals(
                    issue.Autor,
                    emailFuncionario,
                    StringComparison.OrdinalIgnoreCase))
            .OrderBy(issue => issue.DataCriacao)
            .ToList();
    }


    // ============================== AUXILIARES ==============================

    // O SonarQube não devolve mais de 10.000 issues por busca. Se a janela passar disso,
    // ela é dividida ao meio até caber. O dicionário elimina duplicatas nas bordas.
    private async Task BuscarJanelaAsync(string chave, DateTime inicio, DateTime fim,
        Dictionary<string, IssueSonarDto> destino)
    {
        var total = await ContarIssuesAsync(chave, inicio, fim);
        if (total == 0)
            return;

        if (total > LimiteBuscaSonar && (fim - inicio) > TimeSpan.FromSeconds(2))
        {
            var meio = inicio + (fim - inicio) / 2;
            await BuscarJanelaAsync(chave, inicio, meio, destino);
            await BuscarJanelaAsync(chave, meio, fim, destino);
            return;
        }

        var paginas = (int)Math.Ceiling(Math.Min(total, LimiteBuscaSonar) / (double)TamanhoPagina);

        for (var pagina = 1; pagina <= paginas; pagina++)
        {
            using var doc = await GetAsync(MontarUrlIssues(chave, inicio, fim, pagina, TamanhoPagina));

            foreach (var item in doc.RootElement.GetProperty("issues").EnumerateArray())
            {
                var issue = ConversaoSonarQube.ConverterIssueParaDto(item);
                destino[issue.Chave] = issue;
            }
        }
    }

    private async Task<int> ContarIssuesAsync(string chave, DateTime inicio, DateTime fim)
    {
        using var doc = await GetAsync(MontarUrlIssues(chave, inicio, fim, 1, 1));
        return doc.RootElement.GetProperty("paging").GetProperty("total").GetInt32();
    }

    private static string MontarUrlIssues(string chave, DateTime inicio, DateTime fim, int pagina, int tamanho) =>
        $"api/issues/search?componentKeys={Uri.EscapeDataString(chave)}" +
        $"&createdAfter={Uri.EscapeDataString(FormatarData(inicio))}" +
        $"&createdBefore={Uri.EscapeDataString(FormatarData(fim))}" +
        $"&ps={tamanho}&p={pagina}";

    private static string FormatarData(DateTime data) =>
        data.ToUniversalTime().ToString("yyyy-MM-dd'T'HH:mm:ss", CultureInfo.InvariantCulture) + "+0000";

    private static string? ObterTexto(
    JsonElement elemento,
    string propriedade)
    {
        if (!elemento.TryGetProperty(propriedade, out var valor))
            return null;

        if (valor.ValueKind == JsonValueKind.Null ||
            valor.ValueKind == JsonValueKind.Undefined)
        {
            return null;
        }

        return valor.ValueKind switch
        {
            JsonValueKind.String => valor.GetString(),
            JsonValueKind.Number => valor.ToString(),
            JsonValueKind.True => "true",
            JsonValueKind.False => "false",
            JsonValueKind.Object => valor.ToString(),
            JsonValueKind.Array => valor.ToString(),
            _ => null
        };
    }

    private static string? ObterSecaoDescricao(
        JsonElement regra,
        string chave)
    {
        if (!regra.TryGetProperty(
                "descriptionSections",
                out var secoes))
        {
            return null;
        }

        if (secoes.ValueKind != JsonValueKind.Array)
            return null;

        foreach (var secao in secoes.EnumerateArray())
        {
            var chaveSecao = ObterTexto(secao, "key");

            if (string.Equals(
                    chaveSecao,
                    chave,
                    StringComparison.OrdinalIgnoreCase))
            {
                return ObterTexto(secao, "content");
            }
        }

        return null;
    }


    private async Task EnriquecerIssueComDetalhesDaRegraAsync(
    IssueSonarDto issue)
    {
        if (string.IsNullOrWhiteSpace(issue.Regra))
            return;

        var caminho =
            $"api/rules/show?key={Uri.EscapeDataString(issue.Regra)}";

        using var documento = await GetAsync(caminho);

        if (!documento.RootElement.TryGetProperty("rule", out var regra))
            return;


        issue.EsforcoRemediacao =
            ObterTexto(regra, "remediation")
            ?? ObterTexto(regra, "debtRemFnType");

        issue.ComoCorrigir =
            ObterSecaoDescricao(regra, "how_to_fix")
            ?? ObterSecaoDescricao(regra, "howToFix");
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

    private static bool EhIssueDeComplexidade(IssueSonarDto issue)
    {
        if (!EhIssueDeManutencao(issue))
            return false;

        var texto = string.Join(
            " ",
            issue.Regra,
            issue.Mensagem);

        return texto.Contains("complex", StringComparison.OrdinalIgnoreCase)
            || texto.Contains("cognitive", StringComparison.OrdinalIgnoreCase)
            || texto.Contains("cyclomatic", StringComparison.OrdinalIgnoreCase)
            || texto.Contains("complexidade", StringComparison.OrdinalIgnoreCase);
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
        decimal seguranca,
        decimal complexidade)
    {
        return Math.Round(
            (manutencao + confiabilidade + seguranca + complexidade) / 4m,
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
