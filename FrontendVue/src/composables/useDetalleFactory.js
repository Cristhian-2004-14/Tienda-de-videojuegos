// Patrón Factory: centraliza la creación de objetos "detalle" (DetalleVenta,
// DetalleServicio) a partir de un producto o de datos de servicio, para que
// el resto de la app no tenga que armar esa estructura a mano en cada vista.
export function crearDetalleVenta(producto, cantidad = 1) {
  return {
    productoId: producto.id,
    nombre: producto.nombre,
    categoria: producto.categoria,
    marca: producto.marca,
    edicion: producto.edicion || 'Estándar',
    cantidad,
    precioUnitario: producto.precioVenta,
  };
}

export function crearDetalleServicio({ cliente, dispositivo, diagnostico }) {
  return {
    cliente,
    dispositivo,
    diagnostico,
    estado: 'Pendiente',
  };
}
