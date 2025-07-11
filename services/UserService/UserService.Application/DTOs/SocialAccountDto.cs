namespace UserService.Application.DTOs;

public enum SocialNetworkTypeDto
{
    Instagram,
    TikTok,
    X
}

public class SocialAccountDto
{
    public Guid Id { get; set; }
    public SocialNetworkTypeDto NetworkType { get; set; }
    public string Username { get; set; } = string.Empty;
    public DateTime? ExpiresAt { get; set; }
}

public class ConnectSocialAccountDto
{
    public SocialNetworkTypeDto NetworkType { get; set; }
    public string AccessToken { get; set; } = string.Empty;
    public string? RefreshToken { get; set; }
}