using API.Applications.Services;
using API.DTOs.CargoDto;
using API.Exceptions;
using Microsoft.AspNetCore.Mvc;

[Route("api/[controller]")]
[ApiController]
public class CargoController : ControllerBase
{
    private readonly CargoService _service;

    public CargoController(CargoService service)
    {
        _service = service;
    }

    /// <summary>
    /// Lista os cargos
    /// </summary>
    /// <returns>Todos os cargos cadastrados</returns>
    /// <response code="200">Retorna todos os cargos do banco</response>
    [HttpGet]
    public ActionResult<List<ListarCargoDto>> Listar()
    {
        return Ok(_service.ListarCargo());
    }

    /// <summary>
    /// Retorna um cargo por ID
    /// </summary>
    /// <param name="id">ID do cargo</param>
    /// <returns>O cargo encontrado</returns>
    /// <response code="200">Retorna o cargo</response>
    /// <response code="404">Cargo não encontrado</response>
    [HttpGet("{id}")]
    public ActionResult<ListarCargoDto> ObterPorId(int id)
    {
        try
        {
            return Ok(_service.ObterPorId(id));
        }
        catch (DomainException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    /// <summary>
    /// Deleta um cargo
    /// </summary>
    /// <param name="id">ID do cargo a ser deletado</param>
    /// <response code="204">Cargo deletado com sucesso</response>
    /// <response code="400">Cargo não encontrado ou possui tickets vinculados</response>
    [HttpDelete("{id}")]
    public ActionResult Deletar(int id)
    {
        try
        {
            _service.Deletar(id);
            return NoContent();
        }
        catch (DomainException ex)
        {
            return NotFound(ex.Message);
        }
    }

    /// <summary>
    /// Adiciona um cargo
    /// </summary>
    /// <param name="dto">Dados do novo cargo</param>
    /// <response code="201">Cargo criado com sucesso</response>
    /// <response code="400">Dados inválidos ou cargo já existente</response>
    [HttpPost]
    public ActionResult Adicionar(CriarCargoDto dto)
    {
        try
        {
            _service.AdicionarCargo(dto);
            return StatusCode(StatusCodes.Status201Created);
        }
        catch (DomainException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    /// <summary>
    /// Atualiza um cargo
    /// </summary>
    /// <param name="id">ID do cargo a ser atualizado</param>
    /// <param name="dto">Novos dados do cargo</param>
    /// <response code="204">Cargo atualizado com sucesso</response>
    /// <response code="400">Dados inválidos, cargo não encontrado ou já existente</response>
    [HttpPut("{id}")]
    public ActionResult Atualizar(int id, CriarCargoDto dto)
    {
        try
        {
            _service.Atualizar(id, dto);
            return NoContent();
        }
        catch (DomainException ex)
        {
            return BadRequest(ex.Message);
        }
    }
}