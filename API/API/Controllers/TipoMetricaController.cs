using API.Applications.Services;
using API.Domains;
using API.DTOs.TipoMetricaDto;
using API.Exceptions;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TipoMetricaController : ControllerBase
    {
        private readonly TipoMetricaService _service;

        public TipoMetricaController(TipoMetricaService service)
        {
            _service = service;
        }

        /// <summary>
        /// Lista os tipos de metrica
        /// </summary>
        /// <returns>Todos os tipos de metricas cadastradas</returns>
        /// <response code="200">Retorna todos os tipos de metrica do banco</response>
        [HttpGet]
        public ActionResult<List<ListarTipoMetricaDto>> Listar()
        {
            return Ok(_service.Listar());
        }

        /// <summary>
        /// Retorna um tipo de metrica por ID
        /// </summary>
        /// <param name="id">ID do tipo de metrica</param>
        /// <returns>O tipo de metrica encontrado</returns>
        /// <response code="200">Retorna o tipo de metrica</response>
        /// <response code="404">Tipo de metrica não encontrado</response>
        [HttpGet("{id}")]
        public ActionResult<ListarTipoMetricaDto> ObterPorId(int id)
        {
            try
            {
                return Ok(_service.BuscarPorId(id));
            }
            catch (DomainException ex)
            {
                return NotFound(ex.Message);
            }
        }

        /// <summary>
        /// Adiciona um tipo de metrica
        /// </summary>
        /// <param name="dto">Dados do novo tipo de metrica</param>
        /// <response code="201">Tipo de metrica criado com sucesso</response>
        /// <response code="400">Dados inválidos ou tipo de metrica já existente</response>
        [HttpPost]
        public ActionResult Adicionar(CriarTipoMetricaDto dto)
        {
            try
            {
                _service.AdicionarTipoMetrica(dto);
                return StatusCode(StatusCodes.Status201Created);
            }
            catch (DomainException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Atualiza um tipo de metrica
        /// </summary>
        /// <param name="id">ID do tipo de metrica a ser atualizado</param>
        /// <param name="tipoMetrica">Novos dados do tipo de metrica</param>
        /// <response code="204">Tipo de metrica atualizado com sucesso</response>
        /// <response code="400">Dados inválidos, tipo de metrica não encontrado ou já existente</response>
        [HttpPut("{id}")]
        public ActionResult Atualizar(int id, TipoMetrica tipoMetrica)
        {
            try
            {
                _service.Atualizar(id, tipoMetrica);
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
        /// <param name="id">ID do tipo de metrica a ser deletado</param>
        /// <response code="204">Tipo de metrica deletado com sucesso</response>
        /// <response code="400">Tipo de metrica não encontrado ou possui metricas vinculadas</response>
        [HttpDelete("{id}")]
        public ActionResult Remover(int id)
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