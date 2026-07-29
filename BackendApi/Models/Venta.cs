namespace BackendApi.Models;

public class Venta : IEntidad
{
    public int Id { get; set; }
    public int ClienteId { get; set; }
    public string Cliente { get; set; } = "";
    public string TipoCliente { get; set; } = "Registrado";
    public string ClienteCi { get; set; } = "";
    public string ClienteTelefono { get; set; } = "";
    public string ClienteEmail { get; set; } = "";
    public string ClienteDireccion { get; set; } = "";
    public int EmpleadoId { get; set; }
    public string Empleado { get; set; } = "";
    public DateTime Fecha { get; set; } = DateTime.UtcNow;
    public double Subtotal { get; set; }
    public double Descuento { get; set; }
    public double Total { get; set; }
    public string Estado { get; set; } = "Pendiente";
    public List<DetalleVenta> Detalles { get; set; } = [];
    public List<Pago> Pagos { get; set; } = [];
    public double SaldoPendiente => Math.Max(0, Total - Pagos.Sum(pago => pago.Monto));
}

public class DetalleVenta
{
    public int ProductoId { get; set; }
    public string Producto { get; set; } = "";
    public string Edicion { get; set; } = "Estándar";
    public int Cantidad { get; set; }
    public double PrecioUnitario { get; set; }
    public double Subtotal { get; set; }
}
