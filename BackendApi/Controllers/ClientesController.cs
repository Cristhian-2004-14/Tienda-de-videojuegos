using BackendApi.Data;
using BackendApi.Models;
using Microsoft.AspNetCore.Mvc;

namespace BackendApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ClientesController(IRepository<Cliente> repositorio) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Cliente>>> ObtenerClientes() =>
        Ok(await repositorio.ObtenerTodosAsync());

    [HttpGet("{id:int}")]
    public async Task<ActionResult<Cliente>> ObtenerCliente(int id)
    {
        var cliente = await repositorio.ObtenerPorIdAsync(id);
        return cliente is null ? NotFound() : Ok(cliente);
    }

    [HttpPost]
    public async Task<ActionResult<Cliente>> RegistrarCliente(Cliente cliente)
    {
        if (!string.IsNullOrWhiteSpace(cliente.Ci) &&
            (await repositorio.ObtenerTodosAsync()).Any(item => item.Ci == cliente.Ci))
            return Conflict(new { message = "Ya existe un cliente registrado con ese CI." });
        cliente.FechaRegistro = DateTime.UtcNow;
        var creado = await repositorio.CrearAsync(cliente);
        return CreatedAtAction(nameof(ObtenerCliente), new { id = creado.Id }, creado);
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult<Cliente>> ActualizarCliente(int id, Cliente cliente)
    {
        if (!string.IsNullOrWhiteSpace(cliente.Ci) &&
            (await repositorio.ObtenerTodosAsync()).Any(item => item.Id != id && item.Ci == cliente.Ci))
            return Conflict(new { message = "Ya existe otro cliente registrado con ese CI." });
        var actualizado = await repositorio.ActualizarAsync(id, cliente);
        return actualizado is null ? NotFound() : Ok(actualizado);
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> EliminarCliente(int id) =>
        await repositorio.EliminarAsync(id) ? NoContent() : NotFound();
}
