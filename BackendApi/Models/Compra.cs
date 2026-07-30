using System.ComponentModel.DataAnnotations;

namespace BackendApi.Models;

public class Compra : IEntidad
{
    public int Id { get; set; }
    [Range(1, int.MaxValue, ErrorMessage = "Selecciona un proveedor.")]
    public int ProveedorId { get; set; }
    public string Proveedor { get; set; } = "";
    public int EmpleadoId { get; set; }
    public string Empleado { get; set; } = "";
    public DateTime Fecha { get; set; } = DateTime.UtcNow;
    public double Total { get; set; }
    public string Estado { get; set; } = "Recibida";
    [MinLength(1, ErrorMessage = "Agrega al menos un producto.")]
    public List<DetalleCompra> Detalles { get; set; } = [];
}

public class DetalleCompra
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
