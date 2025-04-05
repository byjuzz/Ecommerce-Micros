#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER app
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["03.Service/01.Identity/01.Identity.Api/Identity.Api/Identity.Api.csproj", "03.Service/01.Identity/01.Identity.Api/Identity.Api/"]
COPY ["03.Service/01.Identity/02.Identity.Service/Identity.Service.EventHandlers/Identity.Service.EventHandlers.csproj", "03.Service/01.Identity/02.Identity.Service/Identity.Service.EventHandlers/"]
COPY ["03.Service/01.Identity/03.Identity.Persistence/Identity.Persistence.Database/Identity.Persistence.Database.csproj", "03.Service/01.Identity/03.Identity.Persistence/Identity.Persistence.Database/"]
COPY ["03.Service/01.Identity/04.Identity.Models/Identity.Domain/Identity.Domain.csproj", "03.Service/01.Identity/04.Identity.Models/Identity.Domain/"]
COPY ["03.Service/01.Identity/02.Identity.Service/Identity.Service.Queries/Identity.Service.Queries.csproj", "03.Service/01.Identity/02.Identity.Service/Identity.Service.Queries/"]
COPY ["03.Service/05.Common/ServiceCommon.Collection/ServiceCommon.Collection.csproj", "03.Service/05.Common/ServiceCommon.Collection/"]
COPY ["03.Service/05.Common/ServiceCommon.Mapping/ServiceCommon.Mapping.csproj", "03.Service/05.Common/ServiceCommon.Mapping/"]
COPY ["03.Service/05.Common/ServiceCommon.Paging/ServiceCommon.Paging.csproj", "03.Service/05.Common/ServiceCommon.Paging/"]
RUN dotnet restore "./03.Service/01.Identity/01.Identity.Api/Identity.Api/Identity.Api.csproj"
COPY . .
WORKDIR "/src/03.Service/01.Identity/01.Identity.Api/Identity.Api"
RUN dotnet build "./Identity.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Identity.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Identity.Api.dll"]