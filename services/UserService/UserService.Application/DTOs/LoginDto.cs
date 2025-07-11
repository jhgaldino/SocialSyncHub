namespace UserService.Application.DTOs;

/// <summary>
/// DTO para login de usuário.
/// </summary>
public class LoginDto
{
    /// <example>teste@exemplo.com</example>
    /// <summary>Email do usuário.</summary>
    public string Email { get; set; } = string.Empty;
    
    /// <example>Senha@123</example>
    /// <summary>Senha do usuário.</summary>
    public string Password { get; set; } = string.Empty;
} 