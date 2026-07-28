import { defineStore } from 'pinia';
import { compras, proveedores, roles } from '../data/operaciones';
import { usuarios } from '../data/usuarios';

// Singleton + Repository: Pinia mantiene una única fuente reactiva para los
// módulos mock que luego podrán migrarse a una API sin cambiar las vistas.
export const useOperacionesStore = defineStore('operaciones', {
  state: () => ({
    compras: structuredClone(compras),
    proveedores: structuredClone(proveedores),
    roles: structuredClone(roles),
    personal: usuarios.map((usuario, indice) => ({
      ...usuario,
      nombre: ['Ana Torres', 'Juan Pérez', 'María Rodríguez', 'Luis Gómez', 'Carla Castro'][indice],
      salario: indice ? 3200 : 5500,
      activo: true,
    })),
    comprobante: null,
  }),
  actions: {
    registrarCompra(compra) {
      const proveedor = this.proveedores.find((item) => item.id === Number(compra.proveedorId));
      this.compras.unshift({ ...compra, id: Date.now(), proveedor: proveedor?.nombre || 'Sin proveedor', estado: 'Recibida' });
    },
    agregarProveedor(datos) {
      const proveedor = { ...datos, id: Date.now(), activo: true };
      this.proveedores.push(proveedor);
      return proveedor;
    },
    guardarPersonal(persona) {
      this.personal.push({ ...persona, id: Date.now(), activo: true, password: 'Cambio123' });
    },
    cambiarEstadoPersonal(id) {
      const persona = this.personal.find((item) => item.id === id);
      if (persona) persona.activo = !persona.activo;
    },
    guardarRol(rol) {
      this.roles.push({ ...rol, id: Date.now(), protegido: false });
    },
    registrarPago(venta, datosPago) {
      this.comprobante = { numero: `CMP-${Date.now().toString().slice(-6)}`, venta, ...datosPago, fecha: new Date().toLocaleString('es-BO') };
      return this.comprobante;
    },
  },
});
