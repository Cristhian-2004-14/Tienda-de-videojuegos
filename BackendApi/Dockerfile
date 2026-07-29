# Etapa 1: Imagen base ligera para ejecutar la app
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
WORKDIR /app
EXPOSE 5158
ENV ASPNETCORE_URLS=http://+:5158

# Etapa 2: Imagen SDK para compilar el código C#
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src
COPY ["BackendApi.csproj", "./"]
RUN dotnet restore "BackendApi.csproj"
COPY . .
RUN dotnet build "BackendApi.csproj" -c Release -o /app/build

# Etapa 3: Generar los binarios optimizados de producción
FROM build AS publish
RUN dotnet publish "BackendApi.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Etapa 4: Imagen final que ejecutará el servicio
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "BackendApi.dll"]
