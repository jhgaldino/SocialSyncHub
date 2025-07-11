namespace UserService.Application.DTOs;

/// <summary>
/// DTO de retorno de usuário.
/// </summary>
public class UserDto
{
    /// <example>8c7a6904-52a3-496a-a0ec-b8bfaf7cc94e</example>
    /// <summary>Identificador único do usuário</summary>
    public Guid Id { get; set; }

    /// <example>Usuario Teste</example>
    /// <summary>Nome do usuário</summary>
    public string Name { get; set; } = string.Empty;

    /// <example>teste@exemplo.com</example>
    /// <summary>Email do usuário</summary>
    public string Email { get; set; } = string.Empty;

    /// <example>2025-07-11T12:15:58.7607691</example>
    /// <summary>Data de criação do usuário</summary>
    public DateTime CreatedAt { get; set; }
} 