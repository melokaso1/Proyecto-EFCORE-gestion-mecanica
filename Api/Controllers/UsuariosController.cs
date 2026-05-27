using Api.Helpers;
using Api.Models;
using Application.Dtos;
using Application.Exceptions;
using Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

/// <summary>Operaciones de usuarios y autenticación.</summary>
[ApiController]
[Route("api/usuarios")]
public class UsuariosController(IUsuarioService usuarioService) : ControllerBase
{
    /// <summary>Autentica un usuario y retorna un token JWT.</summary>
    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<ActionResult<TokenResponseDto>> Login([FromBody] LoginDto dto)
    {
        try
        {
            var token = await usuarioService.LoginAsync(dto);
            return Ok(token);
        }
        catch (NotFoundException)
        {
            return Unauthorized(new { message = "Credenciales inválidas." });
        }
    }

    /// <summary>Lista usuarios paginados.</summary>
    [HttpGet]
    [Authorize(Policy = "AdminOnly")]
    public async Task<ActionResult<IEnumerable<UsuarioDto>>> Listar(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10)
    {
        var result = await usuarioService.ListarAsync(pageNumber, pageSize);
        PaginationHelper.AddPaginationHeader(Response, result.TotalCount, pageNumber, pageSize);
        return Ok(result.Items);
    }

    /// <summary>Registra un nuevo usuario.</summary>
    [HttpPost]
    [Authorize(Policy = "AdminOnly")]
    public async Task<ActionResult<UsuarioDto>> Crear([FromBody] CreateUsuarioDto dto)
    {
        var usuario = await usuarioService.CrearAsync(dto);
        return CreatedAtAction(nameof(Listar), new { id = usuario.IdUsuario }, usuario);
    }

    /// <summary>Asigna un rol a un usuario.</summary>
    [HttpPatch("{id:int}/rol")]
    [Authorize(Policy = "AdminOnly")]
    public async Task<IActionResult> AsignarRol(int id, [FromBody] AsignarRolRequest request)
    {
        try
        {
            await usuarioService.AsignarRolAsync(id, request.IdRol);
            return NoContent();
        }
        catch (NotFoundException)
        {
            return NotFound();
        }
    }

    /// <summary>Desactiva un usuario.</summary>
    [HttpPatch("{id:int}/desactivar")]
    [Authorize(Policy = "AdminOnly")]
    public async Task<IActionResult> Desactivar(int id)
    {
        try
        {
            await usuarioService.DesactivarAsync(id);
            return NoContent();
        }
        catch (NotFoundException)
        {
            return NotFound();
        }
    }
}
