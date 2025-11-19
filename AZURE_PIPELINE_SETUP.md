# Configuração do Azure Pipeline para Docker Registry

## Azure Container Registry

O pipeline está configurado para fazer push das imagens Docker para:
- **Registry**: `unewsistemas.azurecr.io`
- **Backend**: `unewsistemas.azurecr.io/bernhoeft-api`
- **Frontend**: `unewsistemas.azurecr.io/bernhoeft-react-app`

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

O nome do Service Connection configurado no `azure-pipelines.yml` é:
```yaml
dockerRegistryServiceConnection: 'unewsistemas-acr-service-connection'
```

**Importante**: Se você usou um nome diferente ao criar o Service Connection, atualize a variável `dockerRegistryServiceConnection` no arquivo `azure-pipelines.yml`.

### Passo 3: Verificar permissões

Certifique-se de que o Service Connection tem as seguintes permissões:
- **Pull** (para baixar imagens base)
- **Push** (para enviar as imagens construídas)

## Estrutura do Pipeline

O pipeline está dividido em 2 stages:

### Stage 1: BuildBackend
- Build da imagem Docker do backend (.NET API)
- Push para `unewsistemas.azurecr.io/bernhoeft-api:$(Build.BuildId)`
- Push para `unewsistemas.azurecr.io/bernhoeft-api:latest`

### Stage 2: BuildFrontend
- Build da imagem Docker do frontend (React App)
- Push para `unewsistemas.azurecr.io/bernhoeft-react-app:$(Build.BuildId)`
- Push para `unewsistemas.azurecr.io/bernhoeft-react-app:latest`

## Variáveis Configuradas

```yaml
dockerRegistryServiceConnection: 'unewsistemas-acr-service-connection'
containerRegistry: 'unewsistemas.azurecr.io'
backendImageRepository: 'unewsistemas.azurecr.io/bernhoeft-api'
frontendImageRepository: 'unewsistemas.azurecr.io/bernhoeft-react-app'
tag: '$(Build.BuildId)'
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
1. Faça commit e push do `azure-pipelines.yml` atualizado
2. Execute o pipeline manualmente ou aguarde o trigger automático
3. Verifique os logs do pipeline para confirmar o push bem-sucedido
4. Verifique no Azure Portal se as imagens foram criadas no ACR

