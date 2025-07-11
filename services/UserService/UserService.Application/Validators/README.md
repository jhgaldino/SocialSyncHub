# FluentValidation no SocialSyncHub

Este documento explica como usar o FluentValidation no projeto SocialSyncHub.

## Estrutura Implementada

### 1. Validadores de DTOs
- `CreateUserDtoValidator.cs` - Validação para criação de usuários
- `LoginDtoValidator.cs` - Validação para login
- `RegisterDtoValidator.cs` - Validação para registro de usuários
- `UserDtoValidator.cs` - Validação para DTOs de usuário

### 2. Configuração Automática
O FluentValidation está configurado automaticamente no `Program.cs`:
- Registra todos os validadores da assembly
- Desabilita validação do DataAnnotations
- Adiciona filtro de validação global

### 3. Filtro de Validação
- `ValidationFilter.cs` - Trata erros de validação e retorna respostas padronizadas

### 4. Serviço de Validação Customizada
- `ValidationService.cs` - Para validações que precisam de acesso ao banco de dados

## Como Usar

### 1. Validação Automática
Os DTOs são validados automaticamente quando chegam nos controllers:

```csharp
[HttpPost]
public async Task<ActionResult<UserDto>> Create([FromBody] CreateUserDto createUserDto)
{
    // A validação acontece automaticamente antes de chegar aqui
    var user = await _userService.CreateAsync(createUserDto);
    return Ok(user);
}
```

### 2. Validação Customizada
Para validações que precisam de acesso ao banco de dados:

```csharp
public class UserController : ControllerBase
{
    private readonly IValidationService _validationService;

    [HttpPost]
    public async Task<ActionResult<UserDto>> Create([FromBody] CreateUserDto createUserDto)
    {
        // Validação customizada
        var emailValidation = await _validationService.ValidateUserEmailAsync(createUserDto.Email);
        if (!emailValidation.IsValid)
        {
            return BadRequest(new
            {
                Success = false,
                Message = "Erro de validação",
                Errors = emailValidation.Errors.Select(e => e.ErrorMessage)
            });
        }

        // Continua o processamento...
    }
}
```

### 3. Criando Novos Validadores

Para criar um novo validador:

```csharp
public class MeuDtoValidator : AbstractValidator<MeuDto>
{
    public MeuDtoValidator()
    {
        RuleFor(x => x.Propriedade)
            .NotEmpty().WithMessage("A propriedade é obrigatória")
            .Length(2, 100).WithMessage("Deve ter entre 2 e 100 caracteres");
    }
}
```

### 4. Validação Inline
Para validações pontuais:

```csharp
var validator = new InlineValidator<string>();
validator.RuleFor(x => x)
    .NotEmpty().WithMessage("Campo obrigatório")
    .EmailAddress().WithMessage("Email inválido");

var result = await validator.ValidateAsync(email);
```

## Regras de Validação Implementadas

### CreateUserDto
- Nome: obrigatório, 2-100 caracteres, apenas letras e espaços
- Email: obrigatório, formato válido, máximo 150 caracteres

### LoginDto
- Email: obrigatório, formato válido, máximo 150 caracteres
- Senha: obrigatória, 6-100 caracteres

### RegisterDto
- Nome: obrigatório, 2-100 caracteres, apenas letras e espaços
- Email: obrigatório, formato válido, máximo 150 caracteres
- Senha: obrigatória, 6-100 caracteres, deve conter maiúscula, minúscula, número e caractere especial
- ConfirmPassword: obrigatória, deve ser igual à senha

## Resposta de Erro Padronizada

Quando há erros de validação, a API retorna:

```json
{
    "success": false,
    "message": "Erro de validação",
    "errors": [
        "O nome é obrigatório",
        "O email deve ser válido"
    ]
}
```

## Benefícios

1. **Separação de Responsabilidades**: Validação separada da lógica de negócio
2. **Reutilização**: Validadores podem ser reutilizados em diferentes contextos
3. **Manutenibilidade**: Fácil de manter e modificar regras de validação
4. **Testabilidade**: Validadores podem ser testados independentemente
5. **Flexibilidade**: Suporte a validações customizadas e assíncronas
6. **Mensagens Localizadas**: Mensagens de erro em português 