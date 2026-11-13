using API.Applications.Conversoes.ConversoesDeDto;
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
                var issue = ConverterSonarQube.ConverterIssueParaDto(item);
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
                    ? ev.EnumerateArray().Select(x => ConverterSonarQube.Texto(x, "name")).ToList()
                    : new List<string>();

                atividades.Add(new AtividadeSonarDto
                {
                    Chave = ConverterSonarQube.Texto(analise, "key"),
                    Data = ConverterSonarQube.Data(analise, "date"),
                    Eventos = eventos
                });
            }

            var total = doc.RootElement.GetProperty("paging").GetProperty("total").GetInt32();
            if (pagina * TamanhoPagina >= total)
                break;
        }

        return atividades;
    }

    // Issue de um projeto pela chave. Só acha se pertencer ao projeto informado.
    public async Task<IssueSonarDto> ObterIssueAsync(string chaveProjeto, string chaveIssue)
    {
        using var doc = await GetAsync(
            $"api/issues/search?issues={Uri.EscapeDataString(chaveIssue)}" +
            $"&componentKeys={Uri.EscapeDataString(chaveProjeto)}&ps=1");

        var issues = doc.RootElement.GetProperty("issues");
        if (issues.GetArrayLength() == 0)
            throw new DomainException("Issue não encontrada neste projeto");

        return ConverterSonarQube.ConverterIssueParaDto(issues[0]);
    }

    // Passo 1: autor, data e hash do commit da linha (git blame do SonarQube).
    public async Task<ScmLinhaSonarDto?> ObterScmLinhaAsync(string componente, int linha)
    {
        using var doc = await GetAsync(
            $"api/sources/scm?key={Uri.EscapeDataString(componente)}&from={linha}&to={linha}&commits_by_line=true");

        var scm = doc.RootElement.GetProperty("scm");
        if (scm.GetArrayLength() == 0)
            return null;

        // Cada item é um array: [linha, autor, data, revisão]
        var item = scm[0];

        DateTime? data = null;
        if (item.GetArrayLength() > 2 &&
            DateTimeOffset.TryParse(item[2].GetString(), CultureInfo.InvariantCulture,
                DateTimeStyles.None, out var dt))
            data = dt.UtcDateTime;

        return new ScmLinhaSonarDto
        {
            Autor = item.GetArrayLength() > 1 ? item[1].GetString() : null,
            Data = data,
            Revisao = item.GetArrayLength() > 3 ? item[3].GetString() : null
        };
    }
}
