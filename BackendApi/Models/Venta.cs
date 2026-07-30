using System.ComponentModel.DataAnnotations;

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
    [Range(0, double.MaxValue)]
    public double Subtotal { get; set; }
    [Range(0, double.MaxValue)]
    public double Descuento { get; set; }
    public double Total { get; set; }
    public string Estado { get; set; } = "Pendiente";
    [MinLength(1, ErrorMessage = "Agrega al menos un producto.")]
    public List<DetalleVenta> Detalles { get; set; } = [];
    public List<Pago> Pagos { get; set; } = [];
    public double SaldoPendiente => Math.Max(0, Total - Pagos.Sum(pago => pago.Monto));
}

public class DetalleVenta
{
    [Range(1, int.MaxValue)]
    public int ProductoId { get; set; }
    public string Producto { get; set; } = "";
    public string Edicion { get; set; } = "Estándar";
    [Range(1, int.MaxValue)]
    public int Cantidad { get; set; }
    [Range(0.01, double.MaxValue)]
    public double PrecioUnitario { get; set; }
    public double Subtotal { get; set; }
}
