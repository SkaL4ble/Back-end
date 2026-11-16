using API.Applications.Services;
using API.DTOs.StatusProjetoDto;
using API.Exceptions;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StatusProjetoController : ControllerBase
    {
        private readonly StatusProjetoService _service;

        public StatusProjetoController(StatusProjetoService service)
        {
            _service = service;
        }

        /// <summary>
        /// Lista os status de projeto
        /// </summary>
        /// <returns>Todos os status de projeto cadastrados</returns>
        /// <response code="200">Retorna todos os status de projeto do banco</response>
        [HttpGet]
        public ActionResult<List<ListarStatusProjetoDto>> Listar()
        {
            return Ok(_service.ListarStatusProjeto());
        }

        /// <summary>
        /// Retorna um status de projeto por ID
        /// </summary>
        /// <param name="id">ID do status de projeto</param>
        /// <returns>O status de projeto encontrado</returns>
        /// <response code="200">Retorna o status de projeto</response>
        /// <response code="404">Status de projeto não encontrado</response>
        [HttpGet("{id}")]
        public ActionResult<ListarStatusProjetoDto> ObterPorId(int id)
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
        /// Adiciona um status de projeto
        /// </summary>
        /// <param name="dto">Dados do novo status de projeto</param>
        /// <response code="201">Status de projeto criado com sucesso</response>
        /// <response code="400">Dados inválidos ou status de projeto já existente</response>
        [HttpPost]
        public ActionResult Adicionar(CriarStatusProjetoDto dto)
        {
            try
            {
                _service.AdicionarStatusProjeto(dto);
                return StatusCode(StatusCodes.Status201Created);
            }
            catch (DomainException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Atualiza um status de projeto
        /// </summary>
        /// <param name="id">ID do status de projeto a ser atualizado</param>
        /// <param name="dto">Novos dados do status de projeto</param>
        /// <response code="204">Status de projeto atualizado com sucesso</response>
        /// <response code="400">Dados inválidos, status de projeto não encontrado ou já existente</response>
        [HttpPut("{id}")]
        public ActionResult Atualizar(int id, CriarStatusProjetoDto dto)
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
        /// Deleta um status de projeto
        /// </summary>
        /// <param name="id">ID do status de projeto a ser deletado</param>
        /// <response code="204">Status de projeto deletado com sucesso</response>
        /// <response code="400">Status de projeto não encontrado ou possui projetos vinculados</response>
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