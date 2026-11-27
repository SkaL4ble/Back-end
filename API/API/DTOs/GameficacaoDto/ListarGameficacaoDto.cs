using API.DTOs.GameficacaoDto;

namespace API.DTOs;

public class ListarGameficacaoDto
{
    public Guid UsuarioId { get; set; }

    public int ExperienciaTotal { get; set; }

    public int NivelGeral { get; set; }

    public DateTime DataAtualizacao { get; set; }

    public List<ListarTecnologiaGameficacaoDto> Tecnologias { get; set; } = new();

    public List<RecomendacaoTecnologiaDto> Recomendacoes { get; set; } = new();
}