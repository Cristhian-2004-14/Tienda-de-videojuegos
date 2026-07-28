using BackendApi.Data;
using BackendApi.Models;
using Microsoft.AspNetCore.Mvc;

namespace BackendApi.Controllers;

[ApiController, Route("api/empleados")]
public class EmpleadosController(IFirestoreRepository<Empleado> repositorio) : ControllerBase
{
    [HttpGet] public async Task<IActionResult> Obtener() => Ok(await repositorio.ObtenerTodosAsync());
    [HttpGet("{id:int}")] public async Task<IActionResult> ObtenerPorId(int id) =>
        await repositorio.ObtenerPorIdAsync(id) is { } entidad ? Ok(entidad) : NotFound();
    [HttpPost] public async Task<IActionResult> Crear(Empleado entidad) => Ok(await repositorio.CrearAsync(entidad));
    [HttpPut("{id:int}")] public async Task<IActionResult> Actualizar(int id, Empleado entidad) =>
        await repositorio.ActualizarAsync(id, entidad) is { } actualizado ? Ok(actualizado) : NotFound();
}

[ApiController, Route("api/roles")]
public class RolesController(IFirestoreRepository<Rol> repositorio) : ControllerBase
{
    [HttpGet] public async Task<IActionResult> Obtener() => Ok(await repositorio.ObtenerTodosAsync());
    [HttpGet("{id:int}")] public async Task<IActionResult> ObtenerPorId(int id) =>
        await repositorio.ObtenerPorIdAsync(id) is { } entidad ? Ok(entidad) : NotFound();
    [HttpPost] public async Task<IActionResult> Crear(Rol entidad) => Ok(await repositorio.CrearAsync(entidad));
    [HttpPut("{id:int}")]
    public async Task<IActionResult> Actualizar(int id, Rol entidad)
    {
        var existente = await repositorio.ObtenerPorIdAsync(id);
        if (existente is null) return NotFound();
        if (existente.Protegido)
        {
            entidad.Nombre = existente.Nombre;
            entidad.Permisos = existente.Permisos;
            entidad.Protegido = true;
        }
        return Ok(await repositorio.ActualizarAsync(id, entidad));
    }
}

[ApiController, Route("api/proveedores")]
public class ProveedoresController(IFirestoreRepository<Proveedor> repositorio) : ControllerBase
{
    [HttpGet] public async Task<IActionResult> Obtener() => Ok(await repositorio.ObtenerTodosAsync());
    [HttpGet("{id:int}")] public async Task<IActionResult> ObtenerPorId(int id) =>
        await repositorio.ObtenerPorIdAsync(id) is { } entidad ? Ok(entidad) : NotFound();
    [HttpPost] public async Task<IActionResult> Crear(Proveedor entidad) => Ok(await repositorio.CrearAsync(entidad));
    [HttpPut("{id:int}")] public async Task<IActionResult> Actualizar(int id, Proveedor entidad) =>
        await repositorio.ActualizarAsync(id, entidad) is { } actualizado ? Ok(actualizado) : NotFound();
}

[ApiController, Route("api/compras")]
public class ComprasController(
    IFirestoreRepository<Compra> repositorio,
    IFirestoreRepository<Producto> productos,
    IFirestoreRepository<Proveedor> proveedores) : ControllerBase
{
    [HttpGet] public async Task<IActionResult> Obtener() => Ok(await repositorio.ObtenerTodosAsync());
    [HttpGet("{id:int}")] public async Task<IActionResult> ObtenerPorId(int id) =>
        await repositorio.ObtenerPorIdAsync(id) is { } entidad ? Ok(entidad) : NotFound();
    [HttpPost]
    public async Task<IActionResult> Crear(Compra entidad)
    {
        if (entidad.Detalles.Count == 0) return BadRequest(new { message = "La compra debe incluir productos." });
        var proveedor = await proveedores.ObtenerPorIdAsync(entidad.ProveedorId);
        if (proveedor is null) return BadRequest(new { message = "Proveedor inexistente." });
        entidad.Proveedor = proveedor.RazonSocial;
        foreach (var detalle in entidad.Detalles)
        {
            var producto = await productos.ObtenerPorIdAsync(detalle.ProductoId);
            if (producto is null || detalle.Cantidad <= 0 || detalle.PrecioUnitario <= 0)
                return BadRequest(new { message = $"Detalle inválido para producto {detalle.ProductoId}." });
            detalle.Producto = producto.Nombre;
            detalle.Subtotal = Math.Round(detalle.Cantidad * detalle.PrecioUnitario, 2);
        }
        entidad.Total = Math.Round(entidad.Detalles.Sum(detalle => detalle.Subtotal), 2);
        entidad.Fecha = DateTime.UtcNow;
        var creada = await repositorio.CrearAsync(entidad);
        foreach (var detalle in entidad.Detalles)
        {
            var producto = (await productos.ObtenerPorIdAsync(detalle.ProductoId))!;
            producto.Stock += detalle.Cantidad;
            producto.PrecioCompra = detalle.PrecioUnitario;
            await productos.ActualizarAsync(producto.Id, producto);
        }
        return Ok(creada);
    }
}

[ApiController, Route("api/dispositivos")]
public class DispositivosController(IFirestoreRepository<Dispositivo> repositorio) : ControllerBase
{
    [HttpGet] public async Task<IActionResult> Obtener() => Ok(await repositorio.ObtenerTodosAsync());
    [HttpGet("{id:int}")] public async Task<IActionResult> ObtenerPorId(int id) =>
        await repositorio.ObtenerPorIdAsync(id) is { } entidad ? Ok(entidad) : NotFound();
    [HttpPost] public async Task<IActionResult> Crear(Dispositivo entidad) => Ok(await repositorio.CrearAsync(entidad));
    [HttpPut("{id:int}")] public async Task<IActionResult> Actualizar(int id, Dispositivo entidad) =>
        await repositorio.ActualizarAsync(id, entidad) is { } actualizado ? Ok(actualizado) : NotFound();
}
