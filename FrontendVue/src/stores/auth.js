import { defineStore } from 'pinia';
import { iniciarSesionApi } from '../services/recursosApi';

// Patrón Singleton: Pinia mantiene una única instancia compartida del estado
// de sesión. La validación de credenciales se delega a BackendApi con Axios.
export const useAuthStore = defineStore('auth', {
  state: () => ({
    usuarioActual: null,
  }),

  getters: {
    estaAutenticado: (state) => state.usuarioActual !== null,
    rolActual: (state) => state.usuarioActual?.rol ?? null,
  },

  actions: {
    async iniciarSesion(username, password) {
      try {
        this.usuarioActual = await iniciarSesionApi({ username, password });
        return true;
      } catch {
        this.usuarioActual = null;
        return false;
      }
    },

    cerrarSesion() {
      this.usuarioActual = null;
    },
  },
});
