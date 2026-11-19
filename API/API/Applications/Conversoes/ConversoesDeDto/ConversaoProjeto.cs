using API.Domains;
using API.DTOs.ProjetoDto;
using System.Globalization;

namespace API.Applications.Conversoes.ConversoesDeDto;

public static class ConversaoProjeto
{
    public static ListarProjetoDto ConverterParaDto(Projeto projeto)
    {
        return new ListarProjetoDto
        {
            ProjetoId = projeto.ProjetoId,
            Nome = projeto.NomeProjeto,
            Descricao = projeto.Descricao,
            LinkRepositorio = projeto.LinkRepositorio,
            AutorId = projeto.Autor,
            NomeAutor = projeto.AutorNavigation?.NomeUsuario ?? string.Empty,
            StatusId = projeto.StatusProjetoId,
            Status = projeto.StatusProjeto?.NomeStatusProjeto ?? string.Empty,
            TipoId = projeto.TipoProjetoId,
            NomeTipo = projeto.TipoProjeto?.NomeTipoProjeto ?? string.Empty,

            Prazo = projeto.Prazo.HasValue
                ? projeto.Prazo.Value.ToString(
                    "dd/MM/yyyy",
                    CultureInfo.InvariantCulture)
                : null,

            UsuariosId = projeto.Usuario?.Select(u => u.UsuarioId).ToList() ?? new List<Guid>(),
            QuantidadeColaboradores = projeto.Usuario?.Count ?? 0,

            DataCriacao = projeto.DataCriacao.HasValue
                ? projeto.DataCriacao.Value.ToString(
                    "dd/MM/yyyy",
                    CultureInfo.InvariantCulture)
                : null,
        };
    }

    public static Projeto ConverterParaDomain(CriarProjetoDto dto, Guid? projetoId = null)
    {
        return new Projeto
        {
            ProjetoId = projetoId ?? Guid.NewGuid(),
            NomeProjeto = dto.Nome!.Trim(),
            Descricao = string.IsNullOrWhiteSpace(dto.Descricao) ? null : dto.Descricao.Trim(),
            LinkRepositorio = dto.LinkRepositorio!.Trim(),
            Autor = dto.AutorId,
            StatusProjetoId = dto.StatusId,
            TipoProjetoId = dto.TipoId,
            Prazo = dto.Prazo,
            Ativo = true,
            DataCriacao = DateTime.UtcNow
        };
    }
}
