# Configuração do Azure Pipeline para Docker Registry

## Estrutura das Pipelines

O projeto possui **duas pipelines separadas** no mesmo repositório:

1. **azure-pipelines-backend.yml** - Para o backend (.NET API)
2. **azure-pipelines-frontend.yml** - Para o frontend (React App)

## Azure Container Registry

As pipelines estão configuradas para fazer push das imagens Docker para:
- **Registry**: `unewsistemas.azurecr.io`
- **Backend**: `unewsistemas.azurecr.io/bernhoeft-api`
- **Frontend**: `unewsistemas.azurecr.io/bernhoeft-react-app`

## Pipeline do Backend (azure-pipelines-backend.yml)

**Trigger**: Executa quando há mudanças em:
- `1-Presentation/Bernhoeft.GRT.Teste.Api/*`
- `2-Application/*`
- `3-Domain/*`
- `4-Infra/*`
- `azure-pipelines-backend.yml`

**Ações**:
1. Build da imagem Docker do backend (.NET API)
2. Push para `unewsistemas.azurecr.io/bernhoeft-api:$(Build.BuildId)`
3. Push para `unewsistemas.azurecr.io/bernhoeft-api:latest`

## Pipeline do Frontend (azure-pipelines-frontend.yml)

**Trigger**: Executa quando há mudanças em:
- `ReactApp/*`
- `azure-pipelines-frontend.yml`

**Ações**:
1. Build da imagem Docker do frontend (React App)
2. Push para `unewsistemas.azurecr.io/bernhoeft-react-app:$(Build.BuildId)`
3. Push para `unewsistemas.azurecr.io/bernhoeft-react-app:latest`

## Configuração do Service Connection no Azure DevOps

Para que a pipeline funcione corretamente, você precisa configurar um **Service Connection** no Azure DevOps:

### Passo 1: Criar Service Connection no Azure DevOps

1. No Azure DevOps, vá para **Project Settings** → **Service connections**
2. Clique em **New service connection**
3. Selecione **Docker Registry**
4. Escolha **Azure Container Registry**
5. Configure:
   - **Subscription**: Selecione a subscription do Azure
   - **Azure container registry**: Selecione `unewsistemas` ou digite `unewsistemas.azurecr.io`
   - **Service connection name**: `unewsistemas-acr-service-connection` (ou o nome que você preferir, mas precisa atualizar no `azure-pipelines.yml`)
6. Clique em **Save**

### Passo 2: Verificar o nome do Service Connection

O nome do Service Connection configurado nas pipelines é:
```yaml
dockerRegistryServiceConnection: 'unewsistemas-acr-service-connection'
```

**Importante**: Se você usou um nome diferente ao criar o Service Connection, atualize a variável `dockerRegistryServiceConnection` nos arquivos:
- `azure-pipelines-backend.yml`
- `azure-pipelines-frontend.yml`

### Passo 3: Configurar as Pipelines no Azure DevOps

No Azure DevOps, você precisa criar **duas pipelines separadas**:

1. **Pipeline do Backend**:
   - Vá para **Pipelines** → **New pipeline**
   - Selecione o repositório
   - Escolha **Existing Azure Pipelines YAML file**
   - Selecione o branch (ex: `master`)
   - Escolha o caminho: `/azure-pipelines-backend.yml`
   - Clique em **Continue** e depois em **Run**

2. **Pipeline do Frontend**:
   - Vá para **Pipelines** → **New pipeline**
   - Selecione o repositório
   - Escolha **Existing Azure Pipelines YAML file**
   - Selecione o branch (ex: `master`)
   - Escolha o caminho: `/azure-pipelines-frontend.yml`
   - Clique em **Continue** e depois em **Run**

### Passo 4: Verificar permissões

Certifique-se de que o Service Connection tem as seguintes permissões:
- **Pull** (para baixar imagens base)
- **Push** (para enviar as imagens construídas)

## Estrutura das Pipelines

### Pipeline Backend (azure-pipelines-backend.yml)

**Trigger**: Executa quando há mudanças em:
- `1-Presentation/Bernhoeft.GRT.Teste.Api/*`
- `2-Application/*`
- `3-Domain/*`
- `4-Infra/*`
- `azure-pipelines-backend.yml`

**Ações**:
- Build da imagem Docker do backend (.NET API)
- Push para `unewsistemas.azurecr.io/bernhoeft-api:$(Build.BuildId)`
- Push para `unewsistemas.azurecr.io/bernhoeft-api:latest`

### Pipeline Frontend (azure-pipelines-frontend.yml)

**Trigger**: Executa quando há mudanças em:
- `ReactApp/*`
- `azure-pipelines-frontend.yml`

**Ações**:
- Build da imagem Docker do frontend (React App)
- Push para `unewsistemas.azurecr.io/bernhoeft-react-app:$(Build.BuildId)`
- Push para `unewsistemas.azurecr.io/bernhoeft-react-app:latest`

## Variáveis Configuradas

**Backend** (`azure-pipelines-backend.yml`):
```yaml
dockerRegistryServiceConnection: 'unewsistemas-acr-service-connection'
containerRegistry: 'unewsistemas.azurecr.io'
backendImageRepository: 'bernhoeft-api'
tag: '$(Build.BuildId)'
```

**Frontend** (`azure-pipelines-frontend.yml`):
```yaml
dockerRegistryServiceConnection: 'unewsistemas-acr-service-connection'
containerRegistry: 'unewsistemas.azurecr.io'
frontendImageRepository: 'bernhoeft-react-app'
tag: '$(Build.BuildId)'
REACT_APP_API_URL: 'http://localhost:5001/api/v1'
```

## Troubleshooting

### Erro: "Unable to locate executable file: 'docker'"
- Verifique se o Docker está instalado no agente
- O pipeline usa `ubuntu-latest` que já possui Docker instalado

### Erro: "Service connection not found"
- Verifique se o nome do Service Connection está correto
- Certifique-se de que o Service Connection foi criado no mesmo projeto do Azure DevOps
- Verifique as permissões do Service Connection

### Erro: "unauthorized: authentication required"
- Verifique se o Service Connection tem as credenciais corretas do ACR
- Verifique se o ACR permite acesso do Service Connection

### Erro: "repository name must be lowercase"
- Os nomes dos repositórios já estão em lowercase: `bernhoeft-api` e `bernhoeft-react-app`

## Testando Localmente

Para testar o push localmente (opcional):

```bash
# Login no ACR
az acr login --name unewsistemas

# Build e push manual
docker build -f 1-Presentation/Bernhoeft.GRT.Teste.Api/Dockerfile -t unewsistemas.azurecr.io/bernhoeft-api:latest .
docker push unewsistemas.azurecr.io/bernhoeft-api:latest
```

## Próximos Passos

Após configurar o Service Connection:
1. Faça commit e push dos arquivos `azure-pipelines-backend.yml` e `azure-pipelines-frontend.yml`
2. Configure as duas pipelines no Azure DevOps (veja Passo 3)
3. Execute as pipelines manualmente ou aguarde o trigger automático
4. Verifique os logs das pipelines para confirmar o push bem-sucedido
5. Verifique no Azure Portal se as imagens foram criadas no ACR

**Nota**: As pipelines são independentes. Se você modificar apenas o backend, apenas a pipeline do backend será executada. O mesmo vale para o frontend.

