# SocialSyncHub

Um projeto de estudo e portfólio que utiliza arquitetura de microserviços em C# (.NET 8).

## Estrutura do Projeto

```
SocialSyncHub/
├── services/
│   └── UserService/
│       ├── UserService.API/          # API REST
│       ├── UserService.Application/   # Serviços de aplicação, DTOs
│       ├── UserService.Domain/       # Entidades, interfaces, enums
│       ├── UserService.Infrastructure/ # Repositórios, persistência
│       └── UserService.Tests/        # Testes unitários
├── gateway/
│   └── Gateway.API/                  # API Gateway (Ocelot)
├── shared/                          # Bibliotecas compartilhadas (futuro)
└── docker-compose.yml               # Orquestração dos serviços
```

## UserService

### Tecnologias Utilizadas

- **.NET 8**
- **ASP.NET Core Web API**
- **Entity Framework Core**
- **SQL Server**
- **Swagger/OpenAPI**
- **Serilog**
- **JWT Authentication**
- **Docker & Docker Compose**
- **xUnit (Testes)**

### Funcionalidades

- ✅ Criar usuário
- ✅ Buscar usuário por ID
- ✅ Listar todos os usuários
- ✅ Validação de email único
- ✅ Logging estruturado
- ✅ Documentação automática (Swagger)
- ✅ **Autenticação JWT**
- ✅ **Registro e Login de usuários**
- ✅ **Testes unitários**
- ✅ **API Gateway**

### Endpoints

#### Autenticação (Público)
- `POST /api/auth/register` - Registrar novo usuário
- `POST /api/auth/login` - Fazer login
- `POST /api/auth/refresh` - Renovar token (não implementado)

#### Usuários (Protegido)
- `GET /api/user/{id}` - Buscar usuário por ID
- `POST /api/user` - Criar novo usuário (público)
- `GET /api/user` - Listar todos os usuários

### Executando o Projeto

#### Opção 1: Docker Compose (Recomendado)

```bash
# Na raiz do projeto
docker-compose up -d
```

- **API Gateway**: `http://localhost:5001`
- **UserService**: `http://localhost:5000`
- **Swagger Gateway**: `http://localhost:5001/swagger`
- **Swagger UserService**: `http://localhost:5000/swagger`

#### Opção 2: Desenvolvimento Local

1. **Instalar SQL Server** (ou usar Docker):
```bash
docker run -e "ACCEPT_EULA=Y" -e "SA_PASSWORD=YourStrong@Passw0rd" -p 1433:1433 --name sqlserver -d mcr.microsoft.com/mssql/server:2022-latest
```

2. **Executar a API**:
```bash
cd services/UserService/UserService.API
dotnet run
```

3. **Executar o Gateway**:
```bash
cd gateway/Gateway.API
dotnet run
```

### Exemplo de Uso

#### 1. Registrar um usuário:
```bash
curl -X POST "http://localhost:5001/auth/register" \
  -H "Content-Type: application/json" \
  -d '{
    "name": "João Silva",
    "email": "joao@example.com",
    "password": "senha123",
    "confirmPassword": "senha123"
  }'
```

#### 2. Fazer login:
```bash
curl -X POST "http://localhost:5001/auth/login" \
  -H "Content-Type: application/json" \
  -d '{
    "email": "joao@example.com",
    "password": "senha123"
  }'
```

#### 3. Usar o token para acessar recursos protegidos:
```bash
curl -X GET "http://localhost:5001/userservice/user/{user-id}" \
  -H "Authorization: Bearer {seu-token-jwt}"
```

### Executando Testes

```bash
# Executar todos os testes
cd services/UserService/UserService.Tests
dotnet test

# Executar com cobertura
dotnet test --collect:"XPlat Code Coverage"
```

### Estrutura de Camadas

#### Domain
- **Entidades**: `User`
- **Interfaces**: `IUserRepository`

#### Application
- **DTOs**: `CreateUserDto`, `UserDto`, `LoginDto`, `RegisterDto`, `AuthResponseDto`
- **Serviços**: `UserService`, `IUserService`, `AuthService`, `IAuthService`

#### Infrastructure
- **Contexto**: `AppDbContext`
- **Repositórios**: `UserRepository`

#### API
- **Controllers**: `UserController`, `AuthController`
- **Configuração**: `Program.cs`, `appsettings.json`

#### Tests
- **Testes de Serviços**: `UserServiceTests`, `AuthServiceTests`

#### Gateway
- **API Gateway**: Ocelot
- **Roteamento**: `ocelot.json`

### Configuração JWT

As configurações JWT estão no `appsettings.json`:

```json
{
  "Jwt": {
    "Secret": "YourSuperSecretKeyHereThatIsAtLeast32CharactersLong",
    "Issuer": "SocialSyncHub",
    "Audience": "SocialSyncHubUsers",
    "ExpirationHours": 1
  }
}
```

### API Gateway

O Gateway usa Ocelot para rotear requisições:

- `/auth/*` → UserService `/api/auth/*`
- `/userservice/*` → UserService `/api/*`

### Próximos Passos

- [ ] Implementar refresh token
- [ ] Adicionar mais validações
- [ ] Implementar testes de integração
- [ ] Criar outros microserviços
- [ ] Adicionar monitoramento e observabilidade
- [ ] Implementar rate limiting
- [ ] Adicionar cache distribuído 