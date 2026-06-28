# Build Stage
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Copy solution and project files
COPY ["BillingSystem.slnx", "./"]
COPY ["BillingSystem.API/BillingSystem.API.csproj", "BillingSystem.API/"]
COPY ["BillingSystem.Application/BillingSystem.Application.csproj", "BillingSystem.Application/"]
COPY ["BillingSystem.Domain/BillingSystem.Domain.csproj", "BillingSystem.Domain/"]
COPY ["BillingSystem.Infrastructure/BillingSystem.Infrastructure.csproj", "BillingSystem.Infrastructure/"]

# Restore dependencies
RUN dotnet restore "BillingSystem.API/BillingSystem.API.csproj"

# Copy everything else and build
COPY . .
WORKDIR "/src/BillingSystem.API"
RUN dotnet build "BillingSystem.API.csproj" -c Release -o /app/build

# Publish Stage
FROM build AS publish
RUN dotnet publish "BillingSystem.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Final Runtime Stage
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app
EXPOSE 8080

# Instalar dependencias necesarias para Microsoft.Data.SqlClient en Linux
RUN apt-get update && apt-get install -y libgssapi-krb5-2 && rm -rf /var/lib/apt/lists/*

COPY --from=publish /app/publish .

ENTRYPOINT ["dotnet", "BillingSystem.API.dll"]
