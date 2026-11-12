using API.DTOs.SonarQubeDto;
using API.DTOs.SonarQubeDto.CommitDto;
using API.Exceptions;
using System.Globalization;
using System.Net;
using System.Net.Http.Headers;
using System.Text.Json;

namespace API.Applications.Services;

// Cliente da API do GitHub. O token é do projeto e vai por requisição, nunca fica no HttpClient.
public class GitHubService
{
    private readonly HttpClient _http;

    public GitHubService(HttpClient http, IConfiguration config)
    {
        // Para GitHub Enterprise, defina GitHub:ApiUrl no .env (GitHub__ApiUrl)
        var urlApi = config["GitHub:ApiUrl"] ?? "https://api.github.com";

        _http = http;
        _http.BaseAddress = new Uri(urlApi.TrimEnd('/') + "/");
        _http.DefaultRequestHeaders.UserAgent.Add(new ProductInfoHeaderValue("skal4ble-api", "1.0"));
        _http.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/vnd.github+json"));
        _http.DefaultRequestHeaders.Add("X-GitHub-Api-Version", "2022-11-28");
    }

    // repositorio no formato "dono/repo"
    public async Task<CommitIssueDto> ObterCommitAsync(string repositorio, string revisao, string token)
    {
        var partes = repositorio.Split('/', 2);
        if (partes.Length != 2 || partes.Any(string.IsNullOrWhiteSpace))
            throw new DomainException("O repositório deve estar no formato dono/repositorio");

        var corpo = await GetAsync(
            $"repos/{Uri.EscapeDataString(partes[0])}/{Uri.EscapeDataString(partes[1])}/commits/{Uri.EscapeDataString(revisao)}",
            token);

        using var doc = JsonDocument.Parse(corpo);
        var raiz = doc.RootElement;
        var commit = raiz.GetProperty("commit");

        return new CommitIssueDto
        {
            Revisao = Texto(raiz, "sha"),
            Mensagem = Texto(commit, "message"),
            UrlCommit = Texto(raiz, "html_url"),
            Autor = LerPessoa(commit, "author", raiz, "author"),
            Committer = LerPessoa(commit, "committer", raiz, "committer")
        };
    }

    // ============================== HTTP ==============================

    private async Task<string> GetAsync(string caminho, string token)
    {
        using var requisicao = new HttpRequestMessage(HttpMethod.Get, caminho);
        requisicao.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

        try
        {
            using var resposta = await _http.SendAsync(requisicao);

            if (!resposta.IsSuccessStatusCode)
                throw new DomainException(TraduzirErro(resposta));

            return await resposta.Content.ReadAsStringAsync();
        }
        catch (HttpRequestException)
        {
            throw new DomainException("Não foi possível conectar ao GitHub");
        }
    }

    private static string TraduzirErro(HttpResponseMessage resposta)
    {
        switch (resposta.StatusCode)
        {
            case HttpStatusCode.Unauthorized:
                return "Token do GitHub inválido ou expirado";

            case HttpStatusCode.Forbidden:
                var semLimite = resposta.Headers.TryGetValues("x-ratelimit-remaining", out var valores)
                    && valores.FirstOrDefault() == "0";
                return semLimite
                    ? "Limite de requisições do GitHub atingido, tente novamente mais tarde"
                    : "O token do GitHub não tem permissão para ler este repositório";

            case HttpStatusCode.NotFound:
            case HttpStatusCode.UnprocessableEntity:
                return "Commit ou repositório não encontrado (confira o repositório e as permissões do token)";

            default:
                return $"Erro ao consultar o GitHub (status {(int)resposta.StatusCode})";
        }
    }

    // ============================== LEITURA DO JSON ==============================

    private static string Texto(JsonElement e, string propriedade) =>
        e.TryGetProperty(propriedade, out var valor) && valor.ValueKind == JsonValueKind.String
            ? valor.GetString() ?? string.Empty
            : string.Empty;

    // Dados do commit (nome, e-mail e data gravados no git) + conta do GitHub, quando existe.
    private static PessoaCommitDto LerPessoa(JsonElement commit, string campoCommit,
        JsonElement raiz, string campoConta)
    {
        var pessoa = new PessoaCommitDto();

        if (commit.TryGetProperty(campoCommit, out var dados) && dados.ValueKind == JsonValueKind.Object)
        {
            pessoa.Nome = Texto(dados, "name");
            pessoa.Email = Texto(dados, "email");

            if (DateTimeOffset.TryParse(Texto(dados, "date"), CultureInfo.InvariantCulture,
                    DateTimeStyles.None, out var data))
                pessoa.Data = data.UtcDateTime;
        }

        if (raiz.TryGetProperty(campoConta, out var conta) && conta.ValueKind == JsonValueKind.Object)
            pessoa.Login = Texto(conta, "login");

        return pessoa;
    }
}