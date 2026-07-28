namespace BackendApi.Models;

public class Empleado : Persona
{
    public string Cargo { get; set; } = "";
    public DateTime FechaIngreso { get; set; } = DateTime.UtcNow;
    public double Salario { get; set; }
    public bool Activo { get; set; } = true;
}
