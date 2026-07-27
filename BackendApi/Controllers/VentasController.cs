using BackendApi.Data;
using BackendApi.Models;
using Microsoft.AspNetCore.Mvc;

namespace BackendApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class VentasController(IFirestoreRepository<Venta> repositorio) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Venta>>> ObtenerVentas() =>
        Ok(await repositorio.ObtenerTodosAsync());

    [HttpGet("{id:int}")]
    public async Task<ActionResult<Venta>> ObtenerVenta(int id)
    {
        var venta = await repositorio.ObtenerPorIdAsync(id);
        return venta is null ? NotFound() : Ok(venta);
    }

    [HttpPost]
    public async Task<ActionResult<Venta>> RegistrarVenta(Venta venta)
    {
        venta.Fecha = venta.Fecha.Kind == DateTimeKind.Utc
            ? venta.Fecha
            : venta.Fecha.ToUniversalTime();
        var creada = await repositorio.CrearAsync(venta);
        return CreatedAtAction(nameof(ObtenerVenta), new { id = creada.Id }, creada);
    }
}
