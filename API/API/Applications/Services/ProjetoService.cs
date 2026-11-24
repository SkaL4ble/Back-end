using API.Applications.Conversoes.ConversoesDeDto;
using API.Domains;
using API.DTOs.ProjetoDto;
using API.Exceptions;
using API.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace API.Applications.Services;

public class ProjetoService
{
    private const int TamanhoMaximoNome = 50;
    private const int TamanhoMaximoLink = 150;

    private readonly IProjetoRepository _repository;
    private readonly IProjetoSonarRepository _sonarRepository;
    private readonly ProjetoSonarService _sonarService;

    public ProjetoService(IProjetoRepository repository, IProjetoSonarRepository sonarRepository, ProjetoSonarService sonarService)
    {
        _repository = repository;
        _sonarRepository = sonarRepository;
        _sonarService = sonarService;
    }

    // ================================ VALIDAÇÕES =================================
    public static string NormalizarNome(string? nome)
    {
        if (string.IsNullOrWhiteSpace(nome))
            throw new DomainException("O nome do projeto é obrigatório");

        var nomeNormalizado = nome.Trim();
        if (nomeNormalizado.Length > TamanhoMaximoNome)
            throw new DomainException($"O nome do projeto deve ter no máximo {TamanhoMaximoNome} caracteres");

        return nomeNormalizado;
    }

    private static string NormalizarLink(string? link)
    {
        if (string.IsNullOrWhiteSpace(link))
            throw new DomainException("O link do repositório é obrigatório");

        var linkNormalizado = link.Trim();
        if (linkNormalizado.Length > TamanhoMaximoLink)
            throw new DomainException($"O link do repositório deve ter no máximo {TamanhoMaximoLink} caracteres");

        return linkNormalizado;
    }

    public Projeto BuscarOuFalhar(Guid id)
    {
        if (id == Guid.Empty)
            throw new DomainException("Projeto não encontrado");

        return _repository.ObterPorId(id)
            ?? throw new DomainException("Projeto não encontrado");
    }

    private void ValidarProjeto(CriarProjetoDto dto, Guid? idIgnorado = null)
    {
        dto.Nome = NormalizarNome(dto.Nome);
        dto.LinkRepositorio = NormalizarLink(dto.LinkRepositorio);

        if (_repository.NomeProjetoExiste(dto.Nome, idIgnorado))
            throw new DomainException("Projeto já existe");

        if (_repository.LinkRepositorioExiste(dto.LinkRepositorio, idIgnorado))
            throw new DomainException("Link de repositório já existe");

        //if (dto.AutorId == Guid.Empty || !_repository.UsuarioExiste(dto.AutorId))
        //    throw new DomainException("Autor não encontrado ou inativo");

        if (!_repository.StatusExiste(dto.StatusId))
            throw new DomainException("Status não encontrado");

        if (!_repository.TipoExiste(dto.TipoId))
            throw new DomainException("Tipo de projeto não encontrado");

        foreach (var usuarioId in dto.UsuariosId.Distinct())
        {
            if (usuarioId == Guid.Empty || !_repository.UsuarioExiste(usuarioId))
                throw new DomainException("Um dos usuários informados não foi encontrado ou está inativo");
        }
    }

    private void ValidarFiltro(FiltroProjetoDto filtro)
    {
        if (filtro.UsuarioId.HasValue && !_repository.UsuarioExiste(filtro.UsuarioId))
            throw new DomainException("Autor não encontrado ou inativo");

        if (filtro.StatusId.HasValue && !_repository.StatusExiste(filtro.StatusId))
            throw new DomainException("Status não encontrado");

        if (filtro.TipoId.HasValue && !_repository.TipoExiste(filtro.TipoId))
            throw new DomainException("Tipo de projeto não encontrado");
    }

