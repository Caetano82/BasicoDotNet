# 🚀 Como Iniciar a Aplicação React

## Status Atual
✅ Dependências instaladas  
✅ Servidor iniciado em background  

## Acesso à Aplicação

A aplicação deve estar acessível em: **http://localhost:3000**

Se não abrir automaticamente no navegador:
1. Abra seu navegador
2. Acesse: `http://localhost:3000`

## ⚠️ Configuração da API

### 1. Criar arquivo .env

Crie um arquivo `.env` na pasta `ReactApp` com:
```
REACT_APP_API_URL=https://localhost:5001/api/v1
```

### 2. Verificar se a API está rodando

A API deve estar rodando em `https://localhost:5001`

### 3. Aceitar certificado SSL (se usar HTTPS)

Se a API usa HTTPS com certificado auto-assinado:
1. Acesse `https://localhost:5001` diretamente no navegador
2. Clique em "Avançado" ou "Advanced"
3. Clique em "Prosseguir para localhost" ou "Proceed to localhost"
4. Depois disso, a aplicação React conseguirá fazer requisições

## 🔍 Verificar se o servidor está rodando

Para verificar se o servidor está ativo:

```bash
# No PowerShell
Get-Process | Where-Object {$_.ProcessName -like "*node*"}
```

Ou verifique no navegador se `http://localhost:3000` está respondendo.

## 🐛 Problemas Comuns

### Erro de CORS
- Verifique se a API está rodando
- Verifique se a URL no `.env` está correta

### Erro de Conexão
- Certifique-se de que aceitou o certificado SSL (se usar HTTPS)
- Verifique se a API está acessível em `https://localhost:5001`

### Página em branco
- Abra o Console do Desenvolvedor (F12)
- Verifique se há erros no console
- Verifique se a API está retornando dados

## 📝 Comandos Úteis

```bash
# Iniciar servidor (se não estiver rodando)
npm start

# Parar servidor
# Pressione Ctrl+C no terminal onde está rodando

# Ver processos Node
Get-Process | Where-Object {$_.ProcessName -like "*node*"}
```

