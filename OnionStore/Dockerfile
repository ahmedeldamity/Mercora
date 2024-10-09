FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER app
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Core/BlazorEcommerce.Domain.csproj", "BlazorEcommerce.Domain/"]
COPY ["Core/BlazorEcommerce.Application", "BlazorEcommerce.Domain/"]
COPY ["OnionStore/BlazorEcommerce.Server.csproj", "BlazorEcommerce.Server/"]
COPY ["Repository/BlazorEcommerce.Persistence.csproj", "BlazorEcommerce.Persistence/"]
COPY ["Service/BlazorEcommerce.Infrastructure.csproj", "BlazorEcommerce.Infrastructure/"]
RUN dotnet restore "./BlazorEcommerce.Server/BlazorEcommerce.Server.csproj"
COPY . .
WORKDIR "/src/BlazorEcommerce.Server"
RUN dotnet build "./BlazorEcommerce.Server.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./BlazorEcommerce.Server.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
COPY ["Repository/Store/DataSeeding", "/app/DataSeeding"]
COPY ["OnionStore/Templates", "/app/Templates"]
ENTRYPOINT ["dotnet", "API.dll"]