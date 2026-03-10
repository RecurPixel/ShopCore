# ── Build stage ──────────────────────────────────────────────────────────────
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Copy solution + project files first (layer-cache friendly)
COPY ShopCore.sln global.json ./
COPY src/ShopCore.API/ShopCore.API.csproj             src/ShopCore.API/
COPY src/ShopCore.Application/ShopCore.Application.csproj  src/ShopCore.Application/
COPY src/ShopCore.Domain/ShopCore.Domain.csproj        src/ShopCore.Domain/
COPY src/ShopCore.Infrastructure/ShopCore.Infrastructure.csproj src/ShopCore.Infrastructure/

# Restore dependencies
RUN dotnet restore

# Copy everything else and build
COPY . .
RUN dotnet publish src/ShopCore.API/ShopCore.API.csproj \
    -c Release \
    -o /app/publish \
    --no-restore

# ── Runtime stage ─────────────────────────────────────────────────────────────
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app

# Create non-root user for security
RUN addgroup --system appgroup && adduser --system --ingroup appgroup appuser

# Copy published output
COPY --from=build /app/publish .

# Create logs directory with correct permissions
RUN mkdir -p logs && chown -R appuser:appgroup /app

USER appuser

EXPOSE 8080

ENTRYPOINT ["dotnet", "ShopCore.API.dll"]
