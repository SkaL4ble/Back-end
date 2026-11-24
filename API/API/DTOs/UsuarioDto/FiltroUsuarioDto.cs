namespace API.DTOs.UsuarioDto
{
    public class FiltroUsuarioDto
    {
        public DateTime? DataAdmissao { get; set; }
        public bool? Ativo { get; init; }

        public Guid? TipoUsuarioId { get; init; }
        public List<int>? CargoIds { get; init; }
        public List<int>? AtuacaoIds { get; init; }
        public List<Guid>? ProjetosIds { get; init; } = new();
        public List<int>? TecnologiasIds { get; init; } = new();
    }
}
