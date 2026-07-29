using BackendApi.Data;
using BackendApi.Models;
using Microsoft.AspNetCore.Mvc;

namespace BackendApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductosController(IRepository<Producto> repositorio) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Producto>>> ObtenerProductos() =>
        Ok(await repositorio.ObtenerTodosAsync());

    [HttpGet("{id:int}")]
    public async Task<ActionResult<Producto>> ObtenerProducto(int id)
    {
        var producto = await repositorio.ObtenerPorIdAsync(id);
        return producto is null ? NotFound() : Ok(producto);
    }

    [HttpPost]
    public async Task<ActionResult<Producto>> RegistrarProducto(Producto producto)
    {
        var creado = await repositorio.CrearAsync(producto);
        return CreatedAtAction(nameof(ObtenerProducto), new { id = creado.Id }, creado);
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult<Producto>> ActualizarProducto(int id, Producto producto)
    {
        var actualizado = await repositorio.ActualizarAsync(id, producto);
        return actualizado is null ? NotFound() : Ok(actualizado);
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> EliminarProducto(int id)
    {
        var producto = await repositorio.ObtenerPorIdAsync(id);
        if (producto is null) return NotFound();
        producto.Activo = false;
        return Ok(await repositorio.ActualizarAsync(id, producto));
    }
}
