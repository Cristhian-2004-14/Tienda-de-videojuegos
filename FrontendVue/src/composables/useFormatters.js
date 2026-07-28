const moneda = new Intl.NumberFormat('es-BO', {
  style: 'currency',
  currency: 'USD',
  minimumFractionDigits: 2,
});

export function formatearDinero(valor) {
  return moneda.format(Number(valor || 0));
}

export function formatearFecha(fecha) {
  if (!fecha) return 'Sin fecha';
  return new Intl.DateTimeFormat('es-BO', { dateStyle: 'medium' }).format(new Date(fecha));
}

export function formatearFechaHora(fecha) {
  if (!fecha) return 'Sin fecha';
  return new Intl.DateTimeFormat('es-BO', {
    dateStyle: 'medium',
    timeStyle: 'short',
  }).format(new Date(fecha));
}

export function formatearCodigo(prefijo, id, longitud = 4) {
  return `#${prefijo}-${String(id).padStart(longitud, '0')}`;
}
