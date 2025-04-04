FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER app
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# ? Copia el .csproj al mismo nivel
COPY ["Identity.ApiGateway.csproj", "."]
RUN dotnet restore "Identity.ApiGateway.csproj"

# ? Ahora sí copiamos todo
COPY . .

# ? Entramos a la carpeta real del proyecto
WORKDIR "/src"
RUN dotnet build "Identity.ApiGateway.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "Identity.ApiGateway.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

# ? Copiar configuración Ocelot si aplica
COPY Configuration /app/Configuration

ENTRYPOINT ["dotnet", "Identity.ApiGateway.dll"]
