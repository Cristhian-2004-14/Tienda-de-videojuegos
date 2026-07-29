namespace BackendApi.Models;

public class Servicio : IEntidad
{
    public int Id { get; set; }
    public int ClienteId { get; set; }
    public string Cliente { get; set; } = "";
    public int EmpleadoId { get; set; }
    public string Empleado { get; set; } = "";
    public int DispositivoId { get; set; }
    public string Dispositivo { get; set; } = "";
    public string Estado { get; set; } = "Pendiente";
    public string Diagnostico { get; set; } = "";
    public DateTime FechaIngreso { get; set; } = DateTime.UtcNow;
    public DateTime? FechaEntrega { get; set; }
    public double CostoManoObra { get; set; }
    public List<DetalleServicio> Detalles { get; set; } = [];
    public List<Pago> Pagos { get; set; } = [];
    public double Total => CostoManoObra + Detalles.Sum(detalle => detalle.Subtotal);
    public double SaldoPendiente => Math.Max(0, Total - Pagos.Sum(pago => pago.Monto));
}

public class DetalleServicio : IEntidad
{
    public int Id { get; set; }
    public int ServicioId { get; set; }
    public int ProductoId { get; set; }
    public string Producto { get; set; } = "";
    public int Cantidad { get; set; }
    public double PrecioUnitario { get; set; }
    public double Subtotal { get; set; }
}
