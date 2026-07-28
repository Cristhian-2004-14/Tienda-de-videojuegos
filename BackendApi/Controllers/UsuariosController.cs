using BackendApi.Data;
using BackendApi.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace BackendApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsuariosController(
    IFirestoreRepository<Usuario> repositorio,
    IPasswordHasher<Usuario> passwordHasher) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IEnumerable<object>>> ObtenerUsuarios()
    {
        var usuarios = await repositorio.ObtenerTodosAsync();
        return Ok(usuarios.Select(SinPassword));
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<object>> ObtenerUsuario(int id)
    {
        var usuario = await repositorio.ObtenerPorIdAsync(id);
        return usuario is null ? NotFound() : Ok(SinPassword(usuario));
    }

    [HttpPost]
    public async Task<ActionResult<object>> RegistrarUsuario(Usuario usuario)
    {
        usuario.Password = passwordHasher.HashPassword(usuario, usuario.Password);
        var creado = await repositorio.CrearAsync(usuario);
        return CreatedAtAction(nameof(ObtenerUsuario), new { id = creado.Id }, SinPassword(creado));
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult<object>> ActualizarUsuario(int id, Usuario usuario)
    {
        var existente = await repositorio.ObtenerPorIdAsync(id);
        if (existente is null) return NotFound();
        usuario.Password = string.IsNullOrWhiteSpace(usuario.Password)
            ? existente.Password
            : passwordHasher.HashPassword(usuario, usuario.Password);
        var actualizado = await repositorio.ActualizarAsync(id, usuario);
        return Ok(SinPassword(actualizado!));
    }

    internal static object SinPassword(Usuario usuario) => new
    {
        usuario.Id,
        usuario.EmpleadoId,
        usuario.RolId,
        usuario.Username,
        usuario.Nombre,
        usuario.Rol,
        usuario.Activo,
    };
}
