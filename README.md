# Tienda de videojuegos

El repositorio está dividido en dos aplicaciones independientes:

```text
TiendaDeVideoJuegos/
├── FrontendVue/                 # Interfaz Vue 3
│   ├── diseños/                 # Mockups y referencia visual
│   ├── public/
│   ├── src/
│   │   ├── components/
│   │   ├── composables/
│   │   ├── data/                # Datos mock temporales
│   │   ├── router/
│   │   ├── stores/
│   │   └── views/
│   ├── package.json
│   └── vite.config.js
├── BackendApi/                  # ASP.NET Core Web API
│   ├── Controllers/
│   ├── Data/                    # Repositorios de Cloud Firestore
│   ├── Models/
│   ├── Properties/
│   ├── Program.cs
│   └── BackendApi.csproj
└── TiendaDeVideoJuegos.slnx
```

## Frontend

```powershell
cd FrontendVue
npm install
npm run dev
```

URL predeterminada: `http://localhost:5173`.

## Backend

```powershell
dotnet run --project BackendApi
```

La especificación OpenAPI queda disponible en desarrollo en `/openapi/v1.json`.

## Firebase y Cloud Firestore

La comunicación sigue este flujo:

```text
Vue → Axios → BackendApi → Google Cloud Firestore
```

El proyecto Firebase configurado es `tienda-83288`. Firestore no utiliza tablas:
los datos se organizan en las colecciones `clientes`, `productos`, `ventas`,
`servicios` y `usuarios`.

La etapa actual utiliza la API REST con la clave web y las reglas de prueba de
Firestore. Con el backend iniciado, el inicializador se puede ejecutar de forma
idempotente:

```powershell
Invoke-RestMethod -Method Post http://localhost:5158/api/database/seed
```

> Advertencia: las reglas de prueba permiten que cualquier persona lea, modifique
> o elimine los datos. Antes de publicar la aplicación hay que cerrar las reglas
> y migrar BackendApi a una cuenta de servicio o a Google Application Default
> Credentials.

## API preparada

- `api/auth/login`
- `api/clientes`
- `api/productos`
- `api/ventas`
- `api/servicios`
- `api/usuarios`
- `api/database/seed`

La estructura sigue el patrón de
[MauroMelgar98/ApiBackend](https://github.com/MauroMelgar98/ApiBackend):
frontend y backend separados, modelos de dominio en `Models` y endpoints REST
mediante controladores en `Controllers`.
