using BackendApi.Data;
using BackendApi.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace BackendApi.Controllers;

[ApiController]
[Route("api/database")]
public class DatabaseController(
    IFirestoreRepository<Cliente> clientes,
    IFirestoreRepository<Producto> productos,
    IFirestoreRepository<Venta> ventas,
    IFirestoreRepository<Servicio> servicios,
    IFirestoreRepository<Usuario> usuarios,
    IFirestoreRepository<Empleado> empleados,
    IFirestoreRepository<Rol> roles,
    IFirestoreRepository<Proveedor> proveedores,
    IFirestoreRepository<Compra> compras,
    IFirestoreRepository<Dispositivo> dispositivos,
    IPasswordHasher<Usuario> passwordHasher) : ControllerBase
{
    [HttpPost("seed")]
    public async Task<IActionResult> CrearColecciones()
    {
        var creados = new List<string>();

        if ((await roles.ObtenerTodosAsync()).Count == 0)
        {
            foreach (var rol in DatosIniciales.Roles) await roles.CrearAsync(rol);
            creados.Add("roles");
        }

        if ((await empleados.ObtenerTodosAsync()).Count == 0)
        {
            foreach (var empleado in DatosIniciales.Empleados) await empleados.CrearAsync(empleado);
            creados.Add("empleados");
        }

        if ((await proveedores.ObtenerTodosAsync()).Count == 0)
        {
            foreach (var proveedor in DatosIniciales.Proveedores) await proveedores.CrearAsync(proveedor);
            creados.Add("proveedores");
        }

        if ((await dispositivos.ObtenerTodosAsync()).Count == 0)
        {
            foreach (var dispositivo in DatosIniciales.Dispositivos) await dispositivos.CrearAsync(dispositivo);
            creados.Add("dispositivos");
        }

        var comprasExistentes = await compras.ObtenerTodosAsync();
        if (comprasExistentes.Count == 0)
        {
            foreach (var compra in DatosIniciales.Compras) await compras.CrearAsync(compra);
            creados.Add("compras");
        }
        else if (comprasExistentes.Count < 2)
        {
            await compras.CrearAsync(DatosIniciales.Compras[1]);
            creados.Add("compra demostrativa");
        }

        if ((await clientes.ObtenerTodosAsync()).Count == 0)
        {
            foreach (var cliente in DatosIniciales.Clientes) await clientes.CrearAsync(cliente);
            creados.Add("clientes");
        }

        var productosExistentes = await productos.ObtenerTodosAsync();
        var nombresExistentes = productosExistentes
            .Select(producto => producto.Nombre)
            .ToHashSet(StringComparer.OrdinalIgnoreCase);
        var productosNuevos = DatosIniciales.Productos
            .Where(producto => !nombresExistentes.Contains(producto.Nombre))
            .ToArray();
        if (productosNuevos.Length > 0)
        {
            foreach (var producto in productosNuevos) await productos.CrearAsync(producto);
            creados.Add($"productos ({productosNuevos.Length} nuevos)");
        }
        foreach (var producto in productosExistentes)
        {
            var requiereMigracion = false;
            if (string.IsNullOrWhiteSpace(producto.Descripcion))
            {
                producto.Descripcion = $"{producto.Nombre} - {producto.Categoria} de {producto.Marca}.";
                requiereMigracion = true;
            }
            if (producto.PrecioCompra <= 0)
            {
                producto.PrecioCompra = Math.Round(producto.PrecioVenta * 0.7, 2);
                requiereMigracion = true;
            }
            if (producto.StockMinimo <= 0)
            {
                producto.StockMinimo = 5;
                requiereMigracion = true;
            }
            if (requiereMigracion) await productos.ActualizarAsync(producto.Id, producto);
        }

        var ventasExistentes = await ventas.ObtenerTodosAsync();
        if (ventasExistentes.Count == 0)
        {
            foreach (var venta in DatosIniciales.Ventas) await ventas.CrearAsync(venta);
            creados.Add("ventas");
        }
        else
        {
            if (!ventasExistentes.Any(venta => venta.Estado == "Pendiente"))
            {
                await ventas.CrearAsync(DatosIniciales.Ventas[1]);
                creados.Add("venta pendiente demostrativa");
            }
            if (!ventasExistentes.Any(venta => venta.Estado == "Completada" && venta.Pagos.Count > 0))
            {
                await ventas.CrearAsync(DatosIniciales.Ventas[0]);
                creados.Add("venta pagada demostrativa");
            }
        }

        var serviciosExistentes = await servicios.ObtenerTodosAsync();
        if (serviciosExistentes.Count == 0)
        {
            foreach (var servicio in DatosIniciales.Servicios) await servicios.CrearAsync(servicio);
            creados.Add("servicios");
        }
        else
        {
            foreach (var servicio in serviciosExistentes)
            {
                var estadoAnterior = servicio.Estado;
                servicio.Estado = servicio.Estado switch
                {
                    "En proceso" or "Esperando repuestos" => "En reparación",
                    "En pruebas" => "En pruebas",
                    _ => servicio.Estado,
                };
                if (servicio.Estado != estadoAnterior) await servicios.ActualizarAsync(servicio.Id, servicio);
            }
        }

        var usuariosExistentes = await usuarios.ObtenerTodosAsync();
        if (usuariosExistentes.Count == 0)
        {
            foreach (var usuario in DatosIniciales.Usuarios)
            {
                usuario.Password = passwordHasher.HashPassword(usuario, usuario.Password);
                await usuarios.CrearAsync(usuario);
            }
            creados.Add("usuarios");
        }
        else
        {
            foreach (var usuario in usuariosExistentes)
            {
                var inicial = DatosIniciales.Usuarios.FirstOrDefault(item =>
                    item.Username.Equals(usuario.Username, StringComparison.OrdinalIgnoreCase));
                if (inicial is null) continue;
                var requiereMigracion =
                    usuario.EmpleadoId != inicial.EmpleadoId ||
                    usuario.RolId != inicial.RolId ||
                    usuario.Nombre != inicial.Nombre ||
                    usuario.Rol != inicial.Rol;
                if (!requiereMigracion) continue;
                usuario.EmpleadoId = inicial.EmpleadoId;
                usuario.RolId = inicial.RolId;
                usuario.Nombre = inicial.Nombre;
                usuario.Rol = inicial.Rol;
                await usuarios.ActualizarAsync(usuario.Id, usuario);
                creados.Add($"usuario {usuario.Username} migrado");
            }
        }

        return Ok(new
        {
            message = creados.Count == 0
                ? "Las colecciones ya contenían datos."
                : "Colecciones inicializadas correctamente.",
            coleccionesCreadas = creados,
        });
    }
}

