namespace API.DTOs.ProjetoDto
{
    public class ListarProjetoDto
    {
        public Guid ProjetoId { get; set; }
        public string Nome { get; set; } = null!;
        public string? Descricao { get; set; } = null!;
        public string LinkRepositorio { get; set; } = null!;
        public Guid AutorId { get; set; }
        public string NomeAutor { get; set; } = null!;
        public int StatusId { get; set; }
        public string Status { get; set; } = null!;
        public int TipoId { get; set; }
        public string NomeTipo { get; set; } = null!;
        public DateTime? Prazo { get; set; }
        public List<Guid> UsuariosId { get; set; } = new List<Guid>();
        public int QuantidadeColaboradores { get; set; }
        public DateTime? DataCriacao { get; set; } = DateTime.Now;
    }
}
