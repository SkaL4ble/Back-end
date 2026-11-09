using API.Applications.Services;
using API.DTOs.AtuacaoDto;
using API.Exceptions;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AtuacaoController : ControllerBase
    {
        private readonly AtuacaoService _service;

        public AtuacaoController(AtuacaoService service)
        {
            _service = service;
        }

        /// <summary>
        /// Lista as atuacoes
        /// </summary>
        /// <returns>Os itens da atuacao</returns>
        /// <response code="200">Returna todas atuacoes do banco</response>
        [HttpGet]
        public ActionResult<List<ListarAtuacaoDto>> Listar()
        {
            return Ok(_service.ListarAtuacoes());
        }

        /// <summary>
        /// Retorna atuacao por id
        /// </summary>
        /// <returns>Retorna a atuacao</returns>
        /// <response code="200">Returna a atuacao por id</response>
        [HttpGet("{id}")]
        public ActionResult<ListarAtuacaoDto> ObterPorId(int id)
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
        /// Adiciona uma atuacao
        /// </summary>
        /// <response code="201">Returna a atuacao por id</response>
        [HttpPost]
        public ActionResult Adicionar(CriarAtuacaoDto dto)
        {
            try
            {
                _service.AdicionarAtuacao(dto);
                return StatusCode(StatusCodes.Status201Created);
            }
            catch (DomainException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Atualiza uma atuacao
        /// </summary>
        /// <response code="204">Atualizacao uma atuacao usando ID de referencia e mandando o corpo</response>
        [HttpPut("{id}")]
        public ActionResult Atualizar(int id, CriarAtuacaoDto dto)
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
        /// Deleta uma atuacao
        /// </summary>
        /// <response code="204">Deleta uma atuacao pelo ID</response>
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