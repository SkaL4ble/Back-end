
using API.Applications.Services;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[Route("api/funcionarios-gameficacao")]
public class FuncionarioGameficacaoController : ControllerBase
{
    private readonly GameficacaoService _service;

    public FuncionarioGameficacaoController(GameficacaoService service)
    {
        _service = service;
    }

    [HttpGet("{usuarioId}")]
    public IActionResult ListarPorFuncionario(Guid usuarioId)
    {
        var resultado = _service.ListarPorFuncionario(usuarioId);

        return Ok(resultado);
    }
}
