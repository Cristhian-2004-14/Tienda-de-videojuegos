namespace BackendApi.Models;

public class Servicio : IEntidad
{
    public int Id { get; set; }
    public int ClienteId { get; set; }
    public string Cliente { get; set; } = "";
    public string Dispositivo { get; set; } = "";
    public string Estado { get; set; } = "Pendiente";
    public string Diagnostico { get; set; } = "";
    public DateTime FechaIngreso { get; set; } = DateTime.UtcNow;
    public List<DetalleServicio> Detalles { get; set; } = [];
}

public class DetalleServicio
{
    public int Id { get; set; }
    public string Descripcion { get; set; } = "";
    public double Costo { get; set; }
}
