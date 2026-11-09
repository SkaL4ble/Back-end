using API.Applications.Services;
using API.DTOs.TecnologiaDto;
using API.Exceptions;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TecnologiaController : ControllerBase
    {
        private readonly TecnologiaService _service;

        public TecnologiaController(TecnologiaService service)
        {
            _service = service;
        }

        /// <summary>
        /// Lista as tecnologias
        /// </summary>
        /// <returns>Todas as tecnologias cadastradas</returns>
        /// <response code="200">Retorna todas as tecnologias do banco</response>
        [HttpGet]
        public ActionResult<List<ListarTecnologiaDto>> Listar()
        {
            return Ok(_service.ListarTecnologias());
        }

        /// <summary>
        /// Retorna uma tecnologia por ID
        /// </summary>
        /// <param name="id">ID da tecnologia</param>
        /// <returns>A tecnologia encontrada</returns>
        /// <response code="200">Retorna a tecnologia</response>
        /// <response code="404">Tecnologia não encontrada</response>
        [HttpGet("{id}")]
        public ActionResult<ListarTecnologiaDto> ObterPorId(int id)
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
        /// Adiciona uma tecnologia
        /// </summary>
        /// <param name="dto">Dados da nova tecnologia</param>
        /// <response code="201">Tecnologia criada com sucesso</response>
        /// <response code="400">Dados inválidos ou tecnologia já existente</response>
        [HttpPost]
        public ActionResult Adicionar(CriarTecnologiaDto dto)
        {
            try
            {
                _service.AdicionarTecnologia(dto);
                return StatusCode(StatusCodes.Status201Created);
            }
            catch (DomainException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Atualiza uma tecnologia
        /// </summary>
        /// <param name="id">ID da tecnologia a ser atualizada</param>
        /// <param name="dto">Novos dados da tecnologia</param>
        /// <response code="204">Tecnologia atualizada com sucesso</response>
        /// <response code="400">Dados inválidos, tecnologia não encontrada ou já existente</response>
        [HttpPut("{id}")]
        public ActionResult Atualizar(int id, CriarTecnologiaDto dto)
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
        /// Deleta uma tecnologia
        /// </summary>
        /// <param name="id">ID da tecnologia a ser deletada</param>
        /// <response code="204">Tecnologia deletada com sucesso</response>
        /// <response code="400">Tecnologia não encontrada ou possui registros vinculados</response>
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