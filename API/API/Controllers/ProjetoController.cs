using API.Applications.Services;
using API.DTOs.ProjetoDto;
using API.DTOs.StatusTicketDto;
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

        /// <summary>
        /// Retorna um projeto por ID
        /// </summary>
        /// <param name="id">ID do projeto</param>
        /// <returns>O projeto encontrado</returns>
        /// <response code="200">Retorna o projeto</response>
        /// <response code="404">Projeto não encontrado</response>
        [HttpGet("{id}")]
        public ActionResult<ListarProjetoDto> ObterPorId(Guid id)
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

        }
    }
