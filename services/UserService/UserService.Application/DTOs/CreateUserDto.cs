namespace UserService.Application.DTOs;

/// <summary>
/// DTO para criação de usuário.
/// </summary>
public class CreateUserDto
{
    /// <example>Usuario Teste</example>
    /// <summary>Nome do usuário</summary>
    public string Name { get; set; } = string.Empty;

    /// <example>teste@exemplo.com</example>
    /// <summary>Email do usuário</summary>
    public string Email { get; set; } = string.Empty;
} 