namespace BackendApi.Models;

// Se embebe dentro de Venta o Servicio; no necesita colección independiente.
public class Pago
{
    public int Id { get; set; }
    public double Monto { get; set; }
    public string MetodoPago { get; set; } = "";
    public string Referencia { get; set; } = "";
    public DateTime Fecha { get; set; } = DateTime.UtcNow;
}
