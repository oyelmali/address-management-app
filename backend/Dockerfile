# Build Stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy csproj and restore
COPY ["AddressApp.API/AddressApp.API.csproj", "AddressApp.API/"]
COPY ["AddressApp.Application/AddressApp.Application.csproj", "AddressApp.Application/"]
COPY ["AddressApp.Core/AddressApp.Core.csproj", "AddressApp.Core/"]
COPY ["AddressApp.Infrastructure/AddressApp.Infrastructure.csproj", "AddressApp.Infrastructure/"]

RUN dotnet restore "AddressApp.API/AddressApp.API.csproj"

# Copy everything else
COPY . .

# Build
WORKDIR "/src/AddressApp.API"
RUN dotnet build "AddressApp.API.csproj" -c Release -o /app/build

# Publish
FROM build AS publish
RUN dotnet publish "AddressApp.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Runtime Stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=publish /app/publish .

EXPOSE 5257

ENTRYPOINT ["dotnet", "AddressApp.API.dll"]