using System.ComponentModel.DataAnnotations;

namespace BackendApi.Models;

public class Empleado : Persona
{
    [Required(ErrorMessage = "El cargo es obligatorio.")]
    [StringLength(80)]
    public string Cargo { get; set; } = "";
    public DateTime FechaIngreso { get; set; } = DateTime.UtcNow;
    [Range(0, double.MaxValue, ErrorMessage = "El salario no puede ser negativo.")]
    public double Salario { get; set; }
    public bool Activo { get; set; } = true;
}
