
FROM mcr.microsoft.com/dotnet/aspnet:10.0.0-rc.1 AS base
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

USER $APP_UID

FROM mcr.microsoft.com/dotnet/sdk:10.0.100-rc.1 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# Install native dependencies required by wasm-tools
RUN apt-get update && \
    apt-get install -y --no-install-recommends python3 python3-pip clang zlib1g-dev && \
    ln -sf /usr/bin/python3 /usr/bin/python && \
    rm -rf /var/lib/apt/lists/*

# Install WebAssembly workloads for .NET 10
RUN dotnet workload install wasm-tools

# Copy project files for restore
COPY ["Blogtify/Blogtify/Blogtify.csproj", "Blogtify/Blogtify/"]
COPY ["Blogtify/Blogtify.Client/Blogtify.Client.csproj", "Blogtify/Blogtify.Client/"]

# Restore NuGet packages
RUN dotnet restore "Blogtify/Blogtify/Blogtify.csproj"

# Copy the entire source
COPY . .

# Build server
WORKDIR "/src/Blogtify/Blogtify"
RUN dotnet build "Blogtify.csproj" -c $BUILD_CONFIGURATION -o /app/build

# Publish server (ASP.NET Core Host)
RUN dotnet publish "Blogtify.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# ──────────────────────────────────────────────
# BUILD CLIENT (Blazor WebAssembly)
# ──────────────────────────────────────────────
WORKDIR "/src/Blogtify/Blogtify.Client"

# Publish client with compat build (fallback WASM mode)
RUN dotnet publish "Blogtify.Client.csproj" -c ReleaseCompat -o /app/publishCompat --no-restore

# Copy compat wasm runtime into server's wwwroot
RUN mkdir -p /app/publish/wwwroot/_frameworkCompat && \
    cp -r /app/publishCompat/wwwroot/_framework/* /app/publish/wwwroot/_frameworkCompat/

FROM mcr.microsoft.com/dotnet/aspnet:10.0.0-rc.1 AS final
WORKDIR /app
COPY --from=build /app/publish .

ENV ASPNETCORE_URLS=http://0.0.0.0:${PORT:-8080}

ENTRYPOINT ["dotnet", "Blogtify.dll"]
