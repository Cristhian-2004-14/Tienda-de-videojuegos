using System.ComponentModel.DataAnnotations;

namespace BackendApi.Models;

public class Rol : IEntidad
{
    public int Id { get; set; }
    [Required(ErrorMessage = "El nombre del rol es obligatorio.")]
    [StringLength(60, MinimumLength = 2)]
    public string Nombre { get; set; } = "";
    [Required(ErrorMessage = "La descripción es obligatoria.")]
    [StringLength(250, MinimumLength = 5)]
    public string Descripcion { get; set; } = "";
    [MinLength(1, ErrorMessage = "Selecciona al menos un permiso.")]
    public List<string> Permisos { get; set; } = [];
    public bool Protegido { get; set; }
}
