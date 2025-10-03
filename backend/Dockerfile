FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 5000

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Proje dosyalarını kopyala
COPY ["AddressApp.API/AddressApp.API.csproj", "AddressApp.API/"]
COPY ["AddressApp.Application/AddressApp.Application.csproj", "AddressApp.Application/"]
COPY ["AddressApp.Infrastructure/AddressApp.Infrastructure.csproj", "AddressApp.Infrastructure/"]
COPY ["AddressApp.Domain/AddressApp.Domain.csproj", "AddressApp.Domain/"]

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
ENV ASPNETCORE_URLS=http://+:5000

ENTRYPOINT ["dotnet", "AddressApp.API.dll"]