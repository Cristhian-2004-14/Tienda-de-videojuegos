#!/usr/bin/env bash
set -Eeuo pipefail

cd "$(dirname "$0")/.."

if [[ ! -f .env ]]; then
  echo "Falta el archivo .env. Cópialo desde .env.example y completa sus valores."
  exit 1
fi

echo "Validando la configuración de Docker Compose..."
docker compose config --quiet

echo "Construyendo y actualizando los contenedores..."
docker compose up --build --detach --remove-orphans

echo "Estado de los servicios:"
docker compose ps

echo "Despliegue completado. Revisa los registros con: docker compose logs --tail=100"
