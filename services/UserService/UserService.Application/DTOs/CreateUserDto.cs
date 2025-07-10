using System.ComponentModel.DataAnnotations;

namespace UserService.Application.DTOs;

public class CreateUserDto
{
    [Required]
    [StringLength(100, MinimumLength = 2)]
    public string Name { get; set; } = string.Empty;
    
    [Required]
    [EmailAddress]
    [StringLength(150)]
    public string Email { get; set; } = string.Empty;
} 