internal static class DatosIniciales
{
    internal static readonly Rol[] Roles =
    [
        new() { Nombre = "Administrador", Descripcion = "Control total del negocio", Protegido = true, Permisos = ["dashboard", "productos", "ventas", "servicios", "clientes", "compras", "personal", "reportes", "roles"] },
        new() { Nombre = "Vendedor", Descripcion = "Ventas, pagos y clientes", Permisos = ["productos", "ventas", "clientes"] },
        new() { Nombre = "Técnico", Descripcion = "Recepción y seguimiento técnico", Permisos = ["servicios", "clientes", "productos"] },
    ];

    internal static readonly Empleado[] Empleados =
    [
        new() { Nombre = "Ana", Apellido = "Torres", Ci = "7845123", Telefono = "69000001", Email = "ana@xstore.bo", Cargo = "Administrador", Salario = 5500 },
        new() { Nombre = "Juan", Apellido = "Pérez", Ci = "7845124", Telefono = "69000002", Email = "juan@xstore.bo", Cargo = "Vendedor", Salario = 3200 },
        new() { Nombre = "María", Apellido = "Rodríguez", Ci = "7845125", Telefono = "69000003", Email = "maria@xstore.bo", Cargo = "Técnico", Salario = 3500 },
    ];

    internal static readonly Proveedor[] Proveedores =
    [
        new() { RazonSocial = "Distribuidora Andina", Nit = "1029384012", Telefono = "77012001", Email = "ventas@andina.bo", Direccion = "Santa Cruz, Bolivia" },
        new() { RazonSocial = "Game Supply Bolivia", Nit = "4829301028", Telefono = "72188042", Email = "pedidos@gamesupply.bo", Direccion = "La Paz, Bolivia" },
    ];

