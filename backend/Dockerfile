# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy ONLY the actual project files (not solution)
COPY src/AddressApp.Domain/*.csproj ./src/AddressApp.Domain/
COPY src/AddressApp.Application/*.csproj ./src/AddressApp.Application/
COPY src/AddressApp.Infrastructure/*.csproj ./src/AddressApp.Infrastructure/
COPY src/AddressApp.API/*.csproj ./src/AddressApp.API/

# Restore API project (which will restore all dependencies)
WORKDIR /src/src/AddressApp.API
RUN dotnet restore

# Copy all source code
WORKDIR /src
COPY src/ ./src/

# Build
WORKDIR /src/src/AddressApp.API
RUN dotnet build -c Release -o /app/build

# Publish
FROM build AS publish
RUN dotnet publish -c Release -o /app/publish

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app

# Install curl for health checks
RUN apt-get update && apt-get install -y curl && rm -rf /var/lib/apt/lists/*

# Copy published files
COPY --from=publish /app/publish .

# Expose port
EXPOSE 8080

# Environment variables
ENV ASPNETCORE_ENVIRONMENT=Production
ENV ASPNETCORE_URLS=http://+:8080
ENV PORT=8080

# Health check
HEALTHCHECK --interval=30s --timeout=10s --start-period=40s --retries=3 \
    CMD curl -f http://localhost:8080/health || exit 1

# Start application
ENTRYPOINT ["dotnet", "AddressApp.API.dll"]