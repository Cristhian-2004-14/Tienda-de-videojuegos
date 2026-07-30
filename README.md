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
- Firebase Analytics y Cloud Firestore para imágenes de productos

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

## Imágenes de productos

El formulario admite una imagen JPG, PNG o WebP por producto. El navegador la
redimensiona y comprime a WebP antes de enviarla. La imagen se guarda en
Firestore sin depender de Firebase Storage.

## Demostración

Consulta [GUIA_DEMOSTRACION.md](GUIA_DEMOSTRACION.md) para probar los recorridos
de compra, venta, pagos parciales y servicio técnico.

## Ejecución con Docker

Los archivos `BackendApi/Dockerfile`, `FrontendVue/Dockerfile`,
`FrontendVue/nginx.conf` y `compose.yaml` dejan la aplicación lista para
construirse como dos contenedores. Nginx sirve Vue y reenvía `/api` al backend,
por lo que el navegador no depende de una URL local fija.

```powershell
Copy-Item .env.example .env
# Completar FIREBASE_PROJECT_ID y FIREBASE_API_KEY en .env
docker compose config
docker compose up --build -d
docker compose ps
```

Aplicación: `http://localhost:8080`

El puerto público puede modificarse con `APP_PORT` en `.env`. El backend no se
expone directamente: Nginx sirve la aplicación y reenvía las solicitudes
`/api`. Ambos contenedores incluyen comprobaciones de salud y se reinician
automáticamente salvo que se detengan manualmente.

Comandos de operación:

```powershell
# Consultar registros
docker compose logs -f

# Reconstruir después de actualizar el código
docker compose up --build -d

# Detener y eliminar los contenedores
docker compose down
```

En producción, el backend recibe la configuración mediante
`Firebase__ProjectId` y `Firebase__ApiKey`. Antes del primer despliegue publica
las reglas incluidas en el repositorio:

```powershell
firebase deploy --only firestore:rules
```

Para instalar o actualizar el proyecto en un servidor Linux consulta
[DESPLIEGUE_VPS.md](DESPLIEGUE_VPS.md). El procedimiento conserva el archivo
`.env` del servidor y reconstruye los contenedores con un solo comando:

```bash
bash scripts/deploy-vps.sh
```
