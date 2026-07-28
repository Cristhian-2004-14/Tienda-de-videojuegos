namespace BackendApi.Models;

public class Cliente : Persona
{
    public DateTime FechaRegistro { get; set; } = DateTime.UtcNow;
}
