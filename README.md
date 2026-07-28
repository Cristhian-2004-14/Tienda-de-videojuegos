# X-Store — Tienda de videojuegos

Proyecto académico para la gestión de una tienda de videojuegos y servicio
técnico. Incluye una tienda pública para consultar productos y preparar pedidos
por WhatsApp, además de un panel administrativo conectado a Cloud Firestore.

## Tecnologías

- Vue 3, Vite y Vue Router
- Pinia para el estado global
- Axios para el consumo de la API
- ASP.NET Core Web API sobre .NET 10
- Google Cloud Firestore como base de datos NoSQL
- Firebase Analytics

## Funcionalidades

### Tienda pública

- Catálogo con búsqueda, filtros y detalle de productos
- Selección de edición y carrito persistente
- Preparación del pedido y envío por WhatsApp
- Consulta pública del estado de una reparación mediante orden y CI o teléfono
- Enlace de soporte por WhatsApp

> El sistema no procesa pagos en línea. El carrito prepara el pedido y los pagos
> solo se registran administrativamente como efectivo, QR, tarjeta o transferencia.

### Panel administrativo

- Clientes, productos, inventario y dispositivos
- Punto de venta con control de stock y descuentos
- Ventas, comprobantes y pagos parciales
- Proveedores, compras y actualización automática del inventario
- Órdenes de servicio técnico, diagnósticos, repuestos, estados y pagos
- Personal, usuarios, roles y permisos
- Reportes de ventas, compras, pagos, servicios y productos con stock bajo

## Arquitectura

```text
Vue 3 + Pinia
       |
     Axios
       |
ASP.NET Core API
       |
Cloud Firestore
```

El frontend utiliza componentes reutilizables para tablas, encabezados, estados,
pagos y líneas de productos. El backend organiza el dominio mediante modelos,
controladores REST y un repositorio genérico para Firestore.

## Estructura

```text
TiendaDeVideoJuegos/
├── FrontendVue/
│   └── src/
│       ├── components/
│       │   ├── common/       # Encabezados, tablas, estados y estados vacíos
│       │   ├── inventory/    # Edición de líneas de productos
│       │   └── payments/     # Formularios, resúmenes e historial de pagos
│       ├── composables/      # Formatos, cálculos y factories
│       ├── router/           # Rutas públicas, administrativas y guardas
│       ├── services/         # Cliente Axios y recursos REST
│       ├── stores/           # Autenticación, carrito, datos y notificaciones
│       └── views/            # Tienda pública y panel administrativo
├── BackendApi/
│   ├── Controllers/          # Endpoints REST y datos iniciales
│   ├── Data/                 # Repositorio de Firestore
│   ├── Models/               # Entidades del dominio
│   └── Program.cs
├── GUIA_DEMOSTRACION.md
└── TiendaDeVideoJuegos.slnx
```

## Instalación y ejecución

### Backend

```powershell
dotnet restore
dotnet run --project BackendApi
```

API local: `http://localhost:5158`

### Frontend

```powershell
cd FrontendVue
npm install
npm run dev
```

Aplicación local: `http://localhost:5173`

## Inicializar datos de demostración

Con el backend iniciado:

```powershell
Invoke-RestMethod -Method Post http://localhost:5158/api/database/seed
```

El seed es idempotente y prepara clientes, productos, proveedores, compras,
ventas, servicios y usuarios para demostrar los flujos principales.

Usuarios iniciales:

| Rol | Usuario | Contraseña |
|---|---|---|
| Administrador | `admin` | `admin123` |
| Vendedor | `jperez` | `vendedor123` |
| Técnico | `mrodriguez` | `tecnico123` |

## Colecciones de Firestore

`clientes`, `productos`, `ventas`, `servicios`, `usuarios`, `empleados`,
`roles`, `proveedores`, `compras` y `dispositivos`.

La configuración de Firebase se encuentra en `BackendApi/appsettings.json`.
Para una instalación diferente se deben reemplazar `ProjectId` y `ApiKey`.

## Demostración

Consulta [GUIA_DEMOSTRACION.md](GUIA_DEMOSTRACION.md) para probar los recorridos
de compra, venta, pagos parciales y servicio técnico.
