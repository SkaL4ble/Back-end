using API.Applications.Constantes;
using API.Applications.Conversoes.ConversoesDeDto;
using API.Applications.Regras;
using API.Domains;
using API.DTOs.UsuarioDto;
using API.Exceptions;
using API.Interfaces;
using API.DTOs.AutenticacaoDto;
using API.Repositories;
using API.Applications.Validacoes;

namespace API.Applications.Services
{
    public class UsuarioService
    {
        private readonly IUsuarioRepository _repository;
        private readonly ITipoUsuarioRepository _tipoUsuarioRepository;
        private readonly AutenticacaoService _autenticacaoService;
        private readonly ValidacoesGerais _validacoes;

        public UsuarioService(
    IUsuarioRepository repository,
    ITipoUsuarioRepository tipoUsuarioRepository,
    AutenticacaoService autenticacaoService,
    ValidacoesGerais validacoes)
        {
            _repository = repository;
            _tipoUsuarioRepository = tipoUsuarioRepository;
            _autenticacaoService = autenticacaoService;
            _validacoes = validacoes;
        }

        private void ValidarAtuacao(List<int> atuacaoId)
        {
            foreach (int atuacao in atuacaoId.Distinct())
            {
                _validacoes.ValidarAtuacao(atuacao);
            }
        }

        private void ValidarCargo(List<int> cargoIds)
        {
            foreach (int cargo in cargoIds.Distinct())
            {
                _validacoes.ValidarCargo(cargo);
            }
        }

        private void ValidarProjetos(List<Guid> projetosIds)
        {
            foreach (Guid projetoId in projetosIds.Distinct())
            {
                _validacoes.ValidarProjeto(projetoId);
            }
        }

        private void ValidarTecnologias(List<int> tecnologiasIds)
        {
            foreach (int tecnologiaId in tecnologiasIds.Distinct())
            {
                _validacoes.ValidarTecnologia(tecnologiaId);
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

            if (filtro.TecnologiasIds.Count > 0)
            {
                ValidarTecnologias(filtro.TecnologiasIds);
            }

            _validacoes.ValidarTipoUsuario(filtro.TipoUsuarioId);

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

            _validacoes.ValidarUsuario(id);

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
            _validacoes.ValidarUsuario(id);

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