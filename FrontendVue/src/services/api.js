import axios from 'axios';

const api = axios.create({
  baseURL: import.meta.env.VITE_API_URL || 'http://localhost:5158/api',
  timeout: 10000,
  headers: {
    'Content-Type': 'application/json',
  },
});

api.interceptors.response.use(
  (respuesta) => respuesta,
  (error) => {
    const mensaje =
      error.response?.data?.message ||
      error.response?.data?.title ||
      'No se pudo conectar con BackendApi.';

    return Promise.reject(new Error(mensaje));
  }
);

export default api;
