# Postman Collection - Bernhoeft GRT Teste API

## 📋 Como Importar

1. Abra o Postman
2. Clique em **Import** (canto superior esquerdo)
3. Selecione o arquivo `Postman_Collection_Bernhoeft_GRT_Teste_API.json`
4. (Opcional) Importe também o arquivo `Postman_Environment_Bernhoeft_GRT_Teste_API.json` para usar variáveis de ambiente

## 🔧 Configuração

### Variáveis de Ambiente

A collection usa a variável `base_url` que por padrão está configurada como:
- **Desenvolvimento**: `https://localhost:5001`
- **Produção**: Ajuste conforme necessário

### Como Configurar

1. Após importar a collection, clique em **Environments** (lateral esquerda)
2. Selecione o environment importado ou crie um novo
3. Ajuste a variável `base_url` conforme seu ambiente

## 📚 Endpoints Disponíveis

### Avisos

#### 1. **GET - Listar Avisos (Paginado)**
- **URL**: `{{base_url}}/api/v1/avisos?page=1&pageSize=10`
- **Método**: GET
- **Parâmetros Query**:
  - `page` (opcional): Número da página (padrão: 1)
  - `pageSize` (opcional): Tamanho da página (padrão: 10, máximo: 100)
- **Resposta**: Lista paginada de avisos

#### 2. **GET - Obter Aviso por ID**
- **URL**: `{{base_url}}/api/v1/avisos/:id`
- **Método**: GET
- **Parâmetros Path**:
  - `id`: ID do aviso (deve ser maior que 0)
- **Resposta**: Detalhes do aviso ou 404 se não encontrado

#### 3. **POST - Criar Aviso**
- **URL**: `{{base_url}}/api/v1/avisos`
- **Método**: POST
- **Body** (JSON):
  ```json
  {
      "titulo": "Novo Aviso",
      "mensagem": "Esta é uma mensagem de teste para o novo aviso"
  }
  ```
- **Validações**:
  - `titulo`: Obrigatório, não pode ser nulo ou vazio
  - `mensagem`: Obrigatório, não pode ser nulo ou vazio
- **Resposta**: Aviso criado com ID gerado

#### 4. **PUT - Atualizar Aviso**
- **URL**: `{{base_url}}/api/v1/avisos/:id`
- **Método**: PUT
- **Parâmetros Path**:
  - `id`: ID do aviso a ser atualizado (deve ser maior que 0)
- **Body** (JSON):
  ```json
  {
      "mensagem": "Mensagem atualizada do aviso"
  }
  ```
- **Validações**:
  - `mensagem`: Obrigatório, não pode ser nulo ou vazio
  - **Nota**: Apenas a mensagem pode ser editada, o título não pode ser alterado
- **Resposta**: Confirmação de atualização ou 404 se não encontrado

#### 5. **DELETE - Excluir Aviso**
- **URL**: `{{base_url}}/api/v1/avisos/:id`
- **Método**: DELETE
- **Parâmetros Path**:
  - `id`: ID do aviso a ser excluído (deve ser maior que 0)
- **Resposta**: 204 No Content se excluído com sucesso, 404 se não encontrado
- **Nota**: Soft delete - o aviso é marcado como deletado, mas não é removido fisicamente do banco

## 🧪 Testes de Validação

A collection inclui também exemplos de requisições que devem retornar erros de validação:

1. **POST - Criar Aviso (Título Vazio)** - Deve retornar BadRequest
2. **POST - Criar Aviso (Mensagem Vazia)** - Deve retornar BadRequest
3. **GET - Obter Aviso (ID Inválido)** - Deve retornar BadRequest
4. **PUT - Atualizar Aviso (Mensagem Vazia)** - Deve retornar BadRequest

## 📝 Exemplos de Respostas

### Sucesso - GET /avisos
```json
{
    "Mensagem": "Avisos encontrados.",
    "Data": {
        "Data": [
            {
                "Id": 1,
                "Titulo": "Aviso 1",
                "Mensagem": "Mensagem do aviso 1",
                "Ativo": true
            }
        ],
        "Page": 1,
        "PageSize": 10,
        "TotalCount": 1,
        "TotalPages": 1,
        "HasPreviousPage": false,
        "HasNextPage": false
    }
}
```

### Sucesso - POST /avisos
```json
{
    "Id": 1,
    "Titulo": "Novo Aviso",
    "Mensagem": "Esta é uma mensagem de teste"
}
```

### Erro - Validação
```json
{
    "errors": {
        "Titulo": [
            "O título é obrigatório."
        ]
    },
    "type": "https://tools.ietf.org/html/rfc7231#section-6.5.1",
    "title": "One or more validation errors occurred.",
    "status": 400
}
```

## 🔒 Autenticação

Atualmente a API está configurada com `[AllowAnonymous]`, então não é necessária autenticação para os testes.

## ⚠️ Observações Importantes

1. **Soft Delete**: Quando um aviso é deletado, ele não aparece mais nas buscas, mas permanece no banco de dados
2. **Paginação**: O máximo de `pageSize` é 100. Valores maiores serão normalizados para 10
3. **Validação**: Todas as validações são feitas usando FluentValidation na camada de aplicação
4. **Timestamps**: Os avisos têm campos `CreatedAt` e `UpdatedAt` que são gerenciados automaticamente

## 🚀 Executando a Collection

1. Certifique-se de que a API está rodando
2. Configure a variável `base_url` no environment
3. Execute os requests na ordem desejada
4. Use o **Collection Runner** do Postman para executar todos os testes de uma vez

