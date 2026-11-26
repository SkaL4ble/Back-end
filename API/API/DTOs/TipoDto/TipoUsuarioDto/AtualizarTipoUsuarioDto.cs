namespace API.DTOs.TipoDto.TipoUsuarioDto
{
    public class AtualizarTipoUsuarioDto
    {
        public Guid TipoUsuarioId { get; set; }
        public string NomeTipoUsuario { get; set; } = null!;
    }
}
