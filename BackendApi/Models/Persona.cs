using System.ComponentModel.DataAnnotations;

namespace BackendApi.Models;

public abstract class Persona : IEntidad
{
    public int Id { get; set; }

    [Required(ErrorMessage = "El nombre es obligatorio.")]
    [StringLength(60, MinimumLength = 2, ErrorMessage = "El nombre debe tener entre 2 y 60 caracteres.")]
    [RegularExpression(@"^[\p{L}][\p{L}\s'.-]*$", ErrorMessage = "El nombre contiene caracteres no válidos.")]
    public string Nombre { get; set; } = "";

    [Required(ErrorMessage = "El apellido es obligatorio.")]
    [StringLength(60, MinimumLength = 2, ErrorMessage = "El apellido debe tener entre 2 y 60 caracteres.")]
    [RegularExpression(@"^[\p{L}][\p{L}\s'.-]*$", ErrorMessage = "El apellido contiene caracteres no válidos.")]
    public string Apellido { get; set; } = "";

    [StringLength(20, ErrorMessage = "El CI no puede superar 20 caracteres.")]
    [RegularExpression(@"^$|^[A-Za-z0-9-]{4,20}$", ErrorMessage = "El CI solo puede contener letras, números y guiones.")]
    public string Ci { get; set; } = "";

    [Required(ErrorMessage = "El teléfono es obligatorio.")]
    [RegularExpression(@"^\+?[0-9][0-9\s-]{6,19}$", ErrorMessage = "Ingresa un teléfono válido.")]
    public string Telefono { get; set; } = "";

    [EmailAddress(ErrorMessage = "Ingresa un correo electrónico válido.")]
    [StringLength(120, ErrorMessage = "El correo no puede superar 120 caracteres.")]
    public string Email { get; set; } = "";

    [StringLength(250, ErrorMessage = "La dirección no puede superar 250 caracteres.")]
    public string Direccion { get; set; } = "";
}
