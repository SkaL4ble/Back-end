using API.Domains;
using API.DTOs.UsuarioDto;

namespace API.Applications.Conversoes.ConversoesDeDto
{
    public static class ConversaoUsuario
    {
        // LEITURA (entidade -> DTO)
        public static LerUsuarioDto ToDto(Usuario entity)
        {
            if (entity == null) return null;

            return new LerUsuarioDto
            {
                UsuarioId = entity.UsuarioId,
                NomeUsuario = entity.NomeUsuario,
                EmailPessoal = entity.EmailPessoal,
                EmailsGit = entity.UsuarioEmailGit?
                    .Select(usuarioEmailGit => usuarioEmailGit.EmailGit)
                    .ToList() ?? new List<string>(),
                TipoUsuario = entity.TipoUsuario?.NomeTipoUsuario,
                Cargo = entity.Cargo?.NomeCargo,
                Atuacao = entity.Atuacao?.NomeAtuacao,
                Imagem = entity.Imagem,
                Ativo = entity.Ativo,
                TipoUsuarioId = entity.TipoUsuarioId,
                CargoId = entity.CargoId,
                AtuacaoId = entity.AtuacaoId,
                ProjetosIds = entity.Projeto?
                    .Select(projeto => projeto.ProjetoId)
                    .ToList() ?? new List<Guid>(),
                TicketsIds = entity.Ticket?
                    .Select(ticket => ticket.TicketId)
                    .ToList() ?? new List<int>(),
                TecnologiasIds = entity.Tecnologia?
                    .Select(tecnologia => tecnologia.TecnologiaId)
                    .ToList() ?? new List<int>()
            };
        }

        // LEITURA DE LISTA (entidades -> lista de DTOs)
        public static IEnumerable<LerUsuarioDto> ToDtoList(IEnumerable<Usuario> entities)
        {
            if (entities == null) return Enumerable.Empty<LerUsuarioDto>();

            return entities.Select(ToDto).ToList();
        }

        // CRIAÇÃO (DTO -> nova entidade)
        public static Usuario ToEntity(CriarUsuarioDto dto)
        {
            if (dto == null) return null;

            var usuario = new Usuario
            {
                UsuarioId = Guid.NewGuid(),
                NomeUsuario = dto.NomeUsuario,
                EmailPessoal = dto.EmailPessoal,
                CargoId = dto.CargoId,
                AtuacaoId = dto.AtuacaoId,
                Imagem = dto.Imagem
            };

            usuario.UsuarioEmailGit.Add(new UsuarioEmailGit
            {
                UsuarioId = usuario.UsuarioId,
                EmailGit = dto.EmailsGit
            });

            return usuario;
        }

        // ATUALIZAÇÃO (DTO -> entidade existente)
        public static void UpdateEntity(AtualizarUsuarioDto dto, Usuario entity)
        {
            if (dto == null || entity == null) return;

            entity.NomeUsuario = dto.NomeUsuario;
            entity.EmailPessoal = dto.EmailPessoal;
            entity.CargoId = dto.CargoId;
            entity.AtuacaoId = dto.AtuacaoId;
            entity.Imagem = dto.Imagem;
        }

        // PATCH DE SENHA (DTO -> entidade existente)
        public static void UpdatePassword(AtribuirSenha dto, Usuario entity)
        {
            if (dto == null || entity == null) return;

            entity.Senha = dto.Senha;
        }
    }
}