namespace API.DTOs.ProjetoDto
{
    public class ListarProjetoDto
    {
        public Guid Id { get; set; }
        public string? Nome { get; set; }
        public int Status { get; set; }
        public DateTime Prazo { get; set; }
    }
}
