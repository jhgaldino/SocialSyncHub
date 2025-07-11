namespace UserService.Application.DTOs;

public class InstagramAuthStartResponseDto
{
    public string AuthorizationUrl { get; set; } = string.Empty;
}

public class InstagramAuthCallbackDto
{
    public string Code { get; set; } = string.Empty;
    public string State { get; set; } = string.Empty;
}

public class InstagramTokenResponseDto
{
    public string AccessToken { get; set; } = string.Empty;
    public string UserId { get; set; } = string.Empty;
}
