using System.ComponentModel.DataAnnotations;

namespace UserService.Domain.Entities;

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