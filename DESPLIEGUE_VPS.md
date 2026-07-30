# Despliegue en el VPS

La aplicación se publica con dos contenedores:

- `frontend`: Nginx sirve Vue y expone el puerto público.
- `backend`: ASP.NET Core permanece dentro de la red privada de Docker.

Nginx reenvía las solicitudes `/api` al backend. Por eso el frontend utiliza
`/api` en producción y no necesita conocer la IP ni el dominio del servidor.

## Requisitos del VPS

- Git.
- Docker Engine.
- Complemento `docker compose`.
- Puerto público permitido por el firewall (por defecto `8080`).

## Primer despliegue

```bash
git clone https://github.com/Cristhian-2004-14/Tienda-de-videojuegos.git
cd Tienda-de-videojuegos
cp .env.example .env
nano .env
```

Completa `.env` únicamente en el VPS:

```dotenv
APP_PORT=8080
FIREBASE_PROJECT_ID=tienda-83288
FIREBASE_API_KEY=tu_api_key
```

No agregues `.env` a Git. Después inicia la aplicación:

```bash
bash scripts/deploy-vps.sh
```

La tienda quedará disponible en `http://IP_DEL_VPS:8080`. Si un proxy inverso
como Nginx, Caddy o Traefik administra HTTPS, debe dirigir el dominio al puerto
indicado por `APP_PORT`.

## Actualizar una instalación existente

Desde la carpeta del proyecto en el VPS:

```bash
git pull --ff-only origin main
bash scripts/deploy-vps.sh
```

El script valida Compose, reconstruye solamente lo necesario, reemplaza los
contenedores y conserva el archivo `.env`.

## Comprobaciones

```bash
docker compose ps
docker compose logs --tail=100
curl --fail http://127.0.0.1:8080/health
curl --fail http://127.0.0.1:8080/api/productos
```

Para seguir los registros en tiempo real:

```bash
docker compose logs --follow
```

## Recuperar la versión anterior

Antes de actualizar, anota el commit que está funcionando:

```bash
git rev-parse HEAD
```

Si fuera necesario volver a ese código, cambia explícitamente a dicho commit y
reconstruye:

```bash
git switch --detach ID_DEL_COMMIT_ANTERIOR
bash scripts/deploy-vps.sh
```

Para regresar a la versión principal:

```bash
git switch main
git pull --ff-only origin main
bash scripts/deploy-vps.sh
```
