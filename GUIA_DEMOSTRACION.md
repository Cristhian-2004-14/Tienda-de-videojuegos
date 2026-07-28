# Guía de demostración académica

## Preparación

1. Iniciar `BackendApi` y `FrontendVue`.
2. Ejecutar una vez `POST /api/database/seed`.
3. Ingresar como `admin` con la contraseña configurada en los datos iniciales.
4. Abrir dos pestañas: panel administrativo y tienda pública.

## Escenario 1: compra y aumento de stock

1. Anotar el stock actual de un producto.
2. Abrir **Compras → Registrar compra**.
3. Seleccionar proveedor, producto, cantidad y costo.
4. Confirmar el ingreso.
5. Volver a Productos y comprobar que el stock aumentó.

## Escenario 2: venta y reducción de stock

1. Abrir **Ventas → Nueva venta**.
2. Seleccionar cliente y productos.
3. Cambiar cantidades, aplicar un descuento y registrar la venta.
4. Mostrar el detalle y el comprobante.
5. Comprobar que el stock disminuyó.

## Escenario 3: abonos parciales

1. Abrir una venta pendiente.
2. Registrar un primer abono por efectivo, QR o tarjeta.
3. Mostrar el saldo restante.
4. Registrar el saldo final y comprobar que queda **Completada**.

## Escenario 4: servicio técnico y consulta pública

1. Registrar un dispositivo para un cliente con CI o teléfono.
2. Crear la orden y anotar su número `SRV`.
3. Agregar diagnóstico, repuesto, mano de obra y cambiar el estado.
4. Abrir `/tienda/servicio`.
5. Consultar con el número de orden y el CI o teléfono del cliente.
6. Mostrar el dispositivo, diagnóstico y estado desde la vista pública.

## Usuarios de demostración

- Administrador: acceso a todos los módulos.
- Vendedor: clientes, productos y ventas.
- Técnico: clientes, productos y servicio técnico.
