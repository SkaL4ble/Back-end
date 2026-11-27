namespace API.DTOs.GameficacaoDto
{
    public class ListarTecnologiaGameficacaoDto
    {
        public int TecnologiaId { get; set; }

        public string NomeTecnologia { get; set; } = string.Empty;

        public decimal AfinidadeMedia { get; set; }

        public int ExperienciaTecnologia { get; set; }

        public int NivelTecnologia { get; set; }

        public int NumeroAtribuicoes { get; set; }
    }
}
