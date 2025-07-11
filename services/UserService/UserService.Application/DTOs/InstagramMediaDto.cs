namespace UserService.Application.DTOs;

public class InstagramMediaDto
{
    public string MediaId { get; set; } = string.Empty;
    public string MediaType { get; set; } = string.Empty;
    public string MediaUrl { get; set; } = string.Empty;
    public string? Caption { get; set; }
    public DateTime Timestamp { get; set; }
}
