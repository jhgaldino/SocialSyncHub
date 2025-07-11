namespace UserService.Application.DTOs;

/// <summary>
/// DTO de resposta de autenticação, contendo token JWT, refresh token, expiração e dados do usuário autenticado.
/// </summary>
public class AuthResponseDto
{
    /// <example>eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...</example>
    /// <summary>Token JWT de acesso.</summary>
    public string Token { get; set; } = string.Empty;

    /// <example>eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...</example>
    /// <summary>Refresh token para renovação do acesso.</summary>
    public string RefreshToken { get; set; } = string.Empty;

    /// <example>2025-07-11T12:15:58.7607691</example>
    /// <summary>Data e hora de expiração do token.</summary>
    public DateTime ExpiresAt { get; set; }

    /// <summary>Dados do usuário autenticado.</summary>
    public UserDto User { get; set; } = new();
} 