using System.ComponentModel.DataAnnotations;

namespace BackendApi.Models;

public class Producto : IEntidad
{
    public int Id { get; set; }
    [Required(ErrorMessage = "El nombre del producto es obligatorio.")]
    [StringLength(120, MinimumLength = 2, ErrorMessage = "El nombre debe tener entre 2 y 120 caracteres.")]
    public string Nombre { get; set; } = "";
    [StringLength(1000, ErrorMessage = "La descripción no puede superar 1000 caracteres.")]
    public string Descripcion { get; set; } = "";
    [Required(ErrorMessage = "La categoría es obligatoria.")]
    public string Categoria { get; set; } = "";
    [Required(ErrorMessage = "La marca es obligatoria.")]
    [StringLength(80, ErrorMessage = "La marca no puede superar 80 caracteres.")]
    public string Marca { get; set; } = "";
    [Range(0, double.MaxValue, ErrorMessage = "El precio de compra no puede ser negativo.")]
    public double PrecioCompra { get; set; }
    [Range(0.01, double.MaxValue, ErrorMessage = "El precio de venta debe ser mayor que cero.")]
    public double PrecioVenta { get; set; }
    [Range(0, int.MaxValue, ErrorMessage = "El stock no puede ser negativo.")]
    public int Stock { get; set; }
    [Range(0, int.MaxValue, ErrorMessage = "El stock mínimo no puede ser negativo.")]
    public int StockMinimo { get; set; } = 5;
    public bool Activo { get; set; } = true;
    public List<ImagenProducto> Imagenes { get; set; } = [];
    // Compatibilidad con productos creados antes de habilitar la galería.
    public string ImagenUrl { get; set; } = "";
    public string ImagenStoragePath { get; set; } = "";
}

public class ImagenProducto
{
    public int Id { get; set; }
    public string Url { get; set; } = "";
    public string StoragePath { get; set; } = "";
}
