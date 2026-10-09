#!/bin/bash
set -a
source "$(dirname "$0")/.env"
set +a

if [ "$(docker ps -aq -f name=mssql)" ]; then
  echo "Container zaten var, başlatılıyor..."
  docker start mssql
else
  echo "Container oluşturuluyor..."
  docker run -e "ACCEPT_EULA=Y" -e "SA_PASSWORD=$DB_PASSWORD" \
    -p 1433:1433 --name mssql \
    -d mcr.microsoft.com/mssql/server:2022-latest
fi
