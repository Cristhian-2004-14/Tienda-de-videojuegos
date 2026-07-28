import { defineStore } from 'pinia';

export const useNotificacionesStore = defineStore('notificaciones', {
  state: () => ({ mensaje: '', tipo: 'exito', id: 0 }),
  actions: {
    mostrar(mensaje, tipo = 'exito') {
      this.mensaje = mensaje;
      this.tipo = tipo;
      this.id += 1;
    },
    limpiar() {
      this.mensaje = '';
    },
  },
});
