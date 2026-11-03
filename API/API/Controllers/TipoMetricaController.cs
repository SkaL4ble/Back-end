using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using API.Applications.Services;
using API.DTOs.TipoMetricaDto;
using API.Domains;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TipoMetricaController : ControllerBase
    {
        private readonly TipoMetricaService _tipoMetricaService;
        public TipoMetricaController(TipoMetricaService tipoMetricaService)
        {
            _tipoMetricaService = tipoMetricaService;
        }
        [HttpGet]
        public IActionResult Listar()
        {
            var tiposMetricas = _tipoMetricaService.Listar();
            var tiposMetricasDto = tiposMetricas.Select(tm => new ListarTipoMetricaDto
            {
                TipoMetricaId = tm.TipoMetricaId,
                NomeMetrica = tm.NomeMetrica
            }).ToList();
            return Ok(tiposMetricasDto);
        }
        [HttpGet("{id}")]
        public IActionResult BuscarPorId(int id)
        {
            var tipoMetrica = _tipoMetricaService.BuscarPorId(id);
            if (tipoMetrica == null)
            {
                return NotFound();
            }
            var tipoMetricaDto = new ListarTipoMetricaDto
            {
                TipoMetricaId = tipoMetrica.TipoMetricaId,
                NomeMetrica = tipoMetrica.NomeMetrica
            };
            return Ok(tipoMetricaDto);
        }
        [HttpPost]
        public IActionResult Cadastrar(CriarTipoMetricaDto criarTipoMetricaDto)
        {
            var tipoMetrica = new TipoMetrica
            {
                NomeMetrica = criarTipoMetricaDto.NomeMetrica
            };
            _tipoMetricaService.Cadastrar(tipoMetrica);
            return CreatedAtAction(nameof(BuscarPorId), new { id = tipoMetrica.TipoMetricaId }, tipoMetrica);
        }
        [HttpPut("{id}")]
        public IActionResult Atualizar(int id, CriarTipoMetricaDto criarTipoMetricaDto)
        {
            var tipoMetricaExistente = _tipoMetricaService.BuscarPorId(id);
            if (tipoMetricaExistente == null)
            {
                return NotFound();
            }
            var tipoMetricaAtualizada = new TipoMetrica
            {
                TipoMetricaId = id,
                NomeMetrica = criarTipoMetricaDto.NomeMetrica
            };
            _tipoMetricaService.Atualizar(id, tipoMetricaAtualizada);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public IActionResult Remover(int id)
        {
            var tipoMetricaExistente = _tipoMetricaService.BuscarPorId(id);
            if (tipoMetricaExistente == null)
            {
                return NotFound();
            }
            _tipoMetricaService.Remover(id);
            return NoContent();
        }
    }
}