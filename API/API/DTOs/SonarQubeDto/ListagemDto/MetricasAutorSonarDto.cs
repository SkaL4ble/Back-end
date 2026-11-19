namespace API.DTOs.SonarQubeDto.ListagemDto
{
    public class MetricasAutorSonarDto
    {
        public Guid UsuarioId { get; set; }
        public string Autor { get; set; } = string.Empty;

        public int TotalIssues { get; set; }

        // Pontuações de 0 a 100. Quanto maior, melhor.
        public decimal NivelManutencao { get; set; }
        public decimal NivelConfiabilidade { get; set; }
        public decimal NivelSeguranca { get; set; }
        public decimal NivelComplexidade { get; set; }
        public decimal NivelGeral { get; set; }

        public int IssuesManutencao { get; set; }
        public int IssuesConfiabilidade { get; set; }
        public int IssuesSeguranca { get; set; }
        public int IssuesComplexidade { get; set; }

        public int EsforcoManutencaoMinutos { get; set; }
        public int EsforcoConfiabilidadeMinutos { get; set; }
        public int EsforcoSegurancaMinutos { get; set; }
        public int EsforcoTotalMinutos { get; set; }

        public int IssuesInfo { get; set; }
        public int IssuesBaixa { get; set; }
        public int IssuesMedia { get; set; }
        public int IssuesAlta { get; set; }
        public int IssuesBloqueadora { get; set; }
    }
}
