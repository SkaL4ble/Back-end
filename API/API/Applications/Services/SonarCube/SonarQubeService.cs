using API.DTOs.SonarQubeDto;
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

    public SonarQubeService(HttpClient http, IConfiguration config)
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

    private static string Texto(JsonElement e, string propriedade) =>
        e.TryGetProperty(propriedade, out var valor) ? valor.GetString() ?? string.Empty : string.Empty;

    private static DateTime? Data(JsonElement e, string propriedade) =>
        DateTimeOffset.TryParse(Texto(e, propriedade), CultureInfo.InvariantCulture,
            DateTimeStyles.None, out var data)
            ? data.UtcDateTime
            : null;

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

    // Token escopado ao projeto (PROJECT_ANALYSIS_TOKEN): só serve para analisar esse projeto.
    // O SonarQube só mostra o valor nesta resposta, por isso quem chama deve guardá-lo.
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

    // ============================== REMOTO ==============================

    // Lista integrações configuradas e, quando não houver nenhuma,
    // retorna também o catálogo de plataformas suportadas pela API.
    public async Task<List<PlataformaSonarDto>> ListarPlataformasAsync()
    {
        using var doc = await GetAsync("api/alm_settings/list_definitions");
        var plataformas = new List<PlataformaSonarDto>();
        var tiposSuportados = new[] { "github", "azure", "bitbucket", "bitbucketcloud" };

        foreach (var tipo in doc.RootElement.EnumerateObject())
        {
            if (tipo.Name.Equals("gitlab", StringComparison.OrdinalIgnoreCase))
                continue;
            if (tipo.Value.ValueKind != JsonValueKind.Array)
                continue;

            foreach (var item in tipo.Value.EnumerateArray())
            {
                plataformas.Add(new PlataformaSonarDto
                {
                    Tipo = tipo.Name,
                    Chave = Texto(item, "key"),
                    Url = item.TryGetProperty("url", out var url) ? url.GetString() : null,
                    Configurada = true
                });
            }
        }

        foreach (var tipo in tiposSuportados)
        {
            if (plataformas.Any(p =>
                p.Tipo.Equals(tipo, StringComparison.OrdinalIgnoreCase)))
                continue;

            plataformas.Add(new PlataformaSonarDto
            {
                Tipo = tipo,
                Chave = string.Empty,
                Url = null,
                Configurada = false
            });
        }

        return plataformas;
    }

    public async Task<(string Chave, string Nome)> ImportarProjetoGithubAsync(
        string almSetting, string organizacao, string repositorio, string tokenGithub)
    {
        (await PostAsync("api/alm_integrations/set_pat", new()
        {
            ["almSetting"] = almSetting,
            ["pat"] = tokenGithub
        })).Dispose();

        using var doc = await PostAsync("api/alm_integrations/import_github_project", new()
        {
            ["almSetting"] = almSetting,
            ["organization"] = organizacao,
            ["repositoryKey"] = repositorio
        });

        var projeto = doc.RootElement.GetProperty("project");
        return (Texto(projeto, "key"), Texto(projeto, "name"));
    }

    // ============================== ISSUES ==============================

    public async Task<List<IssueSonarDto>> ListarIssuesAsync(string chave)
    {
        var resultado = new Dictionary<string, IssueSonarDto>();

        await BuscarJanelaAsync(chave,
            new DateTime(2000, 1, 1, 0, 0, 0, DateTimeKind.Utc),
            DateTime.UtcNow.AddMinutes(1),
            resultado);

        return resultado.Values.OrderBy(i => i.DataCriacao).ToList();
    }

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
                var issue = ConverterIssue(item);
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

    private static IssueSonarDto ConverterIssue(JsonElement e) => new()
    {
        Chave = Texto(e, "key"),
        Regra = Texto(e, "rule"),
        Severidade = Texto(e, "severity"),
        Tipo = Texto(e, "type"),
        Status = Texto(e, "status"),
        Mensagem = Texto(e, "message"),
        Componente = Texto(e, "component"),
        Linha = e.TryGetProperty("line", out var linha) ? linha.GetInt32() : null,
        DataCriacao = Data(e, "creationDate")
    };

    // ============================== ATIVIDADES (bônus) ==============================

    public async Task<List<AtividadeSonarDto>> ListarAtividadesAsync(string chave)
    {
        var atividades = new List<AtividadeSonarDto>();

        for (var pagina = 1; ; pagina++)
        {
            using var doc = await GetAsync(
                $"api/project_analyses/search?project={Uri.EscapeDataString(chave)}&ps={TamanhoPagina}&p={pagina}");

            foreach (var analise in doc.RootElement.GetProperty("analyses").EnumerateArray())
            {
                var eventos = analise.TryGetProperty("events", out var ev)
                    ? ev.EnumerateArray().Select(x => Texto(x, "name")).ToList()
                    : new List<string>();

                atividades.Add(new AtividadeSonarDto
                {
                    Chave = Texto(analise, "key"),
                    Data = Data(analise, "date"),
                    Eventos = eventos
                });
            }

            var total = doc.RootElement.GetProperty("paging").GetProperty("total").GetInt32();
            if (pagina * TamanhoPagina >= total)
                break;
        }

        return atividades;
    }
}
