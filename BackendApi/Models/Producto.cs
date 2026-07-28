namespace BackendApi.Models;

public class Producto : IEntidad
{
    public int Id { get; set; }
    public string Nombre { get; set; } = "";
    public string Descripcion { get; set; } = "";
    public string Categoria { get; set; } = "";
    public string Marca { get; set; } = "";
    public double PrecioCompra { get; set; }
    public double PrecioVenta { get; set; }
    public int Stock { get; set; }
    public int StockMinimo { get; set; } = 5;
    public bool Activo { get; set; } = true;
}
