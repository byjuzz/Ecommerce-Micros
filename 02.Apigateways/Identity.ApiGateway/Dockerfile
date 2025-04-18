#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER app
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["02.Apigateways/Identity.ApiGateway/Identity.ApiGateway.csproj", "02.Apigateways/Identity.ApiGateway/"]
COPY 02.Apigateways/Identity.ApiGateway/Configuration/ocelot.json /app/Configuration/ocelot.json

RUN dotnet restore "./02.Apigateways/Identity.ApiGateway/Identity.ApiGateway.csproj"
COPY . .
WORKDIR "/src/02.Apigateways/Identity.ApiGateway"
RUN dotnet build "./Identity.ApiGateway.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Identity.ApiGateway.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Identity.ApiGateway.dll"]