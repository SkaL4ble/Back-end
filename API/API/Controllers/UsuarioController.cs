using System;
using System.Collections.Generic;
using API.Applications.Services;
using API.DTOs.UsuarioDto;
using API.Exceptions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsuarioController : ControllerBase
    {
        private readonly UsuarioService _service;

        public UsuarioController(UsuarioService service)
        {
            _service = service;
        }

        [HttpGet]
        //[Authorize(Roles = "Tech Lead")]
        public ActionResult<List<LerUsuarioDto>> Listar([FromQuery] FiltroUsuarioDto filtro)
        {
            try
            {
                List<LerUsuarioDto> usuarios = _service.Listar(filtro);
                return Ok(usuarios);
            }
            catch (DomainException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("{id}")]
        [Authorize(Roles = "Tech Lead")]
        public ActionResult<LerUsuarioDto> BuscarPorId(Guid id)
        {
            try
            {
                LerUsuarioDto usuario = _service.BuscarPorId(id);
                return Ok(usuario);
            }
            catch (DomainException ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpPost]
        [Authorize(Roles = "Tech Lead")]
        public async Task<ActionResult> Cadastrar(CriarUsuarioDto dto)
        {
            try
            {
                await _service.Cadastrar(dto);
                return StatusCode(StatusCodes.Status201Created);
            }
            catch (DomainException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Tech Lead")]
        public ActionResult Atualizar(AtualizarUsuarioDto dto, Guid id)
        {
            try
            {
                _service.Atualizar(dto, id);
                return NoContent();
            }
            catch (DomainException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPatch("{id}/inativar")]
        [Authorize(Roles = "Tech Lead")]
        public ActionResult Inativar(Guid id)
        {
            try
            {
                _service.Inativar(id);
                return NoContent();
            }
            catch (DomainException ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}