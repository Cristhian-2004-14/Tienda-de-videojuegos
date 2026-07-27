using BackendApi.Data;
using BackendApi.Models;
using Microsoft.AspNetCore.Mvc;

namespace BackendApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ServiciosController(IFirestoreRepository<Servicio> repositorio) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Servicio>>> ObtenerServicios() =>
        Ok(await repositorio.ObtenerTodosAsync());

    [HttpGet("{id:int}")]
    public async Task<ActionResult<Servicio>> ObtenerServicio(int id)
    {
        var servicio = await repositorio.ObtenerPorIdAsync(id);
        return servicio is null ? NotFound() : Ok(servicio);
    }

    [HttpPost]
    public async Task<ActionResult<Servicio>> RegistrarServicio(Servicio servicio)
    {
        servicio.FechaIngreso = servicio.FechaIngreso.Kind == DateTimeKind.Utc
            ? servicio.FechaIngreso
            : servicio.FechaIngreso.ToUniversalTime();
        var creado = await repositorio.CrearAsync(servicio);
        return CreatedAtAction(nameof(ObtenerServicio), new { id = creado.Id }, creado);
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult<Servicio>> ActualizarServicio(int id, Servicio servicio)
    {
        var actualizado = await repositorio.ActualizarAsync(id, servicio);
        return actualizado is null ? NotFound() : Ok(actualizado);
    }
}
