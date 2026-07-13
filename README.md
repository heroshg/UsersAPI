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
| `ConnectionStrings__Users` | Connection string do RDS Postgres | `Host=fcg-dev-users.xxx.rds.amazonaws.com;Database=users_db;...` |
| `Jwt__RsaPrivateKey` | Chave privada RSA (base64 PEM) usada para assinar o JWT | — |
| `Jwt__RsaPublicKey` | Chave pública RSA (base64 PEM) exposta via JWKS (`/.well-known/jwks.json`) | — |
| `Jwt__Issuer` | Issuer do JWT | `FiapCloudGames` |
| `Jwt__Audience` | Audience do JWT | `FiapCloudGames` |
| `AWS__Region` | Região AWS (filas SQS/SNS) | `us-east-1` |
| `Messaging__Scope` | Prefixo de isolamento por ambiente das filas/tópicos SQS/SNS | `fcg-dev` |
| `Redis__ConnectionString` | Connection string do ElastiCache Redis; vazio → cache em memória | — |

Segredos (JWT, RDS, Redis) são injetados em runtime via AWS Secrets Manager + External Secrets
Operator no EKS (spec 05); localmente, via `dotnet user-secrets` (spec 06).

## Executando localmente

Pré-requisitos: **.NET 8 SDK** + credenciais AWS com acesso aos recursos do ambiente `dev`. Sem
docker-compose, sem Kubernetes:

```bash
dotnet run --project src/Users.API
```

A API sobe contra o RDS `dev` (público, SG allowlist) e as filas SQS/SNS `fcg-dev-*`.
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
