using API.Applications.Services;
using API.DTOs.ProjetoDto;
using API.Exceptions;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProjetoController : ControllerBase
    {
        private readonly ProjetoService _service;

        public ProjetoController(ProjetoService service)
        {
            _service = service;
        }

        [HttpGet]
        public ActionResult<List<ListarProjetoDto>> Listar()
        {
            return Ok(_service.ListarProjetos());
        }

        [HttpGet("{id}")]
        public ActionResult<ListarProjetoDto> ObterPorId(Guid id)
        {
            try
            {
                return Ok(_service.ObterPorId(id));
            }
            catch (DomainException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

        [HttpPost]
        public ActionResult Adicionar(CriarProjetoDto dto)
        {
            try
            {
                var projetoCriado = _service.AdicionarProjeto(dto);

                return CreatedAtAction(nameof(ObterPorId), new { id = projetoCriado.Id }, projetoCriado);
            }
            catch (DomainException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPut("{id}")]
        public ActionResult Atualizar(Guid id, CriarProjetoDto dto)
        {
            try
            {
                _service.Atualizar(id, dto);
                return NoContent();
            }
            catch (DomainException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        public ActionResult Deletar(Guid id)
        {
            try
            {
                _service.Remover(id);
                return NoContent();
            }
            catch (DomainException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}