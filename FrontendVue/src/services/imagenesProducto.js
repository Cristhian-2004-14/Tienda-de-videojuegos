import api from './api';

const TIPOS_PERMITIDOS = ['image/jpeg', 'image/png', 'image/webp'];
const TAMANO_ORIGINAL_MAXIMO = 10 * 1024 * 1024;
const LONGITUD_BASE64_MAXIMA = 780_000;
const DIMENSION_MAXIMA = 1600;

export function validarImagenProducto(archivo) {
  if (!TIPOS_PERMITIDOS.includes(archivo.type)) {
    throw new Error('Selecciona una imagen JPG, PNG o WebP.');
  }
  if (archivo.size > TAMANO_ORIGINAL_MAXIMO) {
    throw new Error('La imagen original no puede superar los 10 MB.');
  }
}

function blobADataUrl(blob) {
  return new Promise((resolve, reject) => {
    const lector = new FileReader();
    lector.onload = () => resolve(lector.result);
    lector.onerror = () => reject(new Error('No se pudo leer la imagen.'));
    lector.readAsDataURL(blob);
  });
}

async function cargarBitmap(archivo) {
  if ('createImageBitmap' in window) return createImageBitmap(archivo);
  const url = URL.createObjectURL(archivo);
  try {
    return await new Promise((resolve, reject) => {
      const imagen = new Image();
      imagen.onload = () => resolve(imagen);
      imagen.onerror = () => reject(new Error('No se pudo procesar la imagen.'));
      imagen.src = url;
    });
  } finally {
    URL.revokeObjectURL(url);
  }
}

export async function comprimirImagenProducto(archivo) {
  validarImagenProducto(archivo);
  const imagen = await cargarBitmap(archivo);
  const escala = Math.min(1, DIMENSION_MAXIMA / Math.max(imagen.width, imagen.height));
  const canvas = document.createElement('canvas');
  canvas.width = Math.max(1, Math.round(imagen.width * escala));
  canvas.height = Math.max(1, Math.round(imagen.height * escala));
  canvas.getContext('2d').drawImage(imagen, 0, 0, canvas.width, canvas.height);
  if (typeof imagen.close === 'function') imagen.close();

  for (const calidad of [0.82, 0.7, 0.58, 0.46, 0.34]) {
    const blob = await new Promise((resolve) => canvas.toBlob(resolve, 'image/webp', calidad));
    if (!blob) continue;
    const contenido = await blobADataUrl(blob);
    if (contenido.length <= LONGITUD_BASE64_MAXIMA) {
      const escalaMiniatura = Math.min(1, 420 / Math.max(canvas.width, canvas.height));
      const miniaturaCanvas = document.createElement('canvas');
      miniaturaCanvas.width = Math.max(1, Math.round(canvas.width * escalaMiniatura));
      miniaturaCanvas.height = Math.max(1, Math.round(canvas.height * escalaMiniatura));
      miniaturaCanvas.getContext('2d').drawImage(
        canvas, 0, 0, miniaturaCanvas.width, miniaturaCanvas.height,
      );
      const miniaturaBlob = await new Promise((resolve) =>
        miniaturaCanvas.toBlob(resolve, 'image/webp', 0.68));
      const miniatura = await blobADataUrl(miniaturaBlob);
      return { contenido, miniatura, tipoMime: 'image/webp' };
    }
  }
  throw new Error('La imagen es demasiado compleja. Usa una imagen de menor resolución.');
}

export async function subirImagenesProducto(productoId, archivos, alProgresar = () => {}) {
  const lista = Array.from(archivos);
  const resultado = [];
  try {
    for (let indice = 0; indice < lista.length; indice += 1) {
      const preparada = await comprimirImagenProducto(lista[indice]);
      const { data } = await api.post(`/productos/${productoId}/imagenes`, preparada);
      resultado.push(data);
      alProgresar(Math.round(((indice + 1) / lista.length) * 100));
    }
    return resultado;
  } catch (error) {
    await Promise.allSettled(resultado.map((imagen) =>
      eliminarImagenProducto(productoId, imagen.id)));
    throw error;
  }
}

export async function eliminarImagenProducto(productoId, imagenId) {
  if (!imagenId) return;
  await api.delete(`/productos/${productoId}/imagenes/${imagenId}`);
}
