using API.Applications.Services;
using API.DTOs.TipoTicketDto;
using API.Exceptions;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TipoTicketController : ControllerBase
    {
        private readonly TipoTicketService _service;

        public TipoTicketController(TipoTicketService service)
        {
            _service = service;
        }

        /// <summary>
        /// Lista os tipos de ticket
        /// </summary>
        /// <returns>Todos os tipos de ticket cadastrados</returns>
        /// <response code="200">Retorna todos os tipos de ticket do banco</response>
        [HttpGet]
        public ActionResult<List<ListarTipoTicketDto>> Listar()
        {
            return Ok(_service.ListarTiposTicket());
        }

        /// <summary>
        /// Retorna um tipo de ticket por ID
        /// </summary>
        /// <param name="id">ID do tipo de ticket</param>
        /// <returns>O tipo de ticket encontrado</returns>
        /// <response code="200">Retorna o tipo de ticket</response>
        /// <response code="404">Tipo de ticket não encontrado</response>
        [HttpGet("{id}")]
        public ActionResult<ListarTipoTicketDto> ObterPorId(int id)
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
        /// Adiciona um tipo de ticket
        /// </summary>
        /// <param name="dto">Dados do novo tipo de ticket</param>
        /// <response code="201">Tipo de ticket criado com sucesso</response>
        /// <response code="400">Dados inválidos ou tipo de ticket já existente</response>
        [HttpPost]
        public ActionResult Adicionar(CriarTipoTicketDto dto)
        {
            try
            {
                _service.AdicionarTipoTicket(dto);
                return StatusCode(StatusCodes.Status201Created);
            }
            catch (DomainException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Atualiza um tipo de ticket
        /// </summary>
        /// <param name="id">ID do tipo de ticket a ser atualizado</param>
        /// <param name="dto">Novos dados do tipo de ticket</param>
        /// <response code="204">Tipo de ticket atualizado com sucesso</response>
        /// <response code="400">Dados inválidos, tipo de ticket não encontrado ou já existente</response>
        [HttpPut("{id}")]
        public ActionResult Atualizar(int id, CriarTipoTicketDto dto)
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

        /// <summary>
        /// Deleta um tipo de ticket
        /// </summary>
        /// <param name="id">ID do tipo de ticket a ser deletado</param>
        /// <response code="204">Tipo de ticket deletado com sucesso</response>
        /// <response code="400">Tipo de ticket não encontrado ou possui tickets vinculados</response>
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
                return BadRequest(ex.Message);
            }
        }
    }
}