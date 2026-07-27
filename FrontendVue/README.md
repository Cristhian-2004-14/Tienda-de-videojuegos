# FrontendVue

Aplicación de interfaz construida con Vue 3, Vite, Vue Router y Pinia.

## Ejecutar

```powershell
cd FrontendVue
npm install
npm run dev
```

La interfaz consume `BackendApi` mediante Axios. La URL predeterminada es
`http://localhost:5158/api` y puede cambiarse con `VITE_API_URL`.

El SDK web de Firebase inicializa el proyecto `tienda-83288` y Analytics. Las
operaciones de Cloud Firestore no se realizan directamente desde el navegador:
pasan por los controladores ASP.NET para no exponer credenciales administrativas.
