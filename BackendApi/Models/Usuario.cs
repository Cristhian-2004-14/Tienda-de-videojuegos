namespace BackendApi.Models;

public class Usuario : IEntidad
{
    public int Id { get; set; }
    public string Username { get; set; } = "";
    public string Password { get; set; } = "";
    public string Nombre { get; set; } = "";
    public string Rol { get; set; } = "";
    public bool Activo { get; set; } = true;
}
