FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER app
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["OnionStore/API.csproj", "OnionStore/"]
COPY ["Repository/Repository.csproj", "Repository/"]
COPY ["Core/Core.csproj", "Core/"]
COPY ["Service/Service.csproj", "Service/"]
RUN dotnet restore "./OnionStore/API.csproj"
COPY . .
WORKDIR "/src/OnionStore"
RUN dotnet build "./API.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./API.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
COPY ["Repository/Store/DataSeeding", "/app/DataSeeding"]
ENTRYPOINT ["dotnet", "API.dll"]
