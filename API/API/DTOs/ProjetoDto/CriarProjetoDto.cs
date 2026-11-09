namespace API.DTOs.ProjetoDto
{
    public class CriarProjetoDto
    {
        public string Nome { get; set; } = null!;
        public string Descricao { get; set; } = null!;
        public string LinkRepositorio { get; set; } = null!;
        public string Autor { get; set; } = null!;
        public int StatusId { get; set; }
        public int TipoId { get; set; }
        public DateTime Prazo { get; set; }
        public List<Guid> UsuariosId { get; set; } = new List<Guid>();
        public DateTime DataCriacao { get; set; } = DateTime.Now;
    }
}
