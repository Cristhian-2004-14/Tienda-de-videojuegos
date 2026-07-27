// Repository: capa de acceso a datos mock de Venta (y su Detalle Venta asociado).
// TODO: reemplazar por fetch a /api/venta cuando el backend esté listo.
export const ventas = [
  {
    id: 1,
    cliente: 'Alex Rivera',
    fecha: '2026-07-15',
    total: 179.99,
    estado: 'Completada',
    detalle: [{ productoId: 1, cantidad: 1, precioUnitario: 179.99 }],
  },
  {
    id: 2,
    cliente: 'Sarah Connor',
    fecha: '2026-07-16',
    total: 499.0,
    estado: 'Completada',
    detalle: [{ productoId: 1, cantidad: 1, precioUnitario: 499.0 }],
  },
  {
    id: 3,
    cliente: 'Marcus Fenix',
    fecha: '2026-07-17',
    total: 24.5,
    estado: 'Pendiente',
    detalle: [{ productoId: 3, cantidad: 1, precioUnitario: 24.5 }],
  },
  {
    id: 4,
    cliente: 'Jill Valentine',
    fecha: '2026-07-18',
    total: 39.99,
    estado: 'Completada',
    detalle: [{ productoId: 5, cantidad: 1, precioUnitario: 39.99 }],
  },
  {
    id: 5,
    cliente: 'Ethan Winters',
    fecha: '2026-07-19',
    total: 120.0,
    estado: 'Cancelada',
    detalle: [{ productoId: 2, cantidad: 1, precioUnitario: 120.0 }],
  },
];
