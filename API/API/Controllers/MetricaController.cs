using API.Applications.Services;
using API.Exceptions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MetricaController : ControllerBase
    {
        private readonly MetricaService _service;

        public MetricaController(
            MetricaService service)
        {
            _service = service;
        }

        [HttpPost("fechar-trimestre")]
        //[Authorize(Roles = "Tech Lead")]
        public async Task<IActionResult> FecharTrimestre(
            CancellationToken cancellationToken)
        {
            await _service.FecharTrimestreAsync(
                DateTime.UtcNow,
                cancellationToken);

            return NoContent();
        }

        [HttpGet("{usuarioId}")]
        public async Task<IActionResult>
        ObterMetricasPorFuncionario(Guid usuarioId, CancellationToken cancellationToken)
        {
            try
            {
                var metricas = await _service.ObterMetricasPorFuncionarioAsync(usuarioId, cancellationToken);
                return Ok(metricas);
            }
            catch (DomainException ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
