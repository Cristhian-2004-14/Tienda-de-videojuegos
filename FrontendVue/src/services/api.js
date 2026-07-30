import axios from 'axios';

const api = axios.create({
  baseURL: import.meta.env.VITE_API_URL || '/api',
  timeout: 10000,
  headers: {
    'Content-Type': 'application/json',
  },
});

api.interceptors.response.use(
  (respuesta) => respuesta,
  (error) => {
    const erroresValidacion = error.response?.data?.errors;
    const primerError = erroresValidacion
      ? Object.values(erroresValidacion).flat().find(Boolean)
      : null;
    const mensaje =
      primerError ||
      error.response?.data?.message ||
      error.response?.data?.title ||
      'No se pudo conectar con BackendApi.';

    const errorNormalizado = new Error(mensaje);
    errorNormalizado.status = error.response?.status;
    errorNormalizado.errores = erroresValidacion || null;
    return Promise.reject(errorNormalizado);
  }
);

export default api;
