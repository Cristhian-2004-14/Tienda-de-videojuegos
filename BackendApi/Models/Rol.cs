namespace BackendApi.Models;

public class Rol : IEntidad
{
    public int Id { get; set; }
    public string Nombre { get; set; } = "";
    public string Descripcion { get; set; } = "";
    public List<string> Permisos { get; set; } = [];
    public bool Protegido { get; set; }
}
