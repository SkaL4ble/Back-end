using API.Domains;
using API.DTOs.SonarQubeDto;
using API.DTOs.SonarQubeDto.WorkFlowDto;
using System.Globalization;
using System.Text.Json;

namespace API.Applications.Conversoes.ConversoesDeDto
{
    public class ConversaoSonarQube
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
        public static ListarWorkFlowDto ConverterWorkFlowParaDto(Workflow e)
        {
            return new ListarWorkFlowDto
            {
                WorkflowId = e.WorkflowId,
                Nome = e.NomeWorkflow
            };
        }
    }
}
