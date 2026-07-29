namespace BackendApi.Models;

public class ImagenProductoDocumento : IEntidad
{
    public int Id { get; set; }
    public int ProductoId { get; set; }
    public string Contenido { get; set; } = "";
    public string Miniatura { get; set; } = "";
    public string TipoMime { get; set; } = "image/webp";
    public int Orden { get; set; }
}
