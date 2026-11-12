using API.DTOs.SonarQubeDto;
using System.Globalization;
using System.Text.Json;

namespace API.Applications.Conversoes.ConversoesDeDto
{
    public class ConverterSonarQube
    {
        public static string Texto(JsonElement e, string propriedade) =>
        e.TryGetProperty(propriedade, out var valor) ? valor.GetString() ?? string.Empty : string.Empty;

        public static DateTime? Data(JsonElement e, string propriedade) =>
            DateTimeOffset.TryParse(Texto(e, propriedade), CultureInfo.InvariantCulture,
                DateTimeStyles.None, out var data)
                ? data.UtcDateTime
                : null;
        public static IssueSonarDto ConverterIssueParaDto(JsonElement e)
        {
            return new IssueSonarDto
            {
                Chave = Texto(e, "key"),
                Autor = e.TryGetProperty("author", out var autor) ? autor.GetString() : null,
                Responsavel = Texto(e, "assignee"),
                Regra = Texto(e, "rule"),
                Severidade = Texto(e, "severity"),
                Tipo = Texto(e, "type"),
                Status = Texto(e, "status"),
                Mensagem = Texto(e, "message"),
                Componente = Texto(e, "component"),
                Linha = e.TryGetProperty("line", out var linha) ? linha.GetInt32() : null,
                DataCriacao = Data(e, "creationDate")
            };
        }
    }
}
