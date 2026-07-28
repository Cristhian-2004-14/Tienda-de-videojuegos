namespace BackendApi.Models;

public class Proveedor : IEntidad
{
    public int Id { get; set; }
    public string RazonSocial { get; set; } = "";
    public string Nit { get; set; } = "";
    public string Telefono { get; set; } = "";
    public string Email { get; set; } = "";
    public string Direccion { get; set; } = "";
    public bool Activo { get; set; } = true;
}
