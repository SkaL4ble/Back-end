using API.Applications.Conversoes.ConversoesDeDto.ConversoesTipo;
using API.Applications.Regras;
using API.Applications.Services.Validacoes;
using API.Domains;
using API.DTOs.TipoDto.TipoUsuarioDto;
using API.Exceptions;
using API.Interfaces;

namespace API.Applications.Services.Tipo
{
    public class TipoUsuarioService
    {
        private readonly ITipoUsuarioRepository _repository;
        private readonly ValidacoesGerais _validacao;

        public TipoUsuarioService(ITipoUsuarioRepository repository, ValidacoesGerais validacao)
        {
            _repository = repository;
            _validacao = validacao;
        }

        public List<LerTipoUsuarioDto> Listar()
        {
            List<TipoUsuario> tiposUsuario = _repository.Listar();
            return ConverterTipoUsuario.ToDtoList(tiposUsuario).ToList();
        }

        public LerTipoUsuarioDto BuscarPorId(Guid id)
        {
            if (id == null)
            {
                throw new DomainException("Preencha o campo de ID do usuário.");
            }

            TipoUsuario tipoUsuario = _repository.BuscarPorId(id);

            if (tipoUsuario == null)
            {
                throw new DomainException("Tipo de usuário não encontrado.");
            }

            return ConverterTipoUsuario.ToDto(tipoUsuario);
        }

        public void Cadastrar(CriarTipoUsuarioDto dto)
        {
            CampoObrigatorio.TipoUsuario(dto.NomeTipoUsuario);

            if (_repository.TipoUsuarioComMesmoNomeExistente(dto.NomeTipoUsuario, Guid.Empty))
            {
                throw new DomainException("Já existe um tipo de usuário com este nome.");
            }

            TipoUsuario tipoUsuario = ConverterTipoUsuario.ToEntity(dto);

            _repository.Cadastrar(tipoUsuario);
        }

        public void Atualizar(AtualizarTipoUsuarioDto dto, Guid id)
        {
            if (id == null)
            {
                throw new DomainException("Preencha o campo de ID do usuário.");
            }

            _validacao.ValidarTipoUsuario(dto.TipoUsuarioId);

            CampoObrigatorio.TipoUsuario(dto.NomeTipoUsuario);

            if (_repository.TipoUsuarioComMesmoNomeExistente(dto.NomeTipoUsuario, id))
            {
                throw new DomainException("Já existe um tipo de usuário com este nome.");
            }

            TipoUsuario tipoUsuario = _repository.BuscarPorId(id);
            ConverterTipoUsuario.UpdateEntity(dto, tipoUsuario);
            _repository.Atualizar(tipoUsuario, id);
        }
    }
}