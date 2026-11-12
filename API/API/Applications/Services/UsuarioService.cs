using API.Applications.Constantes;
using API.Applications.Conversoes.ConversoesDeDto;
using API.Applications.Regras;
using API.Domains;
using API.DTOs.UsuarioDto;
using API.Exceptions;
using API.Interfaces;
using API.Repositories;

namespace API.Applications.Services
{
    public class UsuarioService
    {
        private readonly IUsuarioRepository _repository;
        private readonly ITipoUsuarioRepository _tipoUsuarioRepository;

        public UsuarioService(
    IUsuarioRepository repository,
    ITipoUsuarioRepository tipoUsuarioRepository)
        {
            _repository = repository;
            _tipoUsuarioRepository = tipoUsuarioRepository;
        }


        public List<LerUsuarioDto> Listar()
        {
            List<Usuario> usuarios = _repository.Listar();
            return ConversaoUsuario.ToDtoList(usuarios).ToList();
        }

        public LerUsuarioDto BuscarPorId(Guid id)
        {
            if (id == Guid.Empty)
            {
                throw new DomainException("ID inválido.");
            }

            Usuario usuario = _repository.BuscarPorId(id);

            if (usuario == null)
            {
                throw new DomainException("Usuário não encontrado.");
            }

            return ConversaoUsuario.ToDto(usuario);
        }

        public void Cadastrar(CriarUsuarioDto dto)
        {
            ValidarCriacao(dto);

            if (_repository.EmailExistente(dto.EmailPessoal, Guid.Empty))
            {
                throw new DomainException("Já existe um usuário com este email.");
            }

            if (_repository.EmailGitExistente(dto.EmailsGit, Guid.Empty))
            {
                throw new DomainException("Já existe um usuário com este email GitHub.");
            }

            TipoUsuario colaborador =
    _tipoUsuarioRepository.BuscarPorCodigo(CodigosTipoUsuario.Colaborador);

            if (colaborador == null)
            {
                throw new DomainException(
                    "O tipo de usuário Colaborador não está configurado.");
            }

            Usuario usuario = ConversaoUsuario.ToEntity(dto);
            usuario.TipoUsuarioId = colaborador.TipoUsuarioId;

            _repository.Cadastrar(usuario);

        }

        public void Atualizar(AtualizarUsuarioDto dto, Guid id)
        {
            if (id == Guid.Empty)
            {
                throw new DomainException("ID inválido.");
            }

            if (dto == null)
            {
                throw new DomainException("Informe os dados do usuário.");
            }

            if (!_repository.UsuarioExistente(id))
            {
                throw new DomainException("Usuário não encontrado.");
            }

            dto.NomeUsuario.Nome();
            dto.EmailPessoal.EmailPessoal();

            if (_repository.EmailExistente(dto.EmailPessoal, id))
            {
                throw new DomainException("Já existe um usuário com este email pessoal.");
            }

            Usuario usuario = _repository.BuscarPorId(id);
            ConversaoUsuario.UpdateEntity(dto, usuario);
            _repository.Atualizar(usuario, id);
        }

        public void AtribuirSenha(AtribuirSenha dto, Guid id)
        {
            if (id == Guid.Empty)
            {
                throw new DomainException("Preencha o campo de ID do usuário.");
            }

            if (dto == null || dto.Senha == null || dto.Senha.Length == 0)
            {
                throw new DomainException("Informe uma senha válida.");
            }

            Usuario usuario = _repository.BuscarPorId(id);

            if (usuario == null)
            {
                throw new DomainException("Usuário não encontrado.");
            }

            ConversaoUsuario.UpdatePassword(dto, usuario);
            _repository.Atualizar(usuario, id);
        }

        public void Inativar(Guid id)
        {
            if (id == Guid.Empty)
            {
                throw new DomainException("Preencha o campo de ID do usuário.");
            }

            if (!_repository.UsuarioExistente(id))
            {
                throw new DomainException("Usuário não encontrado.");
            }

            _repository.Inativar(id);
        }

        private static void ValidarCriacao(CriarUsuarioDto dto)
        {
            if (dto == null)
            {
                throw new DomainException("Informe os dados do usuário.");
            }

            dto.NomeUsuario.Nome();
            dto.EmailPessoal.EmailPessoal();
            dto.EmailsGit.EmailGit();

            if (dto.CargoId <= 0)
            {
                throw new DomainException("Selecione o cargo do usuário.");
            }

            if (dto.AtuacaoId <= 0)
            {
                throw new DomainException("Selecione a atuação do usuário.");
            }
        }
    }
}