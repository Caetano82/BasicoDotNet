# Executando o React App no Docker

## Pré-requisitos
- Docker instalado
- Docker Compose instalado (ou Docker com suporte a Compose)

## Opção 1: Usando Docker Compose (Recomendado)

### 1. Configurar a URL da API

Edite o arquivo `docker-compose.yml` e ajuste a variável `REACT_APP_API_URL` na seção `args`:

```yaml
build:
  args:
    - REACT_APP_API_URL=http://host.docker.internal:5001/api/v1
```

Ou defina como variável de ambiente antes de executar:

```bash
# Windows PowerShell
$env:REACT_APP_API_URL="http://host.docker.internal:5001/api/v1"
docker-compose up --build

# Linux/Mac
export REACT_APP_API_URL=http://host.docker.internal:5001/api/v1
docker-compose up --build
```

**Nota**: 
- `host.docker.internal` funciona no Windows e Mac
- No Linux, pode ser necessário usar o IP da máquina host (ex: `192.168.1.100:5001`)
- Se a API também estiver rodando no Docker, use o nome do serviço do docker-compose

### 2. Build e executar

```bash
cd ReactApp
docker-compose up --build
```

O app estará disponível em `http://localhost:3000`

### 3. Executar em background

```bash
docker-compose up -d --build
```

### 4. Parar o container

```bash
docker-compose down
```

## Opção 2: Usando Docker diretamente

### 1. Build da imagem

```bash
cd ReactApp
docker build -t bernhoeft-react-app .
```

### 2. Executar o container

```bash
docker build --build-arg REACT_APP_API_URL=http://host.docker.internal:5001/api/v1 -t bernhoeft-react-app .
docker run -d \
  -p 3000:80 \
  --name bernhoeft-react-app \
  bernhoeft-react-app
```

### 3. Parar o container

```bash
docker stop bernhoeft-react-app
docker rm bernhoeft-react-app
```

## Variáveis de Ambiente

**IMPORTANTE**: Como o React precisa das variáveis de ambiente no momento do build, elas devem ser passadas como `build-args`, não como `environment`.

1. **docker-compose.yml** (recomendado):
```yaml
build:
  args:
    - REACT_APP_API_URL=http://host.docker.internal:5001/api/v1
```

2. **Variável de ambiente do sistema**:
```bash
# Windows PowerShell
$env:REACT_APP_API_URL="http://host.docker.internal:5001/api/v1"
docker-compose up --build

# Linux/Mac
export REACT_APP_API_URL=http://host.docker.internal:5001/api/v1
docker-compose up --build
```

3. **Linha de comando Docker**:
```bash
docker build --build-arg REACT_APP_API_URL=http://host.docker.internal:5001/api/v1 -t bernhoeft-react-app .
```

## Notas Importantes

1. **URL da API**: Se a API estiver rodando na mesma máquina, use `host.docker.internal` (Windows/Mac) ou o IP da máquina (Linux)

2. **SSL/HTTPS**: Se a API usar HTTPS, você precisará:
   - Ajustar a URL para `https://...`
   - Configurar o certificado SSL no nginx se necessário

3. **CORS**: Certifique-se de que a API permite requisições do domínio do React app

4. **Build em produção**: O Dockerfile usa um build otimizado de produção com nginx

5. **Logs**: Para ver os logs:
```bash
docker-compose logs -f react-app
```

## Troubleshooting

### Container não inicia
```bash
docker-compose logs react-app
```

### Verificar se o container está rodando
```bash
docker ps
```

### Acessar o container
```bash
docker exec -it bernhoeft-react-app sh
```

### Rebuild completo
```bash
docker-compose down
docker-compose build --no-cache
docker-compose up
```

