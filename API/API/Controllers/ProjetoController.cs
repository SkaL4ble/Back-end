using API.Applications.Services;
using API.DTOs.ProjetoDto;
using API.Exceptions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

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

        //========================================= CRUD =========================================

        /// <summary>
        /// Retorna um projeto por ID
        /// </summary>
        /// <param name="id">ID do projeto</param>
        /// <returns>O projeto encontrado</returns>
        /// <response code="200">Retorna o projeto</response>
        /// <response code="404">Projeto não encontrado</response>
        [HttpGet("{id}")]
        [Authorize(Roles = "Tech Lead")]
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
        [Authorize(Roles = "Tech Lead")]
        public async Task<IActionResult> Adicionar([FromBody] CriarProjetoDto dto)
        {
            try
            {
                var autorId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
                await _service.AdicionarProjeto(dto, autorId);
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
        [Authorize(Roles = "Tech Lead")]
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
        [Authorize(Roles = "Tech Lead")]
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

        // ========================================= LISTAR =========================================

        /// <summary>
        /// Lista os projetos
        /// </summary>
        /// <returns>Todos os projetos cadastrados</returns>
        /// <response code="200">Retorna todos os projetos do banco</response>
        [HttpGet]
        [Authorize(Roles = "Tech Lead")]
        public ActionResult<List<ListarProjetoDto>> Listar(
            [FromQuery] FiltroProjetoDto filtro)
        {
            return Ok(_service.ListarProjetos(filtro));
        }

        /// <summary>
        /// Lista os projetos fora do prazo
        /// </summary>
        /// <returns>Projetos fora do prazo</returns>
        /// <response code="200">Retorna os projetos fora do prazo</response>
        [HttpGet("fora-do-prazo")]
        [Authorize(Roles = "Tech Lead")]
        public ActionResult<List<ListarProjetoDto>> ListarProjetosForaDoPrazo(
            [FromQuery] FiltroProjetoDto filtro)
        {
            return Ok(_service.ListarProjetosForaDoPrazo(filtro));
        }

        /// <summary>
        /// Lista os projetos dentro do prazo
        /// </summary>
        /// <returns>Projetos dentro do prazo</returns>
        /// <response code="200">Retorna os projetos dentro do prazo</response>
        [HttpGet("dentro-do-prazo")]
        [Authorize(Roles = "Tech Lead")]
        public ActionResult<List<ListarProjetoDto>> ListarProjetosDentroDoPrazo(
            [FromQuery] FiltroProjetoDto filtro)
        {
            return Ok(_service.ListarProjetosDentroDoPrazo(filtro));
        }

        // ========================================= FUNCOES =========================================

        [HttpGet("{id}/link-repositorio")]
        [Authorize(Roles = "Tech Lead")]
        public ActionResult<string?> ObterLinkRepositorio(Guid id)
        {
            return Ok(_service.ObterLinkRepositorioPorId(id));
        }

        [HttpPatch("{projetoId}/associar-usuario/{usuarioId}")]
        //[Authorize(Roles = "Tech Lead")]
        public ActionResult AssociarUsuario(Guid projetoId, Guid usuarioId)
        {
            try
            {
                _service.AssociarUsuarioAoProjeto(projetoId, usuarioId);
                return NoContent();
            }
            catch (DomainException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPatch("{projetoId}/desassociar-usuario/{usuarioId}")]
        [Authorize(Roles = "Tech Lead")]
        public ActionResult DesassociarUsuario(Guid projetoId, Guid usuarioId)
        {
            try
            {
                _service.DesassociarUsuarioDoProjeto(projetoId, usuarioId);
                return NoContent();
            }
            catch (DomainException ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}