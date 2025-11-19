# Executando a API .NET no Docker

## Pré-requisitos
- Docker instalado
- Docker Compose instalado (ou Docker com suporte a Compose)

## Opção 1: Usando Docker Compose (Recomendado)

### 1. Build e executar

```bash
cd 1-Presentation/Bernhoeft.GRT.Teste.Api
docker-compose up --build
```

A API estará disponível em:
- HTTP: `http://localhost:5000`
- HTTPS: `https://localhost:5001`
- Swagger (apenas em Development/Staging): `http://localhost:5000/`

### 2. Executar em background

```bash
docker-compose up -d --build
```

### 3. Parar o container

```bash
docker-compose down
```

## Opção 2: Usando Docker diretamente

### 1. Build da imagem

```bash
cd 1-Presentation/Bernhoeft.GRT.Teste.Api
docker build -f Dockerfile -t bernhoeft-api ../../
```

**Nota**: O build context deve ser o diretório raiz do projeto (onde está o `.sln`)

### 2. Executar o container

```bash
docker run -d \
  -p 5000:80 \
  -p 5001:443 \
  -e ASPNETCORE_ENVIRONMENT=Production \
  -e ASPNETCORE_URLS=http://+:80 \
  --name bernhoeft-api \
  bernhoeft-api
```

### 3. Parar o container

```bash
docker stop bernhoeft-api
docker rm bernhoeft-api
```

## Variáveis de Ambiente

As variáveis de ambiente podem ser configuradas:

1. **docker-compose.yml**:
```yaml
environment:
  - ASPNETCORE_ENVIRONMENT=Production
  - ASPNETCORE_URLS=http://+:80
  - PORT=80
```

2. **Linha de comando**:
```bash
docker run -e ASPNETCORE_ENVIRONMENT=Development ...
```

## Endpoints Disponíveis

- **API Base**: `http://localhost:5000/api/v1`
- **Swagger** (Development/Staging): `http://localhost:5000/`
- **SignalR Hub**: `ws://localhost:5000/hub/avisos`
- **Health Check**: `http://localhost:5000/`

## Notas Importantes

1. **Build Context**: O Dockerfile deve ser executado do diretório raiz do projeto (onde está o `.sln`) porque ele referencia múltiplos projetos

2. **DLLs de Referência**: As DLLs na pasta `Libs/` são copiadas para o container e devem estar acessíveis

3. **Swagger**: Apenas disponível em ambientes `Development` ou `Staging`. Em `Production`, o Swagger não é exposto

4. **Portas**: Por padrão, a API usa a porta 80 no container. Use variáveis de ambiente para customizar

5. **Database**: Se estiver usando banco de dados externo, configure a string de conexão via variáveis de ambiente ou `appsettings.json`

## Troubleshooting

### Container não inicia
```bash
docker-compose logs api
```

### Verificar se o container está rodando
```bash
docker ps
```

### Acessar o container
```bash
docker exec -it bernhoeft-api bash
```

### Rebuild completo
```bash
docker-compose down
docker-compose build --no-cache
docker-compose up
```

