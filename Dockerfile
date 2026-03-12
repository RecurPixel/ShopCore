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
RUN dotnet restore src/ShopCore.API/ShopCore.API.csproj

# Copy everything else and build
COPY . .
RUN dotnet publish src/ShopCore.API/ShopCore.API.csproj \
    -c Release \
    -o /app/publish \
    --no-restore

# ── Runtime stage ─────────────────────────────────────────────────────────────
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app

RUN groupadd --system appgroup && \
    useradd --system --gid appgroup --no-create-home appuser

COPY --from=build /app/publish .

RUN mkdir -p logs && chown -R appuser:appgroup /app

USER appuser

EXPOSE 8080

ENTRYPOINT ["dotnet", "ShopCore.API.dll"]
