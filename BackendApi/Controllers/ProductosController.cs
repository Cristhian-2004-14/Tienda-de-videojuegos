using BackendApi.Data;
using BackendApi.Models;
using Microsoft.AspNetCore.Mvc;

namespace BackendApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductosController(
    IFirestoreRepository<Producto> repositorio,
    IFirestoreRepository<ImagenProductoDocumento> imagenes) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Producto>>> ObtenerProductos()
    {
        var productos = await repositorio.ObtenerTodosAsync();
        return Ok(productos);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<Producto>> ObtenerProducto(int id)
    {
        var producto = await repositorio.ObtenerPorIdAsync(id);
        if (producto is null) return NotFound();
        producto.Imagenes = (await imagenes.ObtenerTodosAsync())
            .Where(imagen => imagen.ProductoId == id)
            .OrderBy(imagen => imagen.Orden)
            .Select(ConvertirImagen)
            .ToList();
        return Ok(producto);
    }

    [HttpPost]
    public async Task<ActionResult<Producto>> RegistrarProducto(Producto producto)
    {
        LimpiarImagenesEmbebidas(producto);
        var creado = await repositorio.CrearAsync(producto);
        return CreatedAtAction(nameof(ObtenerProducto), new { id = creado.Id }, creado);
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult<Producto>> ActualizarProducto(int id, Producto producto)
    {
        LimpiarImagenesEmbebidas(producto);
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

    [HttpPost("{id:int}/imagenes")]
    public async Task<ActionResult<ImagenProducto>> AgregarImagen(int id, ImagenProductoRequest datos)
    {
        if (await repositorio.ObtenerPorIdAsync(id) is null) return NotFound();
        var existentes = (await imagenes.ObtenerTodosAsync())
            .Where(imagen => imagen.ProductoId == id)
            .ToList();
        if (existentes.Count >= 1)
            return BadRequest(new { message = "Cada producto admite una sola imagen." });
        if (datos.TipoMime != "image/webp" || !datos.Contenido.StartsWith("data:image/webp;base64,"))
            return BadRequest(new { message = "La imagen debe estar comprimida en formato WebP." });
        if (!datos.Miniatura.StartsWith("data:image/webp;base64,") || datos.Miniatura.Length > 120_000)
            return BadRequest(new { message = "La miniatura de la imagen no es válida." });
        if (datos.Contenido.Length > 800_000)
            return BadRequest(new { message = "La imagen comprimida no puede superar aproximadamente 600 KB." });

        var creada = await imagenes.CrearAsync(new()
        {
            ProductoId = id,
            Contenido = datos.Contenido,
            Miniatura = datos.Miniatura,
            TipoMime = datos.TipoMime,
            Orden = existentes.Count,
        });
        if (existentes.Count == 0)
        {
            var producto = (await repositorio.ObtenerPorIdAsync(id))!;
            producto.ImagenUrl = creada.Miniatura;
            await repositorio.ActualizarAsync(id, producto);
        }
        return Created("", ConvertirImagen(creada));
    }

    [HttpDelete("{id:int}/imagenes/{imagenId:int}")]
    public async Task<IActionResult> EliminarImagen(int id, int imagenId)
    {
        var imagen = await imagenes.ObtenerPorIdAsync(imagenId);
        if (imagen is null || imagen.ProductoId != id) return NotFound();
        await imagenes.EliminarAsync(imagenId);
        var producto = await repositorio.ObtenerPorIdAsync(id);
        if (producto is not null)
        {
            producto.ImagenUrl = (await imagenes.ObtenerTodosAsync())
                .Where(item => item.ProductoId == id)
                .OrderBy(item => item.Orden)
                .Select(item => item.Miniatura)
                .FirstOrDefault() ?? "";
            await repositorio.ActualizarAsync(id, producto);
        }
        return NoContent();
    }

    private static ImagenProducto ConvertirImagen(ImagenProductoDocumento imagen) => new()
    {
        Id = imagen.Id,
        Url = imagen.Contenido,
    };

    private static void LimpiarImagenesEmbebidas(Producto producto)
    {
        producto.Imagenes = [];
        producto.ImagenStoragePath = "";
    }
}

public record ImagenProductoRequest(string Contenido, string Miniatura, string TipoMime);
