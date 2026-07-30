using System.ComponentModel.DataAnnotations;

namespace BackendApi.Models;

public class Proveedor : IEntidad
{
    public int Id { get; set; }
    [Required(ErrorMessage = "La razón social es obligatoria.")]
    [StringLength(120, MinimumLength = 2, ErrorMessage = "La razón social debe tener entre 2 y 120 caracteres.")]
    public string RazonSocial { get; set; } = "";
    [RegularExpression(@"^$|^[A-Za-z0-9-]{5,20}$", ErrorMessage = "Ingresa un NIT válido.")]
    public string Nit { get; set; } = "";
    [RegularExpression(@"^$|^\+?[0-9][0-9\s-]{6,19}$", ErrorMessage = "Ingresa un teléfono válido.")]
    public string Telefono { get; set; } = "";
    [EmailAddress(ErrorMessage = "Ingresa un correo electrónico válido.")]
    public string Email { get; set; } = "";
    [StringLength(250, ErrorMessage = "La dirección no puede superar 250 caracteres.")]
    public string Direccion { get; set; } = "";
    public bool Activo { get; set; } = true;
}
