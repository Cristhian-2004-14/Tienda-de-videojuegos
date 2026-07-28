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
    agregarProducto(producto, cantidad = 1) {
      const existente = this.items.find((i) => i.productoId === producto.id);
      if (existente) {
        existente.cantidad += cantidad;
        this.ultimoAgregado = producto.nombre;
        this.confirmacionId += 1;
        this.persistir();
        return;
      }
      this.items.push(crearDetalleVenta(producto, cantidad));
      this.ultimoAgregado = producto.nombre;
      this.confirmacionId += 1;
      this.persistir();
    },

    quitarProducto(productoId) {
      this.items = this.items.filter((i) => i.productoId !== productoId);
      this.persistir();
    },

    cambiarCantidad(productoId, cantidad) {
      const item = this.items.find((i) => i.productoId === productoId);
      if (!item) return;
      if (cantidad <= 0) {
        this.quitarProducto(productoId);
        return;
      }
      item.cantidad = cantidad;
      this.persistir();
    },

    vaciarCarrito() {
      this.items = [];
      this.persistir();
    },
  },
});
