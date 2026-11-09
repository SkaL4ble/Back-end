using API.Applications.Services;
using API.DTOs.TipoProjetoDto;
using API.Exceptions;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TipoProjetoController : ControllerBase
    {
        private readonly TipoProjetoService _service;

        public TipoProjetoController(TipoProjetoService service)
        {
            _service = service;
        }

        /// <summary>
        /// Lista os tipos de projeto
        /// </summary>
        /// <returns>Todos os tipos de projeto cadastrados</returns>
        /// <response code="200">Retorna todos os tipos de projeto do banco</response>
        [HttpGet]
        public ActionResult<List<ListarTipoProjetoDto>> Listar()
        {
            return Ok(_service.ListarTiposProjeto());
        }

        /// <summary>
        /// Retorna um tipo de projeto por ID
        /// </summary>
        /// <param name="id">ID do tipo de projeto</param>
        /// <returns>O tipo de projeto encontrado</returns>
        /// <response code="200">Retorna o tipo de projeto</response>
        /// <response code="404">Tipo de projeto não encontrado</response>
        [HttpGet("{id}")]
        public ActionResult<ListarTipoProjetoDto> ObterPorId(int id)
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
        /// Adiciona um tipo de projeto
        /// </summary>
        /// <param name="dto">Dados do novo tipo de projeto</param>
        /// <response code="201">Tipo de projeto criado com sucesso</response>
        /// <response code="400">Dados inválidos ou tipo de projeto já existente</response>
        [HttpPost]
        public ActionResult Adicionar(CriarTipoProjetoDto dto)
        {
            try
            {
                _service.AdicionarTipoProjeto(dto);
                return StatusCode(StatusCodes.Status201Created);
            }
            catch (DomainException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Atualiza um tipo de projeto
        /// </summary>
        /// <param name="id">ID do tipo de projeto a ser atualizado</param>
        /// <param name="dto">Novos dados do tipo de projeto</param>
        /// <response code="204">Tipo de projeto atualizado com sucesso</response>
        /// <response code="400">Dados inválidos, tipo de projeto não encontrado ou já existente</response>
        [HttpPut("{id}")]
        public ActionResult Atualizar(int id, CriarTipoProjetoDto dto)
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
        /// Deleta um tipo de projeto
        /// </summary>
        /// <param name="id">ID do tipo de projeto a ser deletado</param>
        /// <response code="204">Tipo de projeto deletado com sucesso</response>
        /// <response code="400">Tipo de projeto não encontrado ou possui projetos vinculados</response>
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