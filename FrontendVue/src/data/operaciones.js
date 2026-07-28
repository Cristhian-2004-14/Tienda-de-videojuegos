// Repository: datos mock para los casos de uso complementarios.
// TODO: reemplazar por endpoints /api/compras, /api/proveedores y /api/roles.
export const proveedores = [
  { id: 1, nombre: 'Distribuidora Andina', nit: '1029384012', telefono: '770-12001', activo: true },
  { id: 2, nombre: 'Game Supply Bolivia', nit: '4829301028', telefono: '721-88042', activo: true },
];

export const compras = [
  { id: 1, proveedorId: 1, proveedor: 'Distribuidora Andina', fecha: '2026-07-20', total: 2450, estado: 'Recibida', items: 14 },
  { id: 2, proveedorId: 2, proveedor: 'Game Supply Bolivia', fecha: '2026-07-24', total: 980, estado: 'Pendiente', items: 8 },
];

export const roles = [
  { id: 1, nombre: 'Administrador', descripcion: 'Control total del negocio', protegido: true, permisos: ['dashboard', 'productos', 'ventas', 'servicios', 'clientes', 'compras', 'personal', 'reportes', 'roles'] },
  { id: 2, nombre: 'Vendedor', descripcion: 'Ventas, pagos y clientes', protegido: false, permisos: ['ventas', 'clientes', 'productos'] },
  { id: 3, nombre: 'Técnico', descripcion: 'Recepción y seguimiento técnico', protegido: false, permisos: ['servicios', 'clientes'] },
];
