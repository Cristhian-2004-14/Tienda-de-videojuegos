namespace BackendApi.Models;

public class Venta : IEntidad
{
    public int Id { get; set; }
    public int ClienteId { get; set; }
    public string Cliente { get; set; } = "";
    public DateTime Fecha { get; set; } = DateTime.UtcNow;
    public double Total { get; set; }
    public string Estado { get; set; } = "Pendiente";
    public List<DetalleVenta> Detalles { get; set; } = [];
}

public class DetalleVenta
{
    public int Id { get; set; }
    public int ProductoId { get; set; }
    public int Cantidad { get; set; }
    public double PrecioUnitario { get; set; }
}
