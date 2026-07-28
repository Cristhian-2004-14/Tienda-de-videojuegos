import { defineStore } from 'pinia';
import { iniciarSesionApi } from '../services/recursosApi';

const CLAVE_SESION = 'xstore-sesion';
function leerSesion() {
  try { return JSON.parse(sessionStorage.getItem(CLAVE_SESION)) || null; }
  catch { return null; }
}

export const useAuthStore = defineStore('auth', {
  state: () => ({ usuarioActual: leerSesion() }),
  getters: {
    estaAutenticado: (state) => state.usuarioActual !== null,
    rolActual: (state) => state.usuarioActual?.rol ?? null,
    tienePermiso: (state) => (permiso) =>
      !permiso || state.usuarioActual?.rol === 'Administrador' || state.usuarioActual?.permisos?.includes(permiso),
  },
  actions: {
    async iniciarSesion(username, password) {
      try {
        this.usuarioActual = await iniciarSesionApi({ username, password });
        sessionStorage.setItem(CLAVE_SESION, JSON.stringify(this.usuarioActual));
        return true;
      } catch {
        this.cerrarSesion();
        return false;
      }
    },
    cerrarSesion() {
      this.usuarioActual = null;
      sessionStorage.removeItem(CLAVE_SESION);
    },
  },
});
