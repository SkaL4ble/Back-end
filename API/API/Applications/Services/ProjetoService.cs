using API.Applications.Conversoes.ConversoesDeDto;
<<<<<<< HEAD
using API.Applications.Validacoes;
=======
using API.Applications.Services.Validacoes;
>>>>>>> 0fb51031abafc356f7cdc6125a16b23b059a7752
using API.Domains;
using API.DTOs.ProjetoDto;
using API.Exceptions;
using API.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace API.Applications.Services;

public class ProjetoService
{
    private const int TamanhoMaximoLink = 150;

    private readonly IProjetoRepository _repository;
    private readonly IProjetoSonarRepository _sonarRepository;
    private readonly ProjetoSonarService _sonarService;
    private readonly ValidacoesGerais _validacoes;

    public ProjetoService(IProjetoRepository repository, IProjetoSonarRepository sonarRepository, ProjetoSonarService sonarService, ValidacoesGerais validacoes)
    {
        _repository = repository;
        _sonarRepository = sonarRepository;
        _sonarService = sonarService;
        _validacoes = validacoes;
    }

    // ================================ VALIDAÇÕES =================================


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
<<<<<<< HEAD
        dto.Nome = _validacoes.NormalizarNome(dto.Nome);
=======
        dto.Nome = ValidacoesGerais.ValidarTamanho(ValidacoesGerais.NormalizarNome(dto.Nome));
>>>>>>> 0fb51031abafc356f7cdc6125a16b23b059a7752
        dto.LinkRepositorio = NormalizarLink(dto.LinkRepositorio);

        _validacoes.ValidarUsuario(dto.AutorId);
        _validacoes.ValidarStatusProjeto(dto.StatusId);
        _validacoes.ValidarTipoProjeto(dto.TipoId);

        if (_repository.NomeProjetoExiste(dto.Nome, idIgnorado))
            throw new DomainException("Projeto já existe");

        if (_repository.LinkRepositorioExiste(dto.LinkRepositorio, idIgnorado))
            throw new DomainException("Link de repositório já existe");


        foreach (var usuarioId in dto.UsuariosId.Distinct())
        {
            if (usuarioId == Guid.Empty || !_repository.UsuarioExiste(usuarioId))
                throw new DomainException("Um dos usuários informados não foi encontrado ou está inativo");
        }
    }

    private void ValidarFiltro(FiltroProjetoDto filtro)
    {
        _validacoes.ValidarUsuario(filtro.UsuarioId);
        _validacoes.ValidarStatusProjeto(filtro.StatusId);
        _validacoes.ValidarTipoProjeto(filtro.TipoId);
    }

    // ================================ CRUD =================================
    public ListarProjetoDto ObterPorId(Guid id)
    {
        return ConversaoProjeto.ConverterParaDto(BuscarOuFalhar(id));
    }

    public async Task AdicionarProjeto(CriarProjetoDto dto, Guid autorId)
    {
        ValidarProjeto(dto);
        var projetoId = Guid.NewGuid();
        dto.ProjetoId = projetoId;

        _validacoes.ValidarUsuario(dto.AutorId);

        dto.AutorId = autorId;

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

    public List<ListarProjetoDto> ListarProjetosForaDoPrazo(FiltroProjetoDto filtro)
    {
        ValidarFiltro(filtro);
        return _repository.ListarProjetosForaDoPrazo(filtro)
            .Select(ConversaoProjeto.ConverterParaDto)
            .ToList();
    }

    public List<ListarProjetoDto> ListarProjetosDentroDoPrazo(FiltroProjetoDto filtro)
    {
        ValidarFiltro(filtro);
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
