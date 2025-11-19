# Stage 1: Build
FROM node:18-alpine AS builder

WORKDIR /app

# Copiar arquivos de dependências
COPY package*.json ./

# Instalar dependências
RUN npm ci --legacy-peer-deps

# Copiar código fonte
COPY . .

# Definir variável de ambiente para o build (com valor padrão)
ARG REACT_APP_API_URL=http://localhost:5001/api/v1
ENV REACT_APP_API_URL=$REACT_APP_API_URL

# Build da aplicação
RUN npm run build

# Stage 2: Production
FROM nginx:alpine

# Instalar wget para healthcheck
RUN apk add --no-cache wget

# Copiar o build para o nginx
COPY --from=builder /app/build /usr/share/nginx/html

# Copiar configuração customizada do nginx (opcional)
COPY nginx.conf /etc/nginx/conf.d/default.conf

# Expor porta 80
EXPOSE 80

# Comando para iniciar nginx
CMD ["nginx", "-g", "daemon off;"]

