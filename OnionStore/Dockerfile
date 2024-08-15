#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER app
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["OnionStore/API.csproj", "OnionStore/"]
COPY ["Repository/Repository.csproj", "Repository/"]
COPY ["Core/Core.csproj", "Core/"]
COPY ["Shared/Shared.csproj", "Shared/"]
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
ENTRYPOINT ["dotnet", "API.dll"]