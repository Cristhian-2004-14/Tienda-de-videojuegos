# Estructura final de Firestore

Adaptación del diagrama de clases originalmente normalizado para SQL.

## Colecciones principales

- `clientes`: datos personales y fecha de registro.
- `empleados`: datos personales, cargo, salario, fecha de ingreso y estado.
- `usuarios`: credenciales; conserva `empleadoId`, `rolId`, nombre y rol desnormalizados.
- `roles`: nombre, descripción, permisos y protección del rol administrador.
- `productos`: descripción, categoría y marca desnormalizadas, precios, stock, stock mínimo y una miniatura de portada.
- `imagenesProductos`: imágenes WebP comprimidas, vinculadas mediante `productoId`; cada imagen se guarda en un documento independiente.
- `proveedores`: razón social, NIT y datos de contacto.
- `compras`: proveedor y empleado desnormalizados, total y `detalles` embebidos.
- `ventas`: cliente y empleado desnormalizados, `detalles` y `pagos` embebidos.
- `dispositivos`: dueño, tipo, marca, modelo, serie y observaciones.
- `servicios`: cliente, técnico y dispositivo desnormalizados, repuestos en `detalles` y `pagos` embebidos.

## Objetos embebidos

No son colecciones independientes porque siempre se consultan junto a su documento padre:

- `DetalleVenta[]` dentro de una venta.
- `DetalleCompra[]` dentro de una compra.
- `DetalleServicio[]` dentro de un servicio.
- `Pago[]` dentro de una venta o servicio.

## Decisiones de desnormalización

- Categoría y marca se guardan directamente en cada producto.
- Los nombres de cliente, empleado, proveedor, producto y dispositivo se copian en las transacciones junto con su ID.
- Esto conserva el historial aunque posteriormente cambie el nombre del documento relacionado.
- Los IDs permiten navegar o validar la relación cuando sea necesario.

## Migración

`POST /api/database/seed`:

1. Crea las colecciones que todavía no existan.
2. Agrega productos faltantes sin duplicar por nombre.
3. Completa en productos antiguos `descripcion`, `precioCompra` y `stockMinimo`.
4. No elimina documentos existentes.
