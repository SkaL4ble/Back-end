using API.Applications.Services;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[Route("api/gamificacao")]
public sealed class GameficacaoController
    : ControllerBase
{
    private readonly GameficacaoService _service;

    public GameficacaoController( GameficacaoService service)
    {
        _service = service;
    }

    [HttpGet("funcionarios/{usuarioId:guid}")]
    public async Task<IActionResult>
    ObterPerfil(Guid usuarioId, CancellationToken cancellationToken)
    {
        var perfil = await _service.ObterPerfilAsync(usuarioId, cancellationToken);

        return Ok(new{
            perfil.UsuarioId,
            perfil.ExperienciaTotal,
            perfil.Nivel,
            perfil.DataAtualizacao
        });
    }
}