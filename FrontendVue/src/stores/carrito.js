import { defineStore } from 'pinia';
import { crearDetalleVenta } from '../composables/useDetalleFactory';
import { calcularTotal } from '../composables/useCalculoTotal';
const CLAVE_CARRITO = 'xstore-carrito';
function cargarItems() {
  try { return JSON.parse(localStorage.getItem(CLAVE_CARRITO)) || []; }
  catch { return []; }
}

// Patrón Observer: al ser un store de Pinia (reactivo con Vue), cualquier
// componente que lea `items` o `total` (ej. el resumen del carrito) se
// vuelve a renderizar automáticamente cuando estos cambian — no hace falta
// suscribirse manualmente, Vue/Pinia lo resuelven de forma nativa.
export const useCarritoStore = defineStore('carrito', {
  state: () => ({
    items: cargarItems(),
    estrategiaDescuento: 'normal',
    ultimoAgregado: '',
    ultimoAviso: '',
    avisoTipo: 'exito',
    confirmacionId: 0,
  }),

  getters: {
    cantidadItems: (state) =>
      state.items.reduce((acc, item) => acc + item.cantidad, 0),

    subtotal: (state) =>
      state.items.reduce(
        (acc, item) => acc + item.precioUnitario * item.cantidad,
        0
      ),

    total() {
      return calcularTotal(this.subtotal, this.estrategiaDescuento);
    },
  },

  actions: {
    persistir() {
      localStorage.setItem(CLAVE_CARRITO, JSON.stringify(this.items));
    },
    avisar(mensaje, tipo = 'exito') {
      this.ultimoAviso = mensaje;
      this.avisoTipo = tipo;
      this.confirmacionId += 1;
    },
    agregarProducto(producto, cantidad = 1) {
      const stock = Math.max(0, Number(producto.stock) || 0);
      const unidades = Math.max(1, Math.trunc(Number(cantidad) || 1));
      const existente = this.items.find((i) => i.productoId === producto.id);
      const cantidadActual = existente?.cantidad || 0;
      if (stock === 0 || cantidadActual + unidades > stock) {
        this.avisar(stock === 0
          ? `${producto.nombre} está agotado.`
          : `Solo hay ${stock} unidades de ${producto.nombre}; ya tienes ${cantidadActual} en el carrito.`, 'error');
        return false;
      }
      if (existente) {
        existente.cantidad += unidades;
        existente.stockDisponible = stock;
        this.ultimoAgregado = producto.nombre;
        this.avisar(producto.nombre);
        this.persistir();
        return true;
      }
      this.items.push(crearDetalleVenta(producto, unidades));
      this.ultimoAgregado = producto.nombre;
      this.avisar(producto.nombre);
      this.persistir();
      return true;
    },

    quitarProducto(productoId) {
      this.items = this.items.filter((i) => i.productoId !== productoId);
      this.persistir();
    },

    cambiarCantidad(productoId, cantidad) {
      const item = this.items.find((i) => i.productoId === productoId);
      if (!item) return false;
      const nuevaCantidad = Math.trunc(Number(cantidad));
      if (nuevaCantidad <= 0) {
        this.quitarProducto(productoId);
        return true;
      }
      if (!Number.isFinite(nuevaCantidad) || nuevaCantidad > item.stockDisponible) {
        this.avisar(`No puedes agregar más de ${item.stockDisponible} unidades de ${item.nombre}.`, 'error');
        return false;
      }
      item.cantidad = nuevaCantidad;
      this.persistir();
      return true;
    },

    sincronizarStock(productos) {
      const porId = new Map(productos.map((producto) => [producto.id, producto]));
      let ajustado = false;
      this.items = this.items.flatMap((item) => {
        const producto = porId.get(item.productoId);
        if (!producto) return [item];
        const stock = Math.max(0, Number(producto.stock) || 0);
        if (stock === 0) {
          ajustado = true;
          return [];
        }
        const cantidad = Math.min(item.cantidad, stock);
        if (cantidad !== item.cantidad || item.stockDisponible !== stock) ajustado = true;
        return [{ ...item, cantidad, stockDisponible: stock }];
      });
      if (ajustado) {
        this.persistir();
        this.avisar('El carrito fue actualizado según el stock disponible.', 'error');
      }
    },

    vaciarCarrito() {
      this.items = [];
      this.persistir();
    },
  },
});
