using API.Applications.Conversoes.ConversoesDeDto;
using API.Domains;
using API.DTOs.ProjetoDto;
using API.DTOs.StatusTicketDto;
using API.Exceptions;
using API.Interfaces;

namespace API.Applications.Services
{
    public class ProjetoService
    {
        private readonly IProjetoRepository _repository;

        public ProjetoService(IProjetoRepository repository)
        {
            _repository = repository;
        }

        //================================= VALIDAÇÕES ==========================================
        public static string NormalizarNome(string? nome)
        {
            if (string.IsNullOrWhiteSpace(nome))
                throw new DomainException("O nome do projeto é obrigatório");

            return nome.Trim();
        }

        public Projeto? BuscarOuFalhar(Guid id)
        {
            return _repository.ObterPorId(id)
                ?? throw new DomainException("Projeto não encontrado");
        }

        public bool ValidarProjeto(CriarProjetoDto projetoDto)
        {
            projetoDto.Nome = NormalizarNome(projetoDto.Nome);
            if (_repository.NomeProjetoExiste(projetoDto.Nome))
                throw new DomainException("Projeto já existe");
            else if (projetoDto.LinkRepositorio != null && _repository.LinkRepositorioExiste(projetoDto.LinkRepositorio))
                throw new DomainException("Link de repositório já existe");
            else if (projetoDto.AutorId != null && !_repository.UsuarioExiste(projetoDto.AutorId))
                throw new DomainException("Autor não encontrado");
            else if (projetoDto.StatusId != null && !_repository.StatusExiste(projetoDto.StatusId))
                throw new DomainException("Status não encontrado");
            else if (projetoDto.TipoId != null && !_repository.TipoExiste(projetoDto.TipoId))
                throw new DomainException("Tipo de projeto não encontrado");
            else if(_repository.NomeProjetoExiste(projetoDto.Nome))
                throw new DomainException("Projeto já existe");
            return true;
        }
        public bool ValidarFiltro(FiltroProjetoDto filtro){
            if (filtro.UsuarioId != null && !_repository.UsuarioExiste(filtro.UsuarioId))
                throw new DomainException("Autor não encontrado");
            else if (filtro.StatusId != null && !_repository.StatusExiste(filtro.StatusId))
                throw new DomainException("Status não encontrado");
            else if (filtro.TipoId != null && !_repository.TipoExiste(filtro.TipoId))
                throw new DomainException("Tipo de projeto não encontrado");
            return true;
        }

        //================================= CRUD ==========================================

        public ListarProjetoDto ObterPorId(Guid id)
        {
            Projeto? projeto = BuscarOuFalhar(id);
            return ConversaoProjeto.ConverterParaDto(projeto);
        }

        public void AdicionarProjeto(CriarProjetoDto projetoDto)
        {
            ValidarProjeto(projetoDto);
            _repository.CadastrarProjeto(ConversaoProjeto.ConverterParaDomain(projetoDto));
        }

        public void Atualizar(Guid id, CriarProjetoDto projetoDto)
        {
            Projeto? projetoBanco = BuscarOuFalhar(id);
            string novoNome = NormalizarNome(projetoDto.Nome);

            bool nomeMudou = !string.Equals(
                projetoBanco?.NomeProjeto, novoNome, StringComparison.OrdinalIgnoreCase);

            if (nomeMudou && _repository.NomeProjetoExiste(novoNome))
                throw new DomainException("Projeto já existe");

            ValidarProjeto(projetoDto);
            _repository.AtualizarProjeto(id, ConversaoProjeto.ConverterParaDomain(projetoDto));
        }

        public void Remover(Guid id)
        {
            Projeto? projeto = BuscarOuFalhar(id);
            _repository.DeletarProjeto(id);
        }
        //================================= LISTAR ==========================================
        public List<ListarProjetoDto> ListarProjetos(FiltroProjetoDto filtro)
        {
            ValidarFiltro(filtro);
            return _repository.ListarProjetos(filtro).Select(ConversaoProjeto.ConverterParaDto).ToList();
        }

        
    }
}
