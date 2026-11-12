namespace API.DTOs.UsuarioDto
{
    public class LerUsuarioDto
    {
        public Guid UsuarioId { get; init; }

        public string NomeUsuario { get; init; } = null!;

        public string EmailPessoal { get; init; } = null!;

        public List<string> EmailsGit { get; init; } = new();

        public string TipoUsuario { get; init; } = null!;

        public string Cargo { get; init; } = null!;

        public string Atuacao { get; init; } = null!;

        public byte[]? Imagem { get; init; }

        public bool Ativo { get; init; }

        public Guid TipoUsuarioId { get; init; }

        public int CargoId { get; init; }

        public int AtuacaoId { get; init; }

        public List<Guid> ProjetosIds { get; init; } = new();

        public List<int> TicketsIds { get; init; } = new();

        public List<int> TecnologiasIds { get; init; } = new();

    }
}
