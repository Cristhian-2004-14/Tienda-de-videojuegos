using System.ComponentModel.DataAnnotations;

namespace BackendApi.Models;

// Se embebe dentro de Venta o Servicio; no necesita colección independiente.
public class Pago
{
    public int Id { get; set; }
    [Range(0.01, double.MaxValue, ErrorMessage = "El monto debe ser mayor que cero.")]
    public double Monto { get; set; }
    [Required(ErrorMessage = "El método de pago es obligatorio.")]
    [RegularExpression("^(Efectivo|QR|Tarjeta|Transferencia)$", ErrorMessage = "El método de pago no es válido.")]
    public string MetodoPago { get; set; } = "";
    [StringLength(120)]
    public string Referencia { get; set; } = "";
    public DateTime Fecha { get; set; } = DateTime.UtcNow;
}