    private void ValidarFiltroPrazo(FiltroProjetoDentroOuForaPrazo filtro)
    {
        if (filtro.AutorId.HasValue && !_repository.UsuarioExiste(filtro.AutorId))
            throw new DomainException("Autor não encontrado ou inativo");

        if (filtro.StatusId.HasValue && !_repository.StatusExiste(filtro.StatusId))
            throw new DomainException("Status não encontrado");

        if (filtro.TipoId.HasValue && !_repository.TipoExiste(filtro.TipoId))
            throw new DomainException("Tipo de projeto não encontrado");
    }

    // ================================ CRUD =================================
    public ListarProjetoDto ObterPorId(Guid id)
    {
        return ConversaoProjeto.ConverterParaDto(BuscarOuFalhar(id));
    }

    public async Task AdicionarProjeto(CriarProjetoDto dto)
    {
        ValidarProjeto(dto);
        var projetoId = Guid.NewGuid();
        dto.ProjetoId = projetoId;

        //if (!_repository.UsuarioExiste(autorId))
        //    throw new DomainException("Usuário não encontrado ou inativo");
        //dto.AutorId = autorId;

        _repository.CadastrarProjeto(ConversaoProjeto.ConverterParaDomain(dto, projetoId));
        _repository.AtualizarColaboradores(projetoId, dto.UsuariosId);
        await _sonarService.CriarProjetoSonarJuntoAProjeto(ConversaoProjeto.ConverterParaDomain(dto,projetoId), dto.WorkflowId);

    }

    public void Atualizar(Guid id, CriarProjetoDto dto)
    {
        _ = BuscarOuFalhar(id);
        ValidarProjeto(dto, id);
        _repository.AtualizarProjeto(id, ConversaoProjeto.ConverterParaDomain(dto, id));
        _repository.AtualizarColaboradores(id, dto.UsuariosId);
    }

    public void Remover(Guid id)
    {
        _ = BuscarOuFalhar(id);
        _repository.DeletarProjeto(id);
    }

    // ================================ LISTAGEM =================================
    public List<ListarProjetoDto> ListarProjetos(FiltroProjetoDto filtro)
    {
        ValidarFiltro(filtro);
        return _repository.ListarProjetos(filtro)
            .Select(ConversaoProjeto.ConverterParaDto)
            .ToList();
    }

    public List<ListarProjetoDto> ListarProjetosForaDoPrazo(FiltroProjetoDentroOuForaPrazo filtro)
    {
        ValidarFiltroPrazo(filtro);
        return _repository.ListarProjetosForaDoPrazo(filtro)
            .Select(ConversaoProjeto.ConverterParaDto)
            .ToList();
    }

    public List<ListarProjetoDto> ListarProjetosDentroDoPrazo(FiltroProjetoDentroOuForaPrazo filtro)
    {
        ValidarFiltroPrazo(filtro);
        return _repository.ListarProjetosDentroDoPrazo(filtro)
            .Select(ConversaoProjeto.ConverterParaDto)
            .ToList();
    }

    // ================================ FUNCOES =================================
    public string ObterLinkRepositorioPorId(Guid id)
    {
        return BuscarOuFalhar(id).LinkRepositorio;
    }

    public void AssociarUsuarioAoProjeto(Guid projetoId, Guid usuarioId)
    {
        _ = BuscarOuFalhar(projetoId);

        if (!_repository.UsuarioExiste(usuarioId))
            throw new DomainException("Usuário não encontrado ou inativo");

        if (_repository.UsuarioEstaAssociadoAoProjeto(usuarioId, projetoId))
            throw new DomainException("Usuário já associado ao projeto");

        _repository.AssociarUsuarioAoProjeto(usuarioId, projetoId);
    }

    public void DesassociarUsuarioDoProjeto(Guid projetoId, Guid usuarioId)
    {
        _ = BuscarOuFalhar(projetoId);

        if (!_repository.UsuarioEstaAssociadoAoProjeto(usuarioId, projetoId))
            throw new DomainException("Usuário não associado ao projeto");

        _repository.DesassociarUsuarioDoProjeto(usuarioId, projetoId);
    }
}
