using System.ComponentModel.DataAnnotations;

namespace BackendApi.Models;

public class Servicio : IEntidad
{
    public int Id { get; set; }
    [Range(1, int.MaxValue, ErrorMessage = "Selecciona un cliente.")]
    public int ClienteId { get; set; }
    public string Cliente { get; set; } = "";
    public int EmpleadoId { get; set; }
    public string Empleado { get; set; } = "";
    [Range(1, int.MaxValue, ErrorMessage = "Selecciona un dispositivo.")]
    public int DispositivoId { get; set; }
    public string Dispositivo { get; set; } = "";
    public string Estado { get; set; } = "Pendiente";
    [Required(ErrorMessage = "El diagnóstico es obligatorio.")]
    [StringLength(1000, MinimumLength = 5)]
    public string Diagnostico { get; set; } = "";
    public DateTime FechaIngreso { get; set; } = DateTime.UtcNow;
    public DateTime? FechaEntrega { get; set; }
    [Range(0, double.MaxValue, ErrorMessage = "El costo de mano de obra no puede ser negativo.")]
    public double CostoManoObra { get; set; }
    public List<DetalleServicio> Detalles { get; set; } = [];
    public List<Pago> Pagos { get; set; } = [];
    public double Total => CostoManoObra + Detalles.Sum(detalle => detalle.Subtotal);
    public double SaldoPendiente => Math.Max(0, Total - Pagos.Sum(pago => pago.Monto));
}

public class DetalleServicio
{
    [Range(1, int.MaxValue)]
    public int ProductoId { get; set; }
    public string Producto { get; set; } = "";
    [Range(1, int.MaxValue)]
    public int Cantidad { get; set; }
    [Range(0.01, double.MaxValue)]
    public double PrecioUnitario { get; set; }
    public double Subtotal { get; set; }
}
