# CLAUDE.md — Sistema de Ventas y Servicio Técnico (Frontend)

Este archivo guía a Claude Code al trabajar en este repositorio. Léelo por completo antes de generar o modificar código.

## 1. Contexto del proyecto

Sistema de gestión de ventas y servicio técnico para una tienda de videojuegos y consolas, con dos módulos:

- **Panel administrativo** (uso interno: administrador, vendedor, técnico)
- **Tienda online** (uso de clientes)

Este repositorio contiene **solo el frontend** por ahora. La conexión a base de datos y el backend (ASP.NET Core Web API, proyecto `BackendApi`) se integrarán en una fase posterior — de momento todas las pantallas deben construirse con **datos de ejemplo (mock data)** definidos en el propio frontend, sin llamadas reales a la API.

## 2. Stack tecnológico (bloqueado — no cambiar)

- **Vue 3** (Composition API con `<script setup>`)
- **Vite** como bundler y servidor de desarrollo
- **Vue Router** para el enrutamiento entre pantallas
- **Pinia** para manejo de estado global (carrito, sesión de usuario, filtros)
- JavaScript (no TypeScript, salvo que se indique lo contrario)

No introduzcas otras librerías de manejo de estado (Vuex, Redux) ni otros bundlers. No agregues TypeScript sin confirmación previa.

## 3. Estructura de carpetas

Sigue esta estructura, inspirada en el repositorio de referencia (`proyecto-progra-web-2`):

```
/
├── public/
├── src/
│   ├── assets/              # imágenes, wireframes, mockups, íconos
│   ├── components/          # componentes reutilizables (botones, cards, tablas, modales)
│   ├── views/                # una vista por pantalla (ver sección 5)
│   ├── router/
│   │   └── index.js
│   ├── stores/                # Pinia stores (carrito.js, auth.js, productos.js...)
│   ├── data/                  # datos de ejemplo (mock) en formato JS/JSON
│   ├── App.vue
│   └── main.js
├── index.html
├── jsconfig.json
├── package.json
├── vite.config.js
└── .gitignore
```

No crees carpetas fuera de esta estructura sin justificarlo primero.

## 4. Datos de ejemplo (mock data)

Mientras no haya backend conectado:

- Todos los datos (productos, clientes, ventas, servicios, usuarios) viven en archivos dentro de `src/data/` como arrays exportados (ej. `productos.js`, `clientes.js`, `ventas.js`, `servicios.js`).
- Los componentes y vistas consumen estos datos importándolos directamente o a través de un store de Pinia que los expone.
- Estructura cada mock siguiendo los mismos campos que ya están definidos en el diccionario de clases del proyecto (Producto, Cliente, Venta, Detalle Venta, Servicio, Detalle Servicio, Usuario).
- Dejá comentado en cada store un bloque indicando dónde iría la llamada real a la API (`// TODO: reemplazar por fetch a /api/producto cuando el backend esté listo`), para facilitar la migración futura.

## 5. Pantallas del MVP (11 vistas)

### Panel administrativo
| Vista | Ruta sugerida | Archivo |
|---|---|---|
| Login | `/login` | `views/LoginView.vue` |
| Dashboard | `/admin` | `views/admin/DashboardView.vue` |
| Listado de productos | `/admin/productos` | `views/admin/ProductosListView.vue` |
| Formulario de producto | `/admin/productos/nuevo`, `/admin/productos/:id/editar` | `views/admin/ProductoFormView.vue` |
| Listado de clientes | `/admin/clientes` | `views/admin/ClientesListView.vue` |
| Formulario de cliente | `/admin/clientes/nuevo`, `/admin/clientes/:id/editar` | `views/admin/ClienteFormView.vue` |
| Registrar venta (POS) | `/admin/ventas/nueva` | `views/admin/VentaPosView.vue` |
| Servicio técnico (listado + detalle) | `/admin/servicios`, `/admin/servicios/:id` | `views/admin/ServiciosListView.vue`, `views/admin/ServicioDetalleView.vue` |

### Tienda online
| Vista | Ruta sugerida | Archivo |
|---|---|---|
| Catálogo de productos | `/tienda` | `views/tienda/CatalogoView.vue` |
| Detalle de producto | `/tienda/producto/:id` | `views/tienda/ProductoDetalleView.vue` |
| Carrito / Checkout | `/tienda/carrito` | `views/tienda/CarritoView.vue` |
| Login/Registro de cliente | `/tienda/login` | `views/tienda/ClienteLoginView.vue` |
| Seguimiento de servicio | `/tienda/servicio/:id` | `views/tienda/SeguimientoServicioView.vue` |

## 6. Referencia visual

Los wireframes y mockups de cada pantalla (generados en Stitch, estilo inspirado en Xbox: fondo oscuro, acento verde, diseño limpio y poco sobrecargado) están en `diseños` . Antes de construir una vista, revisa la imagen correspondiente en esa carpeta si existe, y sigue su jerarquía visual y disposición de elementos.

Nomenclatura de archivos de referencia:
```
src/assets/wireframes/wireframe-[nombre-vista].png
src/assets/mockups/mockup-[nombre-vista].png
```


## 7. Convenciones de código

- Componentes en PascalCase (`ProductoCard.vue`, `TablaClientes.vue`).
- Un componente por archivo, con `<script setup>` arriba, `<template>` en medio, `<style scoped>` al final.
- Props tipadas con `defineProps` y valores por defecto explícitos.
- Nombres de variables y funciones en español, consistente con el resto del proyecto (`obtenerProductos`, `calcularTotal`, `estadoServicio`).
- Evitar lógica de negocio dentro de los componentes de vista: extraerla a composables (`src/composables/`) cuando se repita en más de una vista.

## 8. Patrones de diseño a aplicar

Este proyecto es parte de un curso con requisitos de patrones de diseño. Aplica estos, y documenta con un comentario breve dónde se usa cada uno:

| Patrón | Dónde aplicarlo |
|---|---|
| **Singleton** | Store de sesión de usuario (Pinia ya lo garantiza por diseño, pero documentarlo) |
| **Factory** | Creación de objetos de "detalle" (DetalleVenta, DetalleServicio) a partir de un producto seleccionado |
| **Observer** | Reactividad de Vue/Pinia entre el carrito y el resumen de total (ya nativo, documentar el uso) |
| **Strategy** | Cálculo de totales o descuentos si se manejan distintas reglas (ej. venta normal vs. venta con descuento) |
| **Repository** | Capa `src/data/` o composables que abstraen el acceso a los datos mock, para que el día que se conecte la API real solo cambie esa capa |

## 9. Qué NO hacer todavía

- No conectar a ninguna API real ni base de datos.
- No implementar autenticación real (JWT, hashing) — el login de esta fase solo valida contra los usuarios mock y redirige.
- No instalar librerías de UI pesadas (Vuetify, Element Plus) salvo que se pida explícitamente — priorizar CSS propio simple, acorde al estilo Xbox definido en los mockups.

## 10. Comandos del proyecto

```bash
npm install       # instalar dependencias
npm run dev       # levantar servidor de desarrollo
npm run build     # compilar para producción
```
