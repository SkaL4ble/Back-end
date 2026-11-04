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

    [HttpGet]
    public ActionResult<List<ListarCargoDto>> Listar()
    {
        return Ok(_service.ListarCargo());
    }

    [HttpGet("{id}")]
    public ActionResult<ListarCargoDto> ObterPorId(int id)
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