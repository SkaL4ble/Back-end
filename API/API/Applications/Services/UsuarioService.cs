using API.Applications.Constantes;
using API.Applications.Conversoes.ConversoesDeDto;
using API.Applications.Regras;
using API.Domains;
using API.DTOs.UsuarioDto;
using API.Exceptions;
using API.Interfaces;
using API.DTOs.AutenticacaoDto;
using API.Repositories;

namespace API.Applications.Services
{
    public class UsuarioService
    {
        private readonly IUsuarioRepository _repository;
        private readonly ITipoUsuarioRepository _tipoUsuarioRepository;
        private readonly AutenticacaoService _autenticacaoService;

        public UsuarioService(
    IUsuarioRepository repository,
    ITipoUsuarioRepository tipoUsuarioRepository,
    AutenticacaoService autenticacaoService)
        {
            _repository = repository;
            _tipoUsuarioRepository = tipoUsuarioRepository;
            _autenticacaoService = autenticacaoService;
        }

        private void ValidarAtuacao(List<int> atuacaoId)
        {
            foreach (int atuacao in atuacaoId.Distinct())
            {
                if (!_repository.AtuacaoExiste(atuacao))
                    throw new DomainException($"Atuacao não encontrado: {atuacao}");
            }
        }

        private void ValidarCargo(List<int> cargoIds)
        {
            foreach (int cargo in cargoIds.Distinct())
            {
                if (!_repository.CargoExiste(cargo))
                    throw new DomainException($"Cargo não encontrado: {cargo}");
            }
        }

        private void ValidarTipoUsuario(Guid tipoUsuarioId)
        {
            if (!_repository.TipoUsuarioExiste(tipoUsuarioId))
                throw new DomainException("Tipo de usuário não encontrado");
        }

        private void ValidarProjetos(List<Guid> projetosIds)
        {
            foreach (Guid projetoId in projetosIds.Distinct())
            {
                if (!_repository.ProjetosExiste(projetoId))
                    throw new DomainException($"Projeto não encontrado ou inativo: {projetoId}");
            }
        }

        private void ValidarTecnologias(List<int> tecnologiasIds)
        {
            foreach (int tecnologiaId in tecnologiasIds.Distinct())
            {
                if (!_repository.TecnologiaExiste(tecnologiaId))
                    throw new DomainException($"Tecnologia não encontrada: {tecnologiaId}");
            }
        }

        public List<LerUsuarioDto> Listar(FiltroUsuarioDto filtro)
        {
            if (filtro.AtuacaoIds.Count > 0)
            {
                ValidarAtuacao(filtro.AtuacaoIds);
            }

            if (filtro.CargoIds.Count > 0)
            {
                ValidarCargo(filtro.CargoIds);
            }

            if (filtro.ProjetosIds.Count > 0)
            {
                ValidarProjetos(filtro.ProjetosIds);
            }

            if (filtro.TipoUsuarioId.HasValue)
            {
                ValidarTipoUsuario(filtro.TipoUsuarioId.Value);
            }

            if (filtro.TecnologiasIds.Count > 0)
            {
                ValidarTecnologias(filtro.TecnologiasIds);
            }

            List<Usuario> usuarios = _repository.Listar(filtro);
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

        public async Task Cadastrar(CriarUsuarioDto dto)
        {
            ValidarCriacao(dto);

            if (_repository.EmailExistente(dto.EmailPessoal, Guid.Empty))
            {
                throw new DomainException(
                    "Já existe um usuário com este email.");
            }

            if (_repository.EmailGitExistente(dto.EmailsGit, Guid.Empty))
            {
                throw new DomainException(
                    "Já existe um usuário com este email GitHub.");
            }

            TipoUsuario colaborador =
                _tipoUsuarioRepository.BuscarPorCodigo(
                    CodigosTipoUsuario.Colaborador);

            if (colaborador == null)
            {
                throw new DomainException(
                    "O tipo de usuário Colaborador não está configurado.");
            }

            Usuario usuario = ConversaoUsuario.ToEntity(dto);
            usuario.TipoUsuarioId = colaborador.TipoUsuarioId;
            usuario.Senha = null; // Será atribuida pelo email enviado ao email pessoal do colaborador
            usuario.Ativo = true;

            _repository.Cadastrar(usuario);

            await _autenticacaoService.SolicitarRedefinicaoSenhaAsync(
                new SolicitarRedefinicaoSenhaDto
                {
                    Email = usuario.EmailPessoal
                });
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

        //public void AtribuirSenha(AtribuirSenha dto, Guid id)
        //{
        //    if (id == Guid.Empty)
        //    {
        //        throw new DomainException("Preencha o campo de ID do usuário.");
        //    }

        //    if (dto == null || dto.Senha == null || dto.Senha.Length == 0)
        //    {
        //        throw new DomainException("Informe uma senha válida.");
        //    }

        //    Usuario usuario = _repository.BuscarPorId(id);

        //    if (usuario == null)
        //    {
        //        throw new DomainException("Usuário não encontrado.");
        //    }

        //    ConversaoUsuario.UpdatePassword(dto, usuario);
        //    _repository.Atualizar(usuario, id);
        //}

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