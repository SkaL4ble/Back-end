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
        /// Lista os projetos
        /// </summary>
        /// <returns>Todos os projetos cadastrados</returns>
        /// <response code="200">Retorna todos os projetos do banco</response>
        [HttpGet]
        public ActionResult<List<ListarProjetoDto>> Listar()
        {
            return Ok(_service.ListarProjetos());
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

        /// <summary>
        /// Adiciona um projeto
        /// </summary>
        /// <param name="dto">Dados do novo projeto</param>
        /// <response code="201">Projeto criado com sucesso</response>
        /// <response code="400">Dados inválidos ou projeto já existente</response>
        [HttpPost]
        public ActionResult Adicionar(CriarProjetoDto dto)
        {
            try
            {
                _service.AdicionarProjeto(dto);
                return StatusCode(StatusCodes.Status201Created);
            }
            catch (DomainException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Atualiza um projeto
        /// </summary>
        /// <param name="id">ID do projeto a ser atualizado</param>
        /// <param name="dto">Novos dados do projeto</param>
        /// <response code="204">Projeto atualizado com sucesso</response>
        /// <response code="400">Dados inválidos, projeto não encontrado ou já existente</response>
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
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Deleta um projeto
        /// </summary>
        /// <param name="id">ID do projeto a ser deletado</param>
        /// <response code="204">Projeto deletado com sucesso</response>
        /// <response code="400">Projeto não encontrado ou possui tickets vinculados</response>
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
                return BadRequest(ex.Message);
            }
        }
    }
}