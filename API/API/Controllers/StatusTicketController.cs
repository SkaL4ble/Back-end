using API.Applications.Services;
using API.DTOs.StatusTicketDto;
using API.Exceptions;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StatusTicketController : ControllerBase
    {
        private readonly StatusTicketService _service;

        public StatusTicketController(StatusTicketService service)
        {
            _service = service;
        }

        /// <summary>
        /// Lista os status de ticket
        /// </summary>
        /// <returns>Todos os status de ticket cadastrados</returns>
        /// <response code="200">Retorna todos os status de ticket do banco</response>
        [HttpGet]
        public ActionResult<List<ListarStatusTicketDto>> Listar()
        {
            return Ok(_service.ListarStatusTicket());
        }

        /// <summary>
        /// Retorna um status de ticket por ID
        /// </summary>
        /// <param name="id">ID do status de ticket</param>
        /// <returns>O status de ticket encontrado</returns>
        /// <response code="200">Retorna o status de ticket</response>
        /// <response code="404">Status de ticket não encontrado</response>
        [HttpGet("{id}")]
        public ActionResult<ListarStatusTicketDto> ObterPorId(int id)
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
        /// Adiciona um status de ticket
        /// </summary>
        /// <param name="dto">Dados do novo status de ticket</param>
        /// <response code="201">Status de ticket criado com sucesso</response>
        /// <response code="400">Dados inválidos ou status de ticket já existente</response>
        [HttpPost]
        public ActionResult Adicionar(CriarStatusTicketDto dto)
        {
            try
            {
                _service.AdicionarStatusTicket(dto);
                return StatusCode(StatusCodes.Status201Created);
            }
            catch (DomainException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Atualiza um status de ticket
        /// </summary>
        /// <param name="id">ID do status de ticket a ser atualizado</param>
        /// <param name="dto">Novos dados do status de ticket</param>
        /// <response code="204">Status de ticket atualizado com sucesso</response>
        /// <response code="400">Dados inválidos, status de ticket não encontrado ou já existente</response>
        [HttpPut("{id}")]
        public ActionResult Atualizar(int id, CriarStatusTicketDto dto)
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
        /// Deleta um status de ticket
        /// </summary>
        /// <param name="id">ID do status de ticket a ser deletado</param>
        /// <response code="204">Status de ticket deletado com sucesso</response>
        /// <response code="400">Status de ticket não encontrado ou possui tickets vinculados</response>
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