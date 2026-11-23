namespace API.DTOs.ProjetoDto
{
    public class CriarProjetoDto
    {
        public Guid ProjetoId { get; set; }
        public string Nome { get; set; } = null!;
        public string? Descricao { get; set; } = null!;
        public string? LinkRepositorio { get; set; } = null!;
        public int WorkflowId { get; set; } = 1;
        public Guid AutorId { get; set; }
        public int StatusId { get; set; }
        public int TipoId { get; set; }
        public DateTime? Prazo { get; set; }
        public List<Guid> UsuariosId { get; set; } = new List<Guid>();
    }
}
