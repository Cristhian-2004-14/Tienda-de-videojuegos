using BackendApi.Data;
using BackendApi.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace BackendApi.Controllers;

[ApiController, Route("api/[controller]")]
public class AuthController(
    IFirestoreRepository<Usuario> usuarios,
    IFirestoreRepository<Rol> roles,
    IPasswordHasher<Usuario> passwordHasher) : ControllerBase
{
    [HttpPost("login")]
    public async Task<IActionResult> IniciarSesion(LoginRequest credenciales)
    {
        var usuario = (await usuarios.ObtenerTodosAsync()).FirstOrDefault(item =>
            item.Username.Equals(credenciales.Username, StringComparison.OrdinalIgnoreCase) && item.Activo);
        if (usuario is null) return Unauthorized(new { message = "Usuario o contraseña incorrectos." });
        var resultado = passwordHasher.VerifyHashedPassword(usuario, usuario.Password, credenciales.Password);
        if (resultado == PasswordVerificationResult.Failed)
            return Unauthorized(new { message = "Usuario o contraseña incorrectos." });
        var rol = await roles.ObtenerPorIdAsync(usuario.RolId);
        return Ok(new
        {
            usuario.Id, usuario.EmpleadoId, usuario.RolId, usuario.Username,
            usuario.Nombre, usuario.Rol, usuario.Activo,
            Permisos = rol?.Permisos ?? [],
        });
    }
}

public record LoginRequest(string Username, string Password);
