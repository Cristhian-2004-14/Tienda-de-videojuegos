namespace BackendApi.Models;

// Se embebe dentro de Venta o Servicio; no necesita colección independiente.
public class Pago : IEntidad
{
    public int Id { get; set; }
    public int? VentaId { get; set; }
    public int? ServicioId { get; set; }
    public double Monto { get; set; }
    public string MetodoPago { get; set; } = "";
    public string Referencia { get; set; } = "";
    public DateTime Fecha { get; set; } = DateTime.UtcNow;
}
