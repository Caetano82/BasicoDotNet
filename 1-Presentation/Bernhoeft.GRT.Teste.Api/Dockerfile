# Stage 1: Build
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copiar arquivo da solução primeiro
COPY ["Bernhoeft.GRT.Teste.sln", "./"]

# Copiar arquivos .csproj para restaurar dependências (camadas de cache)
COPY ["1-Presentation/Bernhoeft.GRT.Teste.Api/Bernhoeft.GRT.Teste.Api.csproj", "1-Presentation/Bernhoeft.GRT.Teste.Api/"]
COPY ["2-Application/Bernhoeft.GRT.Teste.Application/Bernhoeft.GRT.Teste.Application.csproj", "2-Application/Bernhoeft.GRT.Teste.Application/"]
COPY ["3-Domain/Bernhoeft.GRT.Teste.Domain/Bernhoeft.GRT.Teste.Domain.csproj", "3-Domain/Bernhoeft.GRT.Teste.Domain/"]
COPY ["4-Infra/Bernhoeft.GRT.Teste.Infra.Persistence.InMemory/Bernhoeft.GRT.Teste.Infra.Persistence.InMemory.csproj", "4-Infra/Bernhoeft.GRT.Teste.Infra.Persistence.InMemory/"]

# Copiar DLLs de referência (copiar diretório inteiro)
COPY ["Libs/", "Libs/"]

# Restaurar dependências NuGet
RUN dotnet restore "Bernhoeft.GRT.Teste.sln"

# Copiar todo o código fonte
COPY . .

# Build da aplicação
WORKDIR "/src/1-Presentation/Bernhoeft.GRT.Teste.Api"
RUN dotnet build "Bernhoeft.GRT.Teste.Api.csproj" -c Release --no-restore -o /app/build

# Stage 2: Publish
FROM build AS publish
WORKDIR "/src/1-Presentation/Bernhoeft.GRT.Teste.Api"
RUN dotnet publish "Bernhoeft.GRT.Teste.Api.csproj" -c Release --no-restore -o /app/publish /p:UseAppHost=false

# Stage 3: Runtime
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app

# Instalar curl e wget para healthcheck
RUN apt-get update && apt-get install -y --no-install-recommends curl ca-certificates && rm -rf /var/lib/apt/lists/*

# Copiar DLLs de referência para runtime
COPY --from=build /src/Libs/ ./Libs/

# Copiar aplicação publicada
COPY --from=publish /app/publish .

# Expor portas HTTP e HTTPS
EXPOSE 80
EXPOSE 443

# Variáveis de ambiente padrão
ENV ASPNETCORE_ENVIRONMENT=Production
ENV ASPNETCORE_URLS=http://+:80

# Health check (verifica se a aplicação responde na raiz)
HEALTHCHECK --interval=30s --timeout=10s --start-period=30s --retries=3 \
  CMD curl --fail http://localhost:80/ || wget --quiet --tries=1 --spider http://localhost:80/ || exit 1

# Ponto de entrada
ENTRYPOINT ["dotnet", "Bernhoeft.GRT.Teste.Api.dll"]

