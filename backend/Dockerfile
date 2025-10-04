FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Proje dosyalarını kopyala (DOĞRU SIRA ÖNEMLİ!)
COPY ["AddressApp.API/AddressApp.API.csproj", "AddressApp.API/"]
COPY ["AddressApp.Application/AddressApp.Application.csproj", "AddressApp.Application/"]
COPY ["AddressApp.Infrastructure/AddressApp.Infrastructure.csproj", "AddressApp.Infrastructure/"]
COPY ["AddressApp.Core/AddressApp.Core.csproj", "AddressApp.Core/"]

# Restore
RUN dotnet restore "AddressApp.API/AddressApp.API.csproj"

# Tüm kodu kopyala
COPY . .

# Build
WORKDIR "/src/AddressApp.API"
RUN dotnet build "AddressApp.API.csproj" -c Release -o /app/build

# Publish
FROM build AS publish
RUN dotnet publish "AddressApp.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Final image
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

# Environment
ENV ASPNETCORE_ENVIRONMENT=Production

ENTRYPOINT ["dotnet", "AddressApp.API.dll"]