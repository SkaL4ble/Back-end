using API.Applications.Services;
using API.DTOs.TicketDto;
using API.Exceptions;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TicketController : ControllerBase
    {
        private readonly TicketService _service;

        public TicketController(TicketService service)
        {
            _service = service;
        }

        // ================================= CRUD =================================

        /// <summary>
        /// Retorna um ticket por ID
        /// </summary>
        /// <param name="id">ID do ticket</param>
        /// <returns>O ticket encontrado</returns>
        /// <response code="200">Retorna o ticket</response>
        /// <response code="404">Ticket não encontrado</response>
        [HttpGet("{id:int}")]
        public ActionResult<ListarTicketDto> ObterPorId(int id)
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
        /// Adiciona um ticket
        /// </summary>
        /// <param name="dto">Dados do novo ticket</param>
        /// <response code="201">Ticket criado com sucesso</response>
        /// <response code="400">Dados inválidos ou relacionamento não encontrado</response>
        [HttpPost]
        public ActionResult Adicionar(CriarTicketDto dto)
        {
            try
            {
                _service.AdicionarTicket(dto);
                return StatusCode(StatusCodes.Status201Created);
            }
            catch (DomainException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Atualiza um ticket
        /// </summary>
        /// <param name="id">ID do ticket a ser atualizado</param>
        /// <param name="dto">Novos dados do ticket</param>
        /// <response code="204">Ticket atualizado com sucesso</response>
        /// <response code="400">Dados inválidos, ticket não encontrado ou relacionamento não encontrado</response>
        [HttpPut("{id:int}")]
        public ActionResult Atualizar(int id, AtualizarTicketDto dto)
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
        /// Deleta um ticket
        /// </summary>
        /// <param name="id">ID do ticket a ser deletado</param>
        /// <response code="204">Ticket deletado com sucesso</response>
        /// <response code="404">Ticket não encontrado</response>
        [HttpDelete("{id:int}")]
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

        // ================================ FUNCOES =================================

        /// <summary>
        /// Muda o status de um ticket
        /// </summary>
        /// <param name="id">ID do ticket</param>
        /// <param name="statusTicketId">ID do novo status</param>
        /// <response code="204">Status do ticket alterado com sucesso</response>
        /// <response code="400">Ticket ou status não encontrado, ou ticket já está com este status</response>
        [HttpPatch("{id:int}/status/{statusTicketId:int}")]
        public ActionResult MudarEstadoTicket(int id, int statusTicketId)
        {
            try
            {
                _service.MudarEstadoTicket(id, statusTicketId);
                return NoContent();
            }
            catch (DomainException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Transfere a responsabilidade de um ticket para outro usuário
        /// </summary>
        /// <param name="id">ID do ticket</param>
        /// <param name="usuarioId">ID do novo usuário responsável</param>
        /// <response code="204">Responsável transferido com sucesso</response>
        /// <response code="400">Ticket ou usuário não encontrado, ou usuário já é o responsável</response>
        [HttpPatch("{id:int}/responsavel/{usuarioId:guid}")]
        public ActionResult TransferirResponsavel(int id, Guid usuarioId)
        {
            try
            {
                _service.TransferirResponsavel(id, usuarioId);
                return NoContent();
            }
            catch (DomainException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Adiciona um colaborador a um ticket
        /// </summary>
        /// <param name="id">ID do ticket</param>
        /// <param name="usuarioId">ID do usuário a ser adicionado como colaborador</param>
        /// <response code="204">Colaborador adicionado com sucesso</response>
        /// <response code="400">Ticket ou usuário não encontrado, ou usuário já é colaborador</response>
        [HttpPost("{id:int}/colaboradores/{usuarioId:guid}")]
        public ActionResult AdicionarColaborador(int id, Guid usuarioId)
        {
            try
            {
                _service.AdicionarColaborador(id, usuarioId);
                return NoContent();
            }
            catch (DomainException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Remove um colaborador de um ticket
        /// </summary>
        /// <param name="id">ID do ticket</param>
        /// <param name="usuarioId">ID do usuário a ser removido dos colaboradores</param>
        /// <response code="204">Colaborador removido com sucesso</response>
        /// <response code="400">Ticket não encontrado ou usuário não é colaborador</response>
        [HttpDelete("{id:int}/colaboradores/{usuarioId:guid}")]
        public ActionResult RemoverColaborador(int id, Guid usuarioId)
        {
            try
            {
                _service.RemoverColaborador(id, usuarioId);
                return NoContent();
            }
            catch (DomainException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // ================================ LISTAGEM =================================

        /// <summary>
        /// Lista os tickets
        /// </summary>
        /// <returns>Todos os tickets cadastrados</returns>
        /// <response code="200">Retorna todos os tickets do banco</response>
        [HttpGet]
        public ActionResult<List<ListarTicketDto>> Listar(
        [FromQuery] FiltroTicketDto filtro)
        {
            try
            {
                return Ok(_service.ListarTickets(filtro));
            }
            catch (DomainException ex)
            {
                return BadRequest(ex.Message);
            }
        }


        /// <summary>
        /// Lista os tickets criados em um período
        /// </summary>
        /// <param name="dataInicio">Data inicial do período (query string)</param>
        /// <param name="dataFim">Data final do período (query string)</param>
        /// <returns>Os tickets criados no período</returns>
        /// <response code="200">Retorna os tickets criados no período</response>
        /// <response code="400">Data de início maior que a data de fim</response>
        [HttpGet("Periodo")]
        public ActionResult<List<ListarTicketDto>> ListarPorPeriodo(
            [FromQuery] DateTime dataInicio, [FromQuery] DateTime dataFim)
        {
            try
            {
                return Ok(_service.ListarTicketsPorPeriodo(dataInicio, dataFim));
            }
            catch (DomainException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("ForaDoPrazo")]
        public ActionResult<List<ListarTicketDto>> ListarForaDoPrazo([FromQuery] FiltroTicketForaOuDentro filtro)
        {
            try
            {
                return Ok(_service.ListarTicketsForaDoPrazo(filtro));
            }
            catch (DomainException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("DentroDoPrazo")]
        public ActionResult<List<ListarTicketDto>> ListarDentroDoPrazo([FromQuery] FiltroTicketForaOuDentro filtro)
        {
            try
            {
                return Ok(_service.ListarTicketsDentroDoPrazo(filtro));
            }
            catch (DomainException ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}