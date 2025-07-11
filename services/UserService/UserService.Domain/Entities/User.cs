using System.ComponentModel.DataAnnotations;

namespace UserService.Domain.Entities;

public enum SocialNetworkType
{
    Instagram,
    TikTok,
    X
}

public class SocialAccount
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public SocialNetworkType NetworkType { get; set; }
    [Required]
    public string AccessToken { get; set; } = string.Empty;
    public string? RefreshToken { get; set; }
    public DateTime? ExpiresAt { get; set; }
    public string? Username { get; set; }
    public User? User { get; set; }
}

public class InstagramMedia
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string MediaId { get; set; } = string.Empty;
    public string MediaType { get; set; } = string.Empty;
    public string MediaUrl { get; set; } = string.Empty;
    public string? Caption { get; set; }
    public DateTime Timestamp { get; set; }
}

public class User
{
    public Guid Id { get; set; }
    
    [Required]
    [StringLength(100)]
    public string Name { get; set; } = string.Empty;
    
    [Required]
    [EmailAddress]
    [StringLength(150)]
    public string Email { get; set; } = string.Empty;
    
    [Required]
    [StringLength(100)]
    public string PasswordHash { get; set; } = string.Empty;
    
    public DateTime CreatedAt { get; set; }
    
    public DateTime? LastLoginAt { get; set; }
    
    public User()
    {
        Id = Guid.NewGuid();
        CreatedAt = DateTime.UtcNow;
    }
    
    public User(string name, string email, string passwordHash) : this()
    {
        Name = name;
        Email = email;
        PasswordHash = passwordHash;
    }
}