using System.ComponentModel.DataAnnotations;

namespace BackendApi.Models;

public class Usuario : IEntidad
{
    public int Id { get; set; }
    [Range(1, int.MaxValue, ErrorMessage = "El empleado es obligatorio.")]
    public int EmpleadoId { get; set; }
    [Range(1, int.MaxValue, ErrorMessage = "El rol es obligatorio.")]
    public int RolId { get; set; }
    [Required(ErrorMessage = "El nombre de usuario es obligatorio.")]
    [StringLength(40, MinimumLength = 4, ErrorMessage = "El usuario debe tener entre 4 y 40 caracteres.")]
    [RegularExpression(@"^[A-Za-z0-9._-]+$", ErrorMessage = "El usuario contiene caracteres no válidos.")]
    public string Username { get; set; } = "";
    [Required(ErrorMessage = "La contraseña es obligatoria.")]
    [MinLength(6, ErrorMessage = "La contraseña debe tener al menos 6 caracteres.")]
    public string Password { get; set; } = "";
    public string Nombre { get; set; } = "";
    public string Rol { get; set; } = "";
    public bool Activo { get; set; } = true;
}
