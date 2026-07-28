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
export const proveedoresApi = crearServicioRecurso('proveedores');
export const comprasApi = crearServicioRecurso('compras');
export const dispositivosApi = crearServicioRecurso('dispositivos');
export const empleadosApi = crearServicioRecurso('empleados');
export const rolesApi = crearServicioRecurso('roles');

export async function registrarPagoVentaApi(ventaId, pago) {
  const { data } = await api.post(`/ventas/${ventaId}/pagos`, pago);
  return data;
}

export async function anularVentaApi(ventaId) {
  const { data } = await api.put(`/ventas/${ventaId}/anular`);
  return data;
}

export async function actualizarSeguimientoServicioApi(servicioId, seguimiento) {
  const { data } = await api.put(`/servicios/${servicioId}/seguimiento`, seguimiento);
  return data;
}

export async function registrarPagoServicioApi(servicioId, pago) {
  const { data } = await api.post(`/servicios/${servicioId}/pagos`, pago);
  return data;
}

export async function consultarServicioPublicoApi(servicioId, verificacion) {
  const { data } = await api.get(`/servicios/consulta/${servicioId}`, { params: { verificacion } });
  return data;
}

export async function iniciarSesionApi(credenciales) {
  const { data } = await api.post('/auth/login', credenciales);
  return data;
}
