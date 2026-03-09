# UsersAPI

Microsserviço responsável pelo cadastro, autenticação e gerenciamento de usuários da plataforma FiapCloudGames.

## Responsabilidades
- Registro de novos usuários
- Autenticação com geração de token JWT
- CRUD de usuários (Admin)
- Publicação do evento `UserCreatedEvent` para notificações de boas-vindas

## Eventos publicados
| Evento | Quando |
|--------|--------|
| `UserCreatedEvent` | Ao registrar um novo usuário |

## Variáveis de Ambiente

| Variável | Descrição | Exemplo |
|----------|-----------|---------|
| `ConnectionStrings__Users` | Connection string PostgreSQL | `Host=postgres;Database=users_db;...` |
| `Jwt__Key` | Chave secreta para assinar o JWT (mín. 32 chars) | `my-secret-key` |
| `Jwt__Issuer` | Issuer do JWT | `FiapCloudGames` |
| `Jwt__Audience` | Audience do JWT | `FiapCloudGames` |
| `RabbitMQ__Host` | Host do RabbitMQ | `rabbitmq` |
| `RabbitMQ__Username` | Usuário RabbitMQ | `guest` |
| `RabbitMQ__Password` | Senha RabbitMQ | `guest` |

## Executando localmente

```bash
dotnet run
```

Swagger disponível em: http://localhost:5001/swagger

## Endpoints

| Método | Rota | Auth | Descrição |
|--------|------|------|-----------|
| POST | /api/users | Público | Registrar usuário |
| POST | /api/users/login | Público | Login |
| GET | /api/users | Admin | Listar usuários |
| GET | /api/users/{id} | Admin | Buscar por ID |
| GET | /api/users/email | Admin | Buscar por e-mail |
| GET | /api/users/name | Admin | Buscar por nome |
| PATCH | /api/users/{id} | Admin | Atualizar usuário |
| PATCH | /api/users/{id}/role | Admin | Alterar papel |
| DELETE | /api/users/{id} | Admin | Deletar usuário |
