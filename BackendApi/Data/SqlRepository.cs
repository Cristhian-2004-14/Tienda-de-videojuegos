using BackendApi.Models;
using Microsoft.EntityFrameworkCore;

namespace BackendApi.Data;

public class SqlRepository<T>(AppDbContext context) : IRepository<T>
    where T : class, IEntidad
{
    private readonly DbSet<T> _dbSet = context.Set<T>();

    public async Task<IReadOnlyList<T>> ObtenerTodosAsync()
    {
        IQueryable<T> query = _dbSet.AsNoTracking();
        query = IncluirRelaciones(query);
        return await query.ToListAsync();
    }

    public async Task<T?> ObtenerPorIdAsync(int id)
    {
        IQueryable<T> query = _dbSet;
        query = IncluirRelaciones(query);
        return await query.FirstOrDefaultAsync(e => e.Id == id);
    }

    public async Task<T> CrearAsync(T entidad)
    {
        await _dbSet.AddAsync(entidad);
        await context.SaveChangesAsync();
        return entidad;
    }

    public async Task<T?> ActualizarAsync(int id, T entidad)
    {
        var existente = await _dbSet.FindAsync(id);
        if (existente is null) return null;

        context.Entry(existente).CurrentValues.SetValues(entidad);

        if (entidad is Venta venta && existente is Venta ventaExistente)
        {
            ventaExistente.Detalles = venta.Detalles;
            ventaExistente.Pagos = venta.Pagos;
            ventaExistente.Subtotal = venta.Subtotal;
            ventaExistente.Descuento = venta.Descuento;
            ventaExistente.Total = venta.Total;
            ventaExistente.Estado = venta.Estado;
        }
        else if (entidad is Servicio servicio && existente is Servicio servicioExistente)
        {
            servicioExistente.Detalles = servicio.Detalles;
            servicioExistente.Pagos = servicio.Pagos;
            servicioExistente.Estado = servicio.Estado;
            servicioExistente.Diagnostico = servicio.Diagnostico;
            servicioExistente.CostoManoObra = servicio.CostoManoObra;
            servicioExistente.FechaEntrega = servicio.FechaEntrega;
        }
        else if (entidad is Compra compra && existente is Compra compraExistente)
        {
            compraExistente.Detalles = compra.Detalles;
            compraExistente.Total = compra.Total;
            compraExistente.Estado = compra.Estado;
        }
        else if (entidad is Rol rol && existente is Rol rolExistente)
        {
            rolExistente.Permisos = rol.Permisos;
        }

        await context.SaveChangesAsync();
        return existente;
    }

    public async Task<bool> EliminarAsync(int id)
    {
        var existente = await _dbSet.FindAsync(id);
        if (existente is null) return false;

        _dbSet.Remove(existente);
        await context.SaveChangesAsync();
        return true;
    }

    private static IQueryable<T> IncluirRelaciones(IQueryable<T> query)
    {
        if (typeof(T) == typeof(Venta))
        {
            return (IQueryable<T>)(object)((IQueryable<Venta>)(object)query).Include(v => v.Detalles).Include(v => v.Pagos);
        }
        if (typeof(T) == typeof(Servicio))
        {
            return (IQueryable<T>)(object)((IQueryable<Servicio>)(object)query).Include(s => s.Detalles).Include(s => s.Pagos);
        }
        if (typeof(T) == typeof(Compra))
        {
            return (IQueryable<T>)(object)((IQueryable<Compra>)(object)query).Include(c => c.Detalles);
        }
        return query;
    }
}
