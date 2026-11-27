namespace API.DTOs.GameficacaoDto
{
    public class TrilhaRecomendadaDto
    {
        public int TrilhaTreinamentoId { get; set; }

        public string NomeTrilha { get; set; } = string.Empty;

        public string? Descricao { get; set; }

        public int NivelInicial { get; set; }

        public int NivelFinal { get; set; }
    }
}
