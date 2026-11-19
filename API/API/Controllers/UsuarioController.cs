using System;
using System.Collections.Generic;
using API.Applications.Services;
using API.DTOs.UsuarioDto;
using API.Exceptions;
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
        public ActionResult<List<LerUsuarioDto>> Listar()
        {
            List<LerUsuarioDto> usuarios = _service.Listar();
            return Ok(usuarios);
        }

        [HttpGet("{id}")]
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
        public ActionResult Cadastrar(CriarUsuarioDto dto)
        {
            try
            {
                _service.Cadastrar(dto);
                return StatusCode(StatusCodes.Status201Created);
            }
            catch (DomainException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("{id}")]
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

        [HttpPatch("{id}/senha")]
        public ActionResult AtribuirSenha(Guid id, AtribuirSenha dto)
        {
            try
            {
                _service.AtribuirSenha(dto, id);
                return NoContent();
            }
            catch (DomainException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPatch("{id}/inativar")]
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
