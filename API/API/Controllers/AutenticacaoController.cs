using API.Applications.Services;
using API.DTOs.AutenticacaoDto;
using API.Exceptions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AutenticacaoController : ControllerBase
    {
        private readonly AutenticacaoService _service;

        public AutenticacaoController(AutenticacaoService service)
        {
            _service = service;
        }

        [HttpPost("login")]
        public async Task<ActionResult<TokenDto>> Login(LoginDto loginDto)
        {
            try
            {
                TokenDto token = await _service.Login(loginDto);
                return Ok(token);
            }
            catch (DomainException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("solicitar-redefinicao")]
        public async Task<IActionResult> SolicitarRedefinicaoSenha(
            SolicitarRedefinicaoSenhaDto dto)
        {
            await _service.SolicitarRedefinicaoSenhaAsync(dto);

            return Ok(
                "Se o e-mail estiver cadastrado, enviaremos as instruções.");
        }

        [HttpPost("redefinir-senha")]
        public async Task<IActionResult> RedefinirSenha(
            RedefinirSenhaDto dto)
        {
            bool resultado = await _service.RedefinirSenhaAsync(dto);

            if (!resultado)
                return BadRequest("Token inválido ou expirado.");

            return Ok("Senha definida com sucesso.");
        }
    }
}