    internal static readonly Dispositivo[] Dispositivos =
    [
        new() { ClienteId = 1, Cliente = "Alex Rivera", Tipo = "Consola", Marca = "Microsoft", Modelo = "Xbox Series X", NumeroSerie = "XBX-84A2-109", Observaciones = "Ingresa sin cable HDMI" },
        new() { ClienteId = 2, Cliente = "Sarah Connor", Tipo = "Control", Marca = "Sony", Modelo = "DualSense", NumeroSerie = "DS5-22109", Observaciones = "Stick izquierdo con deriva" },
    ];

    internal static readonly Compra[] Compras =
    [
        new()
        {
            ProveedorId = 1, Proveedor = "Distribuidora Andina", EmpleadoId = 1,
            Empleado = "Ana Torres", Total = 2450, Estado = "Recibida",
            Detalles = [new() { ProductoId = 1, Producto = "Xbox Series X", Cantidad = 5, PrecioUnitario = 490, Subtotal = 2450 }],
        },
        new()
        {
            ProveedorId = 2, Proveedor = "Game Supply Bolivia", EmpleadoId = 1,
            Empleado = "Ana Torres", Total = 720, Estado = "Recibida",
            Detalles =
            [
                new() { ProductoId = 3, Producto = "Elden Ring", Cantidad = 8, PrecioUnitario = 55, Subtotal = 440 },
                new() { ProductoId = 5, Producto = "Cable HDMI 2.1 4K", Cantidad = 10, PrecioUnitario = 28, Subtotal = 280 },
            ],
        },
    ];

    internal static readonly Cliente[] Clientes =
    [
        new() { Nombre = "Alex", Apellido = "Rivera", Telefono = "8888-1234", Email = "alex.rivera@correo.com" },
        new() { Nombre = "Sarah", Apellido = "Connor", Telefono = "8888-5678", Email = "sarah.connor@correo.com" },
        new() { Nombre = "Marcus", Apellido = "Fenix", Telefono = "8888-9012", Email = "marcus.fenix@correo.com" },
        new() { Nombre = "Jill", Apellido = "Valentine", Telefono = "8888-3456", Email = "jill.valentine@correo.com" },
        new() { Nombre = "Ethan", Apellido = "Winters", Telefono = "8888-7890", Email = "ethan.winters@correo.com" },
    ];

    internal static readonly Producto[] Productos =
    [
        new() { Nombre = "Xbox Series X", Categoria = "Consolas", Marca = "Microsoft", PrecioVenta = 499.99, Stock = 24 },
        new() { Nombre = "DualSense Controller", Categoria = "Accesorios", Marca = "Sony", PrecioVenta = 69.99, Stock = 3 },
        new() { Nombre = "Elden Ring", Categoria = "Videojuegos", Marca = "Bandai Namco", PrecioVenta = 59.99, Stock = 112 },
        new() { Nombre = "Halo Infinite", Categoria = "Videojuegos", Marca = "Xbox Game Studios", PrecioVenta = 59.99, Stock = 0 },
        new() { Nombre = "Cable HDMI 2.1 4K", Categoria = "Accesorios", Marca = "Genérico", PrecioVenta = 39.99, Stock = 87 },
        new() { Nombre = "PlayStation 5 Slim", Categoria = "Consolas", Marca = "Sony", PrecioVenta = 549.99, Stock = 12 },
        new() { Nombre = "Nintendo Switch OLED", Categoria = "Consolas", Marca = "Nintendo", PrecioVenta = 349.99, Stock = 8 },
        new() { Nombre = "Xbox Series S 1TB", Categoria = "Consolas", Marca = "Microsoft", PrecioVenta = 379.99, Stock = 6 },
        new() { Nombre = "The Legend of Zelda: Tears of the Kingdom", Categoria = "Videojuegos", Marca = "Nintendo", PrecioVenta = 69.99, Stock = 18 },
        new() { Nombre = "God of War Ragnarök", Categoria = "Videojuegos", Marca = "PlayStation Studios", PrecioVenta = 59.99, Stock = 9 },
        new() { Nombre = "Forza Horizon 5", Categoria = "Videojuegos", Marca = "Xbox Game Studios", PrecioVenta = 49.99, Stock = 15 },
        new() { Nombre = "EA Sports FC 26", Categoria = "Videojuegos", Marca = "Electronic Arts", PrecioVenta = 69.99, Stock = 4 },
        new() { Nombre = "Control Xbox Wireless", Categoria = "Accesorios", Marca = "Microsoft", PrecioVenta = 64.99, Stock = 22 },
        new() { Nombre = "Headset HyperX Cloud III", Categoria = "Accesorios", Marca = "HyperX", PrecioVenta = 99.99, Stock = 7 },
        new() { Nombre = "SSD WD Black 1TB para PS5", Categoria = "Accesorios", Marca = "Western Digital", PrecioVenta = 119.99, Stock = 5 },
        new() { Nombre = "Base de carga DualSense", Categoria = "Accesorios", Marca = "Sony", PrecioVenta = 34.99, Stock = 11 },
        new() { Nombre = "Mario Kart 8 Deluxe", Categoria = "Videojuegos", Marca = "Nintendo", PrecioVenta = 59.99, Stock = 16 },
        new() { Nombre = "Mortal Kombat 1", Categoria = "Videojuegos", Marca = "Warner Bros. Games", PrecioVenta = 44.99, Stock = 2 },
    ];

