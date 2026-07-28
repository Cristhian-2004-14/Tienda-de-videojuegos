using BackendApi.Data;
using BackendApi.Models;
using Microsoft.AspNetCore.Mvc;

namespace BackendApi.Controllers;

[ApiController, Route("api/[controller]")]
public class ServiciosController(
    IFirestoreRepository<Servicio> repositorio,
    IFirestoreRepository<Cliente> clientes,
    IFirestoreRepository<Dispositivo> dispositivos,
    IFirestoreRepository<Producto> productos) : ControllerBase
{
    [HttpGet] public async Task<IActionResult> Obtener() => Ok(await repositorio.ObtenerTodosAsync());
    [HttpGet("{id:int}")] public async Task<IActionResult> Obtener(int id) =>
        await repositorio.ObtenerPorIdAsync(id) is { } servicio ? Ok(servicio) : NotFound();

    [HttpGet("consulta/{id:int}")]
    public async Task<IActionResult> ConsultaPublica(int id, [FromQuery] string verificacion)
    {
        var servicio = await repositorio.ObtenerPorIdAsync(id);
        if (servicio is null) return NotFound(new { message = "Orden de servicio inexistente." });
        var cliente = await clientes.ObtenerPorIdAsync(servicio.ClienteId);
        var valor = verificacion.Trim();
        if (cliente is null || (!cliente.Ci.Equals(valor, StringComparison.OrdinalIgnoreCase) &&
            !cliente.Telefono.Equals(valor, StringComparison.OrdinalIgnoreCase)))
            return Unauthorized(new { message = "Los datos de validación no coinciden." });
        return Ok(servicio);
    }

    [HttpPost]
    public async Task<IActionResult> Crear(Servicio servicio)
    {
        var cliente = await clientes.ObtenerPorIdAsync(servicio.ClienteId);
        var dispositivo = await dispositivos.ObtenerPorIdAsync(servicio.DispositivoId);
        if (cliente is null) return BadRequest(new { message = "Cliente inexistente." });
        if (dispositivo is null || dispositivo.ClienteId != cliente.Id)
            return BadRequest(new { message = "El dispositivo no pertenece al cliente seleccionado." });
        servicio.Cliente = $"{cliente.Nombre} {cliente.Apellido}".Trim();
        servicio.Dispositivo = $"{dispositivo.Marca} {dispositivo.Modelo}".Trim();
        servicio.Estado = "Recibido";
        servicio.FechaIngreso = DateTime.UtcNow;
        servicio.Detalles = [];
        servicio.Pagos = [];
        var creado = await repositorio.CrearAsync(servicio);
        return CreatedAtAction(nameof(Obtener), new { id = creado.Id }, creado);
    }

    [HttpPut("{id:int}/seguimiento")]
    public async Task<IActionResult> ActualizarSeguimiento(int id, SeguimientoServicioRequest datos)
    {
        var servicio = await repositorio.ObtenerPorIdAsync(id);
        if (servicio is null) return NotFound();
        string[] estados = ["Recibido", "En diagnóstico", "En reparación", "En pruebas", "Listo para entrega", "Entregado", "Cancelado"];
        if (!estados.Contains(datos.Estado)) return BadRequest(new { message = "Estado de servicio inválido." });
        var nuevos = new List<DetalleServicio>();
        foreach (var detalle in datos.Detalles)
        {
            var producto = await productos.ObtenerPorIdAsync(detalle.ProductoId);
            if (producto is null || detalle.Cantidad <= 0)
                return BadRequest(new { message = $"Repuesto inválido: {detalle.ProductoId}." });
            var anterior = servicio.Detalles.Where(d => d.ProductoId == detalle.ProductoId).Sum(d => d.Cantidad);
            if (detalle.Cantidad - anterior > producto.Stock)
                return BadRequest(new { message = $"Stock insuficiente para {producto.Nombre}." });
            var precio = detalle.PrecioUnitario > 0 ? detalle.PrecioUnitario : producto.PrecioVenta;
            nuevos.Add(new()
            {
                ProductoId = producto.Id, Producto = producto.Nombre, Cantidad = detalle.Cantidad,
                PrecioUnitario = precio, Subtotal = Math.Round(detalle.Cantidad * precio, 2),
            });
        }
        var ids = servicio.Detalles.Select(d => d.ProductoId).Union(nuevos.Select(d => d.ProductoId)).Distinct();
        foreach (var productoId in ids)
        {
            var producto = await productos.ObtenerPorIdAsync(productoId);
            if (producto is null) continue;
            var anterior = servicio.Detalles.Where(d => d.ProductoId == productoId).Sum(d => d.Cantidad);
            var nuevo = nuevos.Where(d => d.ProductoId == productoId).Sum(d => d.Cantidad);
            producto.Stock -= nuevo - anterior;
            await productos.ActualizarAsync(producto.Id, producto);
        }
        servicio.Estado = datos.Estado;
        servicio.Diagnostico = datos.Diagnostico.Trim();
        servicio.CostoManoObra = Math.Max(0, datos.CostoManoObra);
        servicio.Detalles = nuevos;
        if (datos.Estado == "Entregado" && servicio.FechaEntrega is null) servicio.FechaEntrega = DateTime.UtcNow;
        return Ok(await repositorio.ActualizarAsync(id, servicio));
    }

    [HttpPost("{id:int}/pagos")]
    public async Task<IActionResult> RegistrarPago(int id, Pago pago)
    {
        var servicio = await repositorio.ObtenerPorIdAsync(id);
        if (servicio is null) return NotFound();
        if (pago.Monto <= 0 || pago.Monto > servicio.SaldoPendiente + .009)
            return BadRequest(new { message = "El monto no puede superar el saldo pendiente." });
        pago.Id = servicio.Pagos.Count == 0 ? 1 : servicio.Pagos.Max(item => item.Id) + 1;
        pago.Fecha = DateTime.UtcNow;
        servicio.Pagos.Add(pago);
        return Ok(await repositorio.ActualizarAsync(id, servicio));
    }
}

public record SeguimientoServicioRequest(
    string Estado, string Diagnostico, double CostoManoObra, List<DetalleServicio> Detalles);
