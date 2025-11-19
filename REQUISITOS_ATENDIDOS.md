# Checklist de Requisitos - Desafio Técnico Bernhoeft GRT

## ✅ 1. Implementação de Endpoints

- [x] **GET /avisos/{id}**: Retornar um aviso específico com base no ID
  - Implementado em: `AvisosController.GetAvisoById`
  - Handler: `GetAvisoByIdHandler`
  - Validação de ID inválido implementada

- [x] **POST /avisos**: Criar um novo aviso
  - Implementado em: `AvisosController.CreateAviso`
  - Handler: `CreateAvisoHandler`
  - Validação FluentValidation implementada

- [x] **PUT /avisos/{id}**: Edita um aviso com base no ID
  - Implementado em: `AvisosController.UpdateAviso`
  - Handler: `UpdateAvisoHandler`
  - **Ajustado**: Agora edita apenas a mensagem (conforme requisito)

- [x] **DELETE /avisos/{id}**: Remove um aviso
  - Implementado em: `AvisosController.DeleteAviso`
  - Handler: `DeleteAvisoHandler`
  - Soft delete implementado

## ✅ 2. Ajustes de Negócio

- [x] **Controle de data de criação e edição**
  - `CreatedAt`: Campo privado que é definido automaticamente na criação
  - `UpdatedAt`: Campo atualizado automaticamente quando o aviso é editado ou deletado
  - Implementado em: `AvisoEntity` com métodos `UpdateMessage()` e `MarkAsDeleted()`

## ✅ 3. Regras Implementadas

### 3.1 Fluent Validation
- [x] **CreateAvisoValidator**: Valida título e mensagem não nulos/vazios
- [x] **UpdateAvisoValidator**: Valida mensagem não nula/vazia
- [x] **GetAvisoByIdRequestValidator**: Valida ID > 0
- [x] **DeleteAvisoCommandValidator**: Valida ID > 0
- Configurado em: `Program.cs` com `AddFluentValidationAutoValidation`

### 3.2 Validação de ID inválido no GET
- [x] Validação no controller: `if (id <= 0) return BadRequest("Id inválido.")`
- [x] Validador FluentValidation: `GetAvisoByIdRequestValidator`

### 3.3 Validação na criação
- [x] Título não pode ser nulo ou vazio: `RuleFor(x => x.Titulo).NotEmpty()`
- [x] Mensagem não pode ser nula ou vazia: `RuleFor(x => x.Mensagem).NotEmpty()`
- Implementado em: `CreateAvisoValidator`

### 3.4 Validação na edição
- [x] Apenas mensagem pode ser editada (título não é editável)
- [x] Mensagem não pode ser nula ou vazia: `RuleFor(x => x.Mensagem).NotEmpty()`
- Implementado em: `UpdateAvisoValidator`
- **Ajustado**: `UpdateAvisoRequest` agora contém apenas `Mensagem`

### 3.5 Soft Delete
- [x] Campo `IsDeleted` na entidade
- [x] Método `MarkAsDeleted()` que marca como deletado e atualiza `UpdatedAt`
- [x] Método `SoftDeleteAvisoAsync()` no repositório
- Implementado em: `AvisoEntity` e `AvisoRepository`

### 3.6 Buscas apenas de avisos ativos (não deletados)
- [x] `ObterTodosAvisosAsync`: Filtra por `!x.IsDeleted`
- [x] `ObterAvisoPorIdAsync`: Filtra por `!x.IsDeleted`
- [x] `ObterAvisosPaginadosAsync`: Filtra por `!x.IsDeleted`
- Implementado em: `AvisoRepository`

## 📋 Arquitetura

- ✅ Seguindo padrão Clean Architecture (Domain, Application, Infrastructure, Presentation)
- ✅ Uso de MediatR para CQRS
- ✅ Uso de FluentValidation para validações
- ✅ Repository Pattern implementado
- ✅ Soft Delete implementado
- ✅ Controle de auditoria (CreatedAt, UpdatedAt)

## 📝 Observações

1. **Paginação**: Implementada como melhoria adicional no GET /avisos
2. **Validações**: Todas as validações estão na camada de aplicação usando FluentValidation
3. **Soft Delete**: Implementado corretamente, todas as buscas filtram avisos deletados
4. **Update**: Ajustado para editar apenas a mensagem, conforme requisito

## ✅ Status Final

**TODOS OS REQUISITOS FORAM ATENDIDOS**