    internal static readonly Venta[] Ventas =
    [
        new()
        {
            ClienteId = 1, Cliente = "Alex Rivera", EmpleadoId = 2, Empleado = "Juan Pérez",
            Fecha = new DateTime(2026, 7, 15, 0, 0, 0, DateTimeKind.Utc), Subtotal = 179.99, Total = 179.99,
            Estado = "Completada",
            Detalles = [new() { ProductoId = 3, Producto = "Elden Ring", Cantidad = 3, PrecioUnitario = 59.99, Subtotal = 179.97 }],
            Pagos = [new() { Id = 1, Monto = 179.99, MetodoPago = "QR", Referencia = "DEMO-001", Fecha = new DateTime(2026, 7, 15, 0, 5, 0, DateTimeKind.Utc) }],
        },
        new()
        {
            ClienteId = 2, Cliente = "Sarah Connor", EmpleadoId = 2, Empleado = "Juan Pérez",
            Fecha = new DateTime(2026, 7, 16, 0, 0, 0, DateTimeKind.Utc), Subtotal = 499, Total = 499,
            Estado = "Pendiente",
            Detalles = [new() { ProductoId = 1, Producto = "Xbox Series X", Cantidad = 1, PrecioUnitario = 499, Subtotal = 499 }],
            Pagos = [new() { Id = 1, Monto = 200, MetodoPago = "Efectivo", Fecha = new DateTime(2026, 7, 16, 0, 5, 0, DateTimeKind.Utc) }],
        },
    ];

    internal static readonly Servicio[] Servicios =
    [
        new() { ClienteId = 1, Cliente = "Alex Rivera", EmpleadoId = 3, Empleado = "María Rodríguez", DispositivoId = 1, Dispositivo = "Microsoft Xbox Series X", Estado = "En reparación", Diagnostico = "Limpieza y cambio de pasta térmica", CostoManoObra = 35 },
        new() { ClienteId = 2, Cliente = "Sarah Connor", EmpleadoId = 3, Empleado = "María Rodríguez", DispositivoId = 2, Dispositivo = "Sony DualSense", Estado = "Recibido", Diagnostico = "Deriva en el stick izquierdo", CostoManoObra = 20 },
        new() { ClienteId = 3, Cliente = "Marcus Fenix", EmpleadoId = 3, Empleado = "María Rodríguez", Dispositivo = "PlayStation 5", Estado = "Listo para entrega", Diagnostico = "Puerto HDMI reemplazado y probado", CostoManoObra = 55 },
    ];

    internal static readonly Usuario[] Usuarios =
    [
        new() { EmpleadoId = 1, RolId = 1, Username = "admin", Password = "admin123", Nombre = "Ana Torres", Rol = "Administrador" },
        new() { EmpleadoId = 2, RolId = 2, Username = "jperez", Password = "vendedor123", Nombre = "Juan Pérez", Rol = "Vendedor" },
        new() { EmpleadoId = 3, RolId = 3, Username = "mrodriguez", Password = "tecnico123", Nombre = "María Rodríguez", Rol = "Técnico" },
    ];
}
