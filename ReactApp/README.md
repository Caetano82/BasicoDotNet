
# 🚀 Guia de Instalação Rápida

## Passo a Passo

### 1. Instalar Dependências
```bash
cd ReactApp
npm install
```

### 2. Configurar URL da API

Crie um arquivo `.env` na pasta `ReactApp` com o seguinte conteúdo:
```
REACT_APP_API_URL=https://localhost:5001/api/v1
```

Ou se sua API estiver em outro endereço, ajuste conforme necessário.

### 3. Iniciar a Aplicação
```bash
npm start
```

A aplicação abrirá automaticamente em `http://localhost:3000`

## ⚠️ Importante

### Certificado SSL Auto-assinado

Se sua API usa HTTPS com certificado auto-assinado (como `https://localhost:5001`):

1. **Primeira vez**: Acesse `https://localhost:5001` diretamente no navegador
2. Aceite o aviso de segurança do navegador
3. Depois disso, a aplicação React conseguirá fazer as requisições

### Alternativa: Usar HTTP

Se preferir, você pode configurar sua API para usar HTTP em desenvolvimento e ajustar a URL no `.env`:
```
REACT_APP_API_URL=http://localhost:5000/api/v1
```

## 🎯 Testando

1. Certifique-se de que a API está rodando
2. Abra `http://localhost:3000`
3. Clique em "+ Novo Aviso" para criar seu primeiro post-it
4. Os avisos aparecerão como post-its coloridos no board!

## 🐛 Problemas Comuns

### Erro de CORS
Se aparecer erro de CORS, verifique se a API está configurada para aceitar requisições do `http://localhost:3000`

### Erro de Conexão
- Verifique se a API está rodando
- Verifique se a URL no `.env` está correta
- Verifique se aceitou o certificado SSL (se usar HTTPS)
