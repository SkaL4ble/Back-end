using API.Applications.Services;
using API.DTOs.TipoUsuarioDto;
using API.Exceptions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TipoUsuarioController : ControllerBase
    {
        private readonly TipoUsuarioService _service;

        public TipoUsuarioController(TipoUsuarioService service)
        {
            _service = service;
        }

        [HttpGet]
        public ActionResult<List<LerTipoUsuarioDto>> Listar()
        {
            List<LerTipoUsuarioDto> tiposUsuario = _service.Listar();
            return Ok(tiposUsuario);
        }

        [HttpGet("{id}")]
        public ActionResult<LerTipoUsuarioDto> BuscarPorId(Guid id)
        {
            try
            {
                LerTipoUsuarioDto tipoUsuario = _service.BuscarPorId(id);
                return Ok(tipoUsuario);
            }
            catch (DomainException ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpPost]
        public ActionResult Cadastrar(CriarTipoUsuarioDto dto)
        {
            try
            {
                _service.Cadastrar(dto);
                return Created();
            }
            catch (DomainException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("{id}")]
        public ActionResult Atualizar(AtualizarTipoUsuarioDto dto, Guid id)
        {
            try
            {
                _service.Atualizar(dto, id);
                return NoContent();
            }
            catch (DomainException ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}