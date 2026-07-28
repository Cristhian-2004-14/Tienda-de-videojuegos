import { defineStore } from 'pinia';

// Singleton: estado compartido de búsqueda y filtros entre navbar y catálogo.
export const useTiendaUiStore = defineStore('tienda-ui', {
  state: () => ({ busqueda: '', categoria: 'Todo' }),
  actions: {
    seleccionarCategoria(categoria) {
      this.categoria = categoria;
    },
  },
});
