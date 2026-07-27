using BackendApi.Data;
using BackendApi.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace BackendApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController(
    IFirestoreRepository<Usuario> repositorio,
    IPasswordHasher<Usuario> passwordHasher) : ControllerBase
{
    [HttpPost("login")]
    public async Task<IActionResult> IniciarSesion(LoginRequest credenciales)
    {
        var usuarios = await repositorio.ObtenerTodosAsync();
        var usuario = usuarios.FirstOrDefault(item =>
            item.Username.Equals(credenciales.Username, StringComparison.OrdinalIgnoreCase) &&
            item.Activo);

        if (usuario is null) return Unauthorized(new { message = "Usuario o contraseña incorrectos." });

        var resultado = passwordHasher.VerifyHashedPassword(
            usuario,
            usuario.Password,
            credenciales.Password);

        return resultado == PasswordVerificationResult.Failed
            ? Unauthorized(new { message = "Usuario o contraseña incorrectos." })
            : Ok(UsuariosController.SinPassword(usuario));
    }
}

public record LoginRequest(string Username, string Password);
