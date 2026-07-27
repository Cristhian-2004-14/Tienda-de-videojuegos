using BackendApi.Data;
using BackendApi.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace BackendApi.Controllers;

[ApiController]
[Route("api/database")]
public class DatabaseController(
    IFirestoreRepository<Cliente> clientes,
    IFirestoreRepository<Producto> productos,
    IFirestoreRepository<Venta> ventas,
    IFirestoreRepository<Servicio> servicios,
    IFirestoreRepository<Usuario> usuarios,
    IPasswordHasher<Usuario> passwordHasher) : ControllerBase
{
    [HttpPost("seed")]
    public async Task<IActionResult> CrearColecciones()
    {
        var creados = new List<string>();

        if ((await clientes.ObtenerTodosAsync()).Count == 0)
        {
            foreach (var cliente in DatosIniciales.Clientes) await clientes.CrearAsync(cliente);
            creados.Add("clientes");
        }

        if ((await productos.ObtenerTodosAsync()).Count == 0)
        {
            foreach (var producto in DatosIniciales.Productos) await productos.CrearAsync(producto);
            creados.Add("productos");
        }

        if ((await ventas.ObtenerTodosAsync()).Count == 0)
        {
            foreach (var venta in DatosIniciales.Ventas) await ventas.CrearAsync(venta);
            creados.Add("ventas");
        }

        if ((await servicios.ObtenerTodosAsync()).Count == 0)
        {
            foreach (var servicio in DatosIniciales.Servicios) await servicios.CrearAsync(servicio);
            creados.Add("servicios");
        }

        if ((await usuarios.ObtenerTodosAsync()).Count == 0)
        {
            foreach (var usuario in DatosIniciales.Usuarios)
            {
                usuario.Password = passwordHasher.HashPassword(usuario, usuario.Password);
                await usuarios.CrearAsync(usuario);
            }
            creados.Add("usuarios");
        }

        return Ok(new
        {
            message = creados.Count == 0
                ? "Las colecciones ya contenían datos."
                : "Colecciones inicializadas correctamente.",
            coleccionesCreadas = creados,
        });
    }
}

internal static class DatosIniciales
{
    internal static readonly Cliente[] Clientes =
    [
        new() { Nombre = "Alex", Apellido = "Rivera", Telefono = "8888-1234", Email = "alex.rivera@correo.com" },
        new() { Nombre = "Sarah", Apellido = "Connor", Telefono = "8888-5678", Email = "sarah.connor@correo.com" },
        new() { Nombre = "Marcus", Apellido = "Fenix", Telefono = "8888-9012", Email = "marcus.fenix@correo.com" },
        new() { Nombre = "Jill", Apellido = "Valentine", Telefono = "8888-3456", Email = "jill.valentine@correo.com" },
        new() { Nombre = "Ethan", Apellido = "Winters", Telefono = "8888-7890", Email = "ethan.winters@correo.com" },
    ];

    internal static readonly Producto[] Productos =
    [
        new() { Nombre = "Xbox Series X", Categoria = "Consolas", Marca = "Microsoft", PrecioVenta = 499.99, Stock = 24 },
        new() { Nombre = "DualSense Controller", Categoria = "Accesorios", Marca = "Sony", PrecioVenta = 69.99, Stock = 3 },
        new() { Nombre = "Elden Ring", Categoria = "Videojuegos", Marca = "Bandai Namco", PrecioVenta = 59.99, Stock = 112 },
        new() { Nombre = "Halo Infinite", Categoria = "Videojuegos", Marca = "Xbox Game Studios", PrecioVenta = 59.99, Stock = 0 },
        new() { Nombre = "Cable HDMI 2.1 4K", Categoria = "Accesorios", Marca = "Genérico", PrecioVenta = 39.99, Stock = 87 },
    ];

    internal static readonly Venta[] Ventas =
    [
        new() { ClienteId = 1, Cliente = "Alex Rivera", Fecha = new DateTime(2026, 7, 15, 0, 0, 0, DateTimeKind.Utc), Total = 179.99, Estado = "Completada" },
        new() { ClienteId = 2, Cliente = "Sarah Connor", Fecha = new DateTime(2026, 7, 16, 0, 0, 0, DateTimeKind.Utc), Total = 499, Estado = "Completada" },
    ];

    internal static readonly Servicio[] Servicios =
    [
        new() { ClienteId = 1, Cliente = "Alex Rivera", Dispositivo = "Xbox Series X", Estado = "En proceso", Diagnostico = "Limpieza y cambio de pasta térmica" },
        new() { ClienteId = 2, Cliente = "Sarah Connor", Dispositivo = "DualSense Controller", Estado = "Esperando repuestos", Diagnostico = "Reparación de stick drift" },
        new() { ClienteId = 3, Cliente = "Marcus Fenix", Dispositivo = "PlayStation 5", Estado = "En pruebas", Diagnostico = "Reemplazo de puerto HDMI" },
    ];

    internal static readonly Usuario[] Usuarios =
    [
        new() { Username = "admin", Password = "admin123", Nombre = "Administrador", Rol = "Administrador" },
        new() { Username = "jperez", Password = "vendedor123", Nombre = "Juan Pérez", Rol = "Vendedor" },
        new() { Username = "mrodriguez", Password = "tecnico123", Nombre = "María Rodríguez", Rol = "Técnico" },
    ];
}
