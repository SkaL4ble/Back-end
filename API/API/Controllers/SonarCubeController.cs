using API.Applications.Services;
using API.DTOs.SonarQubeDto;
using API.DTOs.SonarQubeDto.CommitDto;
using API.Exceptions;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class SonarQubeController : ControllerBase
{
    private readonly ProjetoSonarService _service;

    public SonarQubeController(ProjetoSonarService service)
    {
        _service = service;
    }

    /// <summary>
    /// Lista os tipos de workflow disponíveis para um projeto local.
    /// </summary>
    [HttpGet("workflows")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public IActionResult ListarWorkflows() => Ok(_service.ListarTiposWorkflow());

    /// <summary>
    /// Cria um projeto local no SonarQube e gera o token de análise, guardado de forma criptografada.
    /// </summary>
    /// <param name="dto">Nome, descrição, projeto de origem e token do GitHub.</param>
    /// <returns>O projeto criado, sem expor os tokens.</returns>
    /// <response code="201">Projeto criado com sucesso.</response>
    /// <response code="400">Dados inválidos ou erro retornado pelo SonarQube.</response>
    [HttpPost("projetos/local")]
    [ProducesResponseType(typeof(ListarProjetoSonarDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CriarProjeto([FromBody] CriarProjetoSonarDto dto)
    {
        try
        {
            var projeto = await _service.CriarProjetoAsync(dto);
            return CreatedAtAction(nameof(ObterPorId), new { id = projeto.ProjetoSonarId }, projeto);
        }
        catch (DomainException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    /// <summary>
    /// Busca um projeto do SonarQube cadastrado no sistema.
    /// </summary>
    /// <param name="id">Identificador do projeto do SonarQube.</param>
    /// <returns>Os dados do projeto, sem expor os tokens.</returns>
    /// <response code="200">Projeto encontrado.</response>
    /// <response code="404">Projeto não encontrado.</response>
    [HttpGet("projetos/{id:guid}")]
    [ProducesResponseType(typeof(ListarProjetoSonarDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult ObterPorId(Guid id)
    {
        try
        {
            return Ok(_service.ObterPorId(id));
        }
        catch (DomainException ex)
        {
            return NotFound(ex.Message);
        }
    }

    /// <summary>
    /// Devolve o token de análise do projeto, descriptografado.
    /// </summary>
    /// <param name="id">Identificador do projeto do SonarQube.</param>
    /// <returns>O token de análise.</returns>
    /// <response code="200">Token retornado com sucesso.</response>
    /// <response code="400">Não foi possível ler o token salvo.</response>
    /// <response code="404">Projeto não encontrado.</response>
    // Endpoint sensível: proteja com [Authorize] e restrinja a quem pode ver o token.
    [HttpGet("projetos/{id:guid}/token")]
    [ProducesResponseType(typeof(TokenSonarDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult ObterToken(Guid id)
    {
        try
        {
            return Ok(new TokenSonarDto { Token = _service.ObterTokenSonar(id) });
        }
        catch (DomainException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    /// <summary>
    /// Retorna os dados necessários para configurar a análise do projeto no GitHub Actions.
    /// O workflow usa a branch develop e o tipo padrão é dotnet.
    /// </summary>
    /// <param name="id">Identificador do projeto do SonarQube.</param>
    /// <response code="200">Configuração retornada com sucesso.</response>
    /// <response code="400">Tipo inválido ou erro ao ler o token.</response>
    /// <response code="404">Projeto não encontrado.</response>
    [HttpGet("projetos/{id:guid}/configuracao")]
    [ProducesResponseType(typeof(ConfiguracaoSonarQubeDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult ObterConfiguracao(Guid id)
    {
        try
        {
            return Ok(_service.ObterConfiguracao(id));
        }
        catch (DomainException ex) when (ex.Message.Contains("não encontrado", StringComparison.OrdinalIgnoreCase))
        {
            return NotFound(ex.Message);
        }
        catch (DomainException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    /// <summary>
    /// Lista todas as issues do projeto no SonarQube, paginando e fatiando por período quando passam de 10.000.
    /// </summary>
    /// <param name="id">Identificador do projeto do SonarQube.</param>
    /// <returns>Lista completa de issues do projeto.</returns>
    /// <response code="200">Issues listadas com sucesso.</response>
    /// <response code="400">Erro ao consultar o SonarQube.</response>
    /// <response code="404">Projeto não encontrado.</response>
    [HttpGet("projetos/{id:guid}/issues")]
    [ProducesResponseType(typeof(List<IssueSonarDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ListarIssues(Guid id)
    {
        try
        {
            return Ok(await _service.ListarIssuesAsync(id));
        }
        catch (DomainException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    /// <summary>
    /// Lista o histórico de análises (atividades) do projeto no SonarQube.
    /// </summary>
    /// <param name="id">Identificador do projeto do SonarQube.</param>
    /// <returns>Lista de análises com data e eventos.</returns>
    /// <response code="200">Atividades listadas com sucesso.</response>
    /// <response code="400">Erro ao consultar o SonarQube.</response>
    /// <response code="404">Projeto não encontrado.</response>
    [HttpGet("projetos/{id:guid}/atividades")]
    [ProducesResponseType(typeof(List<AtividadeSonarDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ListarAtividades(Guid id)
    {
        try
        {
            return Ok(await _service.ListarAtividadesAsync(id));
        }
        catch (DomainException ex)
        {
            return BadRequest(ex.Message);
        }
    }
}
