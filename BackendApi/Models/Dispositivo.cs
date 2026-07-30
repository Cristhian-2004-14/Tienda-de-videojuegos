using System.ComponentModel.DataAnnotations;

namespace BackendApi.Models;

public class Dispositivo : IEntidad
{
    public int Id { get; set; }
    [Range(1, int.MaxValue, ErrorMessage = "Selecciona un cliente.")]
    public int ClienteId { get; set; }
    public string Cliente { get; set; } = "";
    [Required(ErrorMessage = "El tipo de dispositivo es obligatorio.")]
    public string Tipo { get; set; } = "";
    [Required(ErrorMessage = "La marca es obligatoria.")]
    [StringLength(80)]
    public string Marca { get; set; } = "";
    [Required(ErrorMessage = "El modelo es obligatorio.")]
    [StringLength(100)]
    public string Modelo { get; set; } = "";
    [StringLength(100)]
    public string NumeroSerie { get; set; } = "";
    [StringLength(500)]
    public string Observaciones { get; set; } = "";
}
