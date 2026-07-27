import api from './api';

function crearServicioRecurso(recurso) {
  return {
    async obtenerTodos() {
      const { data } = await api.get(`/${recurso}`);
      return data;
    },
    async obtenerPorId(id) {
      const { data } = await api.get(`/${recurso}/${id}`);
      return data;
    },
    async crear(payload) {
      const { data } = await api.post(`/${recurso}`, payload);
      return data;
    },
    async actualizar(id, payload) {
      const { data } = await api.put(`/${recurso}/${id}`, payload);
      return data;
    },
    async eliminar(id) {
      await api.delete(`/${recurso}/${id}`);
    },
  };
}

export const clientesApi = crearServicioRecurso('clientes');
export const productosApi = crearServicioRecurso('productos');
export const ventasApi = crearServicioRecurso('ventas');
export const serviciosApi = crearServicioRecurso('servicios');
export const usuariosApi = crearServicioRecurso('usuarios');

export async function iniciarSesionApi(credenciales) {
  const { data } = await api.post('/auth/login', credenciales);
  return data;
}
