using BackendApi.Data;
using BackendApi.Models;
using Microsoft.AspNetCore.Mvc;

namespace BackendApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class VentasController(
    IFirestoreRepository<Venta> repositorio,
    IFirestoreRepository<Producto> productos) : ControllerBase
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
        if (venta.Detalles.Count == 0) return BadRequest(new { message = "La venta debe incluir productos." });
        foreach (var detalle in venta.Detalles)
        {
            var producto = await productos.ObtenerPorIdAsync(detalle.ProductoId);
            if (producto is null) return BadRequest(new { message = $"Producto {detalle.ProductoId} inexistente." });
            if (detalle.Cantidad <= 0 || producto.Stock < detalle.Cantidad)
                return BadRequest(new { message = $"Stock insuficiente para {producto.Nombre}." });
            detalle.Producto = producto.Nombre;
            detalle.PrecioUnitario = detalle.PrecioUnitario > 0 ? detalle.PrecioUnitario : producto.PrecioVenta;
            detalle.Subtotal = Math.Round(detalle.Cantidad * detalle.PrecioUnitario, 2);
        }
        venta.Subtotal = Math.Round(venta.Detalles.Sum(detalle => detalle.Subtotal), 2);
        venta.Descuento = Math.Round(Math.Clamp(venta.Descuento, 0, venta.Subtotal), 2);
        venta.Total = Math.Round(venta.Subtotal - venta.Descuento, 2);
        venta.Estado = "Pendiente";
        venta.Fecha = venta.Fecha.Kind == DateTimeKind.Utc
            ? venta.Fecha
            : venta.Fecha.ToUniversalTime();
        var creada = await repositorio.CrearAsync(venta);
        foreach (var detalle in venta.Detalles)
        {
            var producto = (await productos.ObtenerPorIdAsync(detalle.ProductoId))!;
            producto.Stock -= detalle.Cantidad;
            await productos.ActualizarAsync(producto.Id, producto);
        }
        return CreatedAtAction(nameof(ObtenerVenta), new { id = creada.Id }, creada);
    }

    [HttpPost("{id:int}/pagos")]
    public async Task<ActionResult<Venta>> RegistrarPago(int id, Pago pago)
    {
        var venta = await repositorio.ObtenerPorIdAsync(id);
        if (venta is null) return NotFound();
        if (pago.Monto <= 0 || pago.Monto > venta.SaldoPendiente)
            return BadRequest(new { message = "El monto debe ser mayor a cero y no superar el saldo pendiente." });
        pago.Id = venta.Pagos.Count == 0 ? 1 : venta.Pagos.Max(item => item.Id) + 1;
        pago.Fecha = DateTime.UtcNow;
        venta.Pagos.Add(pago);
        venta.Estado = venta.SaldoPendiente <= 0.009 ? "Completada" : "Pendiente";
        return Ok(await repositorio.ActualizarAsync(id, venta));
    }

    [HttpPut("{id:int}/anular")]
    public async Task<ActionResult<Venta>> Anular(int id)
    {
        var venta = await repositorio.ObtenerPorIdAsync(id);
        if (venta is null) return NotFound();
        if (venta.Estado == "Anulada") return Ok(venta);
        foreach (var detalle in venta.Detalles)
        {
            var producto = await productos.ObtenerPorIdAsync(detalle.ProductoId);
            if (producto is null) continue;
            producto.Stock += detalle.Cantidad;
            await productos.ActualizarAsync(producto.Id, producto);
        }
        venta.Estado = "Anulada";
        return Ok(await repositorio.ActualizarAsync(id, venta));
    }
}
