import { defineStore } from 'pinia';
import { crearDetalleVenta } from '../composables/useDetalleFactory';
import { calcularTotal } from '../composables/useCalculoTotal';

// Patrón Observer: al ser un store de Pinia (reactivo con Vue), cualquier
// componente que lea `items` o `total` (ej. el resumen del carrito) se
// vuelve a renderizar automáticamente cuando estos cambian — no hace falta
// suscribirse manualmente, Vue/Pinia lo resuelven de forma nativa.
export const useCarritoStore = defineStore('carrito', {
  state: () => ({
    items: [],
    estrategiaDescuento: 'normal',
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
    agregarProducto(producto, cantidad = 1) {
      const existente = this.items.find((i) => i.productoId === producto.id);
      if (existente) {
        existente.cantidad += cantidad;
        return;
      }
      this.items.push(crearDetalleVenta(producto, cantidad));
    },

    quitarProducto(productoId) {
      this.items = this.items.filter((i) => i.productoId !== productoId);
    },

    cambiarCantidad(productoId, cantidad) {
      const item = this.items.find((i) => i.productoId === productoId);
      if (!item) return;
      if (cantidad <= 0) {
        this.quitarProducto(productoId);
        return;
      }
      item.cantidad = cantidad;
    },

    vaciarCarrito() {
      this.items = [];
    },
  },
});
