import { defineStore } from 'pinia';
import { clientes as clientesMock } from '../data/clientes';
import { productos as productosMock } from '../data/productos';
import { servicios as serviciosMock } from '../data/servicios';
import { ventas as ventasMock } from '../data/ventas';
import {
  clientesApi,
  productosApi,
  serviciosApi,
  ventasApi,
} from '../services/recursosApi';

// Patrón Repository: el store centraliza el acceso HTTP y conserva datos mock
// como respaldo visual cuando BackendApi no está iniciado.
export const useDatosApiStore = defineStore('datos-api', {
  state: () => ({
    clientes: [...clientesMock],
    productos: [...productosMock],
    servicios: [...serviciosMock],
    ventas: [...ventasMock],
    cargando: false,
    error: null,
  }),

  actions: {
    async cargarRecurso(nombre, servicio) {
      try {
        const datos = await servicio.obtenerTodos();
        if (Array.isArray(datos)) this[nombre] = datos;
      } catch (error) {
        this.error = error.message;
      }
    },

    async cargarTodo() {
      this.cargando = true;
      this.error = null;
      await Promise.all([
        this.cargarRecurso('clientes', clientesApi),
        this.cargarRecurso('productos', productosApi),
        this.cargarRecurso('servicios', serviciosApi),
        this.cargarRecurso('ventas', ventasApi),
      ]);
      this.cargando = false;
    },

    async guardarCliente(cliente) {
      const guardado = cliente.id
        ? await clientesApi.actualizar(cliente.id, cliente)
        : await clientesApi.crear(cliente);
      await this.cargarRecurso('clientes', clientesApi);
      return guardado;
    },

    async eliminarCliente(id) {
      await clientesApi.eliminar(id);
      this.clientes = this.clientes.filter((cliente) => cliente.id !== id);
    },

    async guardarProducto(producto) {
      const guardado = producto.id
        ? await productosApi.actualizar(producto.id, producto)
        : await productosApi.crear(producto);
      await this.cargarRecurso('productos', productosApi);
      return guardado;
    },

    async eliminarProducto(id) {
      await productosApi.eliminar(id);
      this.productos = this.productos.filter((producto) => producto.id !== id);
    },

    async registrarVenta(venta) {
      const guardada = await ventasApi.crear(venta);
      this.ventas.unshift(guardada);
      await this.cargarRecurso('productos', productosApi);
      return guardada;
    },
  },
});
