namespace UserService.Application.DTOs;

/// <summary>
/// DTO para registro de novo usuário, contendo nome, email, senha e confirmação de senha.
/// </summary>
public class RegisterDto
{
    /// <example>Usuario Teste</example>
    /// <summary>Nome do usuário.</summary>
    public string Name { get; set; } = string.Empty;

    /// <example>teste@exemplo.com</example>
    /// <summary>Email do usuário.</summary>
    public string Email { get; set; } = string.Empty;

    /// <example>Senha@123</example>
    /// <summary>Senha do usuário.</summary>
    public string Password { get; set; } = string.Empty;

    /// <example>Senha@123</example>
    /// <summary>Confirmação da senha.</summary>
    public string ConfirmPassword { get; set; } = string.Empty;
} 