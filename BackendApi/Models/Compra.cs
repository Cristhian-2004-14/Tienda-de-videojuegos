namespace BackendApi.Models;

public class Compra : IEntidad
{
    public int Id { get; set; }
    public int ProveedorId { get; set; }
    public string Proveedor { get; set; } = "";
    public int EmpleadoId { get; set; }
    public string Empleado { get; set; } = "";
    public DateTime Fecha { get; set; } = DateTime.UtcNow;
    public double Total { get; set; }
    public string Estado { get; set; } = "Recibida";
    public List<DetalleCompra> Detalles { get; set; } = [];
}

public class DetalleCompra
{
    public int ProductoId { get; set; }
    public string Producto { get; set; } = "";
    public int Cantidad { get; set; }
    public double PrecioUnitario { get; set; }
    public double Subtotal { get; set; }
}
