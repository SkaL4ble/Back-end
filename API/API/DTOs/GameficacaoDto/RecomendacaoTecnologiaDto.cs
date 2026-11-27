namespace API.DTOs.GameficacaoDto
{
    public class RecomendacaoTecnologiaDto
    {
        public int TecnologiaId { get; set; }

        public string NomeTecnologia { get; set; } = string.Empty;

        public string TipoRecomendacao { get; set; } = string.Empty;

        public string Justificativa { get; set; } = string.Empty;

        public int NivelAtual { get; set; }

        public int NivelRecomendado { get; set; }

        public int QuantidadeTicketsRelacionados { get; set; }

        public int QuantidadeProjetosRelacionados { get; set; }

        public decimal Prioridade { get; set; }

        public List<TrilhaRecomendadaDto> Trilhas { get; set; } = new();
    }
}
