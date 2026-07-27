namespace BackendApi.Models;

public class Producto : IEntidad
{
    public int Id { get; set; }
    public string Nombre { get; set; } = "";
    public string Categoria { get; set; } = "";
    public string Marca { get; set; } = "";
    public double PrecioVenta { get; set; }
    public int Stock { get; set; }
}
