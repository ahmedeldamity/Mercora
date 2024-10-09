FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER app
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

COPY ["src/Presentation/BlazorEcommerce.Shared/BlazorEcommerce.Shared.csproj", "src/Presentation/BlazorEcommerce.Shared/"]
COPY ["src/Core/BlazorEcommerce.Domain/BlazorEcommerce.Domain.csproj", "src/Core/BlazorEcommerce.Domain/"]
COPY ["src/Core/BlazorEcommerce.Application/BlazorEcommerce.Application.csproj", "src/Core/BlazorEcommerce.Application/"]
COPY ["src/Infrastructure/BlazorEcommerce.Persistence/BlazorEcommerce.Persistence.csproj", "src/Infrastructure/BlazorEcommerce.Persistence/"]
COPY ["src/Infrastructure/BlazorEcommerce.Infrastructure/BlazorEcommerce.Infrastructure.csproj", "src/Infrastructure/BlazorEcommerce.Infrastructure/"]
COPY ["src/Presentation/BlazorEcommerce.Server/BlazorEcommerce.Server.csproj", "src/Presentation/BlazorEcommerce.Server/"]

RUN dotnet restore "./src/Presentation/BlazorEcommerce.Server/BlazorEcommerce.Server.csproj"

COPY . .

WORKDIR "/src/src/Presentation/BlazorEcommerce.Server"

RUN dotnet build "./BlazorEcommerce.Server.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish

ARG BUILD_CONFIGURATION=Release

RUN dotnet publish "./BlazorEcommerce.Server.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app

COPY --from=publish /app/publish .

COPY ["src/Infrastructure/BlazorEcommerce.Persistence/Store/DataSeeding", "/app/DataSeeding"]
COPY ["src/Presentation/BlazorEcommerce.Server/Templates", "/app/Templates"]

ENTRYPOINT ["dotnet", "BlazorEcommerce.Server.dll"]