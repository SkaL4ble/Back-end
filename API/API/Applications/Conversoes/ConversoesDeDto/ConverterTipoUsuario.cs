using API.Domains;
using API.DTOs.TipoUsuarioDto;

namespace API.Applications.Conversoes.ConversoesDeDto
{
    public static class ConverterTipoUsuario
    {
        // 1. LEITURA (entidade -> DTO)
        public static LerTipoUsuarioDto ToDto(TipoUsuario entity)
        {
            if (entity == null) return null;

            return new LerTipoUsuarioDto
            {
                TipoUsuarioId = entity.TipoUsuarioId,
                NomeTipoUsuario = entity.NomeTipoUsuario
            };
        }

        // 1.1. LEITURA DE LISTA (entidades -> lista de DTOs)
        public static IEnumerable<LerTipoUsuarioDto> ToDtoList(IEnumerable<TipoUsuario> entities)
        {
            if (entities == null) return Enumerable.Empty<LerTipoUsuarioDto>();

            return entities.Select(ToDto).ToList();
        }

        // 2. CRIAÇÃO (DTO -> nova entidade)
        public static TipoUsuario ToEntity(CriarTipoUsuarioDto dto)
        {
            if (dto == null) return null;

            return new TipoUsuario
            {
                TipoUsuarioId = Guid.NewGuid(),
                NomeTipoUsuario = dto.NomeTipoUsuario
            };
        }

        // 3. ATUALIZAÇÃO (DTO -> entidade atualizada)
        public static void UpdateEntity(AtualizarTipoUsuarioDto dto, TipoUsuario entity)
        {
            if (dto == null || entity == null) return;

            entity.NomeTipoUsuario = dto.NomeTipoUsuario;
        }
    }
}