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
                return NotFound(ex.Message);
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
        public ActionResult<List<ListarTicketDto>> Listar()
        {
            return Ok(_service.ListarTickets());
        }

        /// <summary>
        /// Lista os tickets de um usuário (como responsável ou colaborador)
        /// </summary>
        /// <param name="usuarioId">ID do usuário</param>
        /// <returns>Os tickets do usuário</returns>
        /// <response code="200">Retorna os tickets do usuário</response>
        /// <response code="404">Usuário não encontrado</response>
        [HttpGet("usuario/{usuarioId:guid}")]
        public ActionResult<List<ListarTicketDto>> ListarPorUsuario(Guid usuarioId)
        {
            try
            {
                return Ok(_service.ListarTicketsPorUsuario(usuarioId));
            }
            catch (DomainException ex)
            {
                return NotFound(ex.Message);
            }
        }

        /// <summary>
        /// Lista os tickets de um projeto
        /// </summary>
        /// <param name="projetoId">ID do projeto</param>
        /// <returns>Os tickets do projeto</returns>
        /// <response code="200">Retorna os tickets do projeto</response>
        /// <response code="404">Projeto não encontrado</response>
        [HttpGet("projeto/{projetoId:guid}")]
        public ActionResult<List<ListarTicketDto>> ListarPorProjeto(Guid projetoId)
        {
            try
            {
                return Ok(_service.ListarTicketsPorProjeto(projetoId));
            }
            catch (DomainException ex)
            {
                return NotFound(ex.Message);
            }
        }

        /// <summary>
        /// Lista os tickets por status
        /// </summary>
        /// <param name="statusTicketId">ID do status de ticket</param>
        /// <returns>Os tickets com o status informado</returns>
        /// <response code="200">Retorna os tickets com o status informado</response>
        /// <response code="404">Status de ticket não encontrado</response>
        [HttpGet("status/{statusTicketId:int}")]
        public ActionResult<List<ListarTicketDto>> ListarPorStatus(int statusTicketId)
        {
            try
            {
                return Ok(_service.ListarTicketsPorStatus(statusTicketId));
            }
            catch (DomainException ex)
            {
                return NotFound(ex.Message);
            }
        }

        /// <summary>
        /// Lista os tickets por tipo
        /// </summary>
        /// <param name="tipoTicketId">ID do tipo de ticket</param>
        /// <returns>Os tickets do tipo informado</returns>
        /// <response code="200">Retorna os tickets do tipo informado</response>
        /// <response code="404">Tipo de ticket não encontrado</response>
        [HttpGet("tipo/{tipoTicketId:int}")]
        public ActionResult<List<ListarTicketDto>> ListarPorTipo(int tipoTicketId)
        {
            try
            {
                return Ok(_service.ListarTicketsPorTipo(tipoTicketId));
            }
            catch (DomainException ex)
            {
                return NotFound(ex.Message);
            }
        }

        /// <summary>
        /// Lista os tickets de um usuário em um projeto
        /// </summary>
        /// <param name="usuarioId">ID do usuário</param>
        /// <param name="projetoId">ID do projeto</param>
        /// <returns>Os tickets do usuário no projeto</returns>
        /// <response code="200">Retorna os tickets do usuário no projeto</response>
        /// <response code="404">Usuário ou projeto não encontrado</response>
        [HttpGet("usuario/{usuarioId:guid}/projeto/{projetoId:guid}")]
        public ActionResult<List<ListarTicketDto>> ListarPorUsuarioEProjeto(Guid usuarioId, Guid projetoId)
        {
            try
            {
                return Ok(_service.ListarTicketsPorUsuarioEProjeto(usuarioId, projetoId));
            }
            catch (DomainException ex)
            {
                return NotFound(ex.Message);
            }
        }

        /// <summary>
        /// Lista os tickets por prioridade
        /// </summary>
        /// <param name="prioridade">Valor da prioridade</param>
        /// <returns>Os tickets com a prioridade informada</returns>
        /// <response code="200">Retorna os tickets com a prioridade informada</response>
        [HttpGet("prioridade/{prioridade:int}")]
        public ActionResult<List<ListarTicketDto>> ListarPorPrioridade(int prioridade)
        {
            return Ok(_service.ListarTicketsPorPrioridade(prioridade));
        }

        /// <summary>
        /// Lista os tickets criados em um período
        /// </summary>
        /// <param name="dataInicio">Data inicial do período (query string)</param>
        /// <param name="dataFim">Data final do período (query string)</param>
        /// <returns>Os tickets criados no período</returns>
        /// <response code="200">Retorna os tickets criados no período</response>
        /// <response code="400">Data de início maior que a data de fim</response>
        [HttpGet("periodo")]
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

        // ================================ PRAZO =================================

        /// <summary>
        /// Lista os tickets fora do prazo
        /// </summary>
        /// <returns>Os tickets fora do prazo</returns>
        /// <response code="200">Retorna os tickets fora do prazo</response>
        [HttpGet("prazo/fora")]
        public ActionResult<List<ListarTicketDto>> ListarForaDoPrazo()
        {
            return Ok(_service.ListarTicketPorDataForaDoPrazo());
        }

        /// <summary>
        /// Lista os tickets fora do prazo de um usuário
        /// </summary>
        /// <param name="usuarioId">ID do usuário</param>
        /// <returns>Os tickets fora do prazo do usuário</returns>
        /// <response code="200">Retorna os tickets fora do prazo do usuário</response>
        /// <response code="404">Usuário não encontrado</response>
        [HttpGet("prazo/fora/usuario/{usuarioId:guid}")]
        public ActionResult<List<ListarTicketDto>> ListarForaDoPrazoPorUsuario(Guid usuarioId)
        {
            try
            {
                return Ok(_service.ListarTicketPorDataForaDoPrazoPorUsuario(usuarioId));
            }
            catch (DomainException ex)
            {
                return NotFound(ex.Message);
            }
        }

        /// <summary>
        /// Lista os tickets fora do prazo de um projeto
        /// </summary>
        /// <param name="projetoId">ID do projeto</param>
        /// <returns>Os tickets fora do prazo do projeto</returns>
        /// <response code="200">Retorna os tickets fora do prazo do projeto</response>
        /// <response code="404">Projeto não encontrado</response>
        [HttpGet("prazo/fora/projeto/{projetoId:guid}")]
        public ActionResult<List<ListarTicketDto>> ListarForaDoPrazoPorProjeto(Guid projetoId)
        {
            try
            {
                return Ok(_service.ListarTicketPorDataForaDoPrazoPorProjeto(projetoId));
            }
            catch (DomainException ex)
            {
                return NotFound(ex.Message);
            }
        }

        /// <summary>
        /// Lista os tickets dentro do prazo
        /// </summary>
        /// <returns>Os tickets dentro do prazo</returns>
        /// <response code="200">Retorna os tickets dentro do prazo</response>
        [HttpGet("prazo/dentro")]
        public ActionResult<List<ListarTicketDto>> ListarDentroDoPrazo()
        {
            return Ok(_service.ListarTicketPorDataDentroDoPrazo());
        }

        /// <summary>
        /// Lista os tickets dentro do prazo de um usuário
        /// </summary>
        /// <param name="usuarioId">ID do usuário</param>
        /// <returns>Os tickets dentro do prazo do usuário</returns>
        /// <response code="200">Retorna os tickets dentro do prazo do usuário</response>
        /// <response code="404">Usuário não encontrado</response>
        [HttpGet("prazo/dentro/usuario/{usuarioId:guid}")]
        public ActionResult<List<ListarTicketDto>> ListarDentroDoPrazoPorUsuario(Guid usuarioId)
        {
            try
            {
                return Ok(_service.ListarTicketPorDataDentroDoPrazoPorUsuario(usuarioId));
            }
            catch (DomainException ex)
            {
                return NotFound(ex.Message);
            }
        }

        /// <summary>
        /// Lista os tickets dentro do prazo de um projeto
        /// </summary>
        /// <param name="projetoId">ID do projeto</param>
        /// <returns>Os tickets dentro do prazo do projeto</returns>
        /// <response code="200">Retorna os tickets dentro do prazo do projeto</response>
        /// <response code="404">Projeto não encontrado</response>
        [HttpGet("prazo/dentro/projeto/{projetoId:guid}")]
        public ActionResult<List<ListarTicketDto>> ListarDentroDoPrazoPorProjeto(Guid projetoId)
        {
            try
            {
                return Ok(_service.ListarTicketPorDataDentroDoPrazoPorProjeto(projetoId));
            }
            catch (DomainException ex)
            {
                return NotFound(ex.Message);
            }
        }
    }
}