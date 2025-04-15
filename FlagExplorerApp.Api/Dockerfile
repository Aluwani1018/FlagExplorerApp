# Base image for the application
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["FlagExplorerApp.Api/FlagExplorerApp.Api.csproj", "FlagExplorerApp.Api/"]
RUN dotnet restore "./FlagExplorerApp.Api/FlagExplorerApp.Api.csproj"
COPY . .
WORKDIR "/src/FlagExplorerApp.Api"
RUN dotnet build "./FlagExplorerApp.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build

# Publish stage
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./FlagExplorerApp.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# Final stage for the application
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "FlagExplorerApp.Api.dll"]