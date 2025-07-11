using System.Net.Http;
using System.Net.Http.Json;
using Microsoft.Extensions.Configuration;
using UserService.Application.DTOs;

namespace UserService.Application.Services;

public class InstagramAuthService : IInstagramAuthService
{
    private readonly IConfiguration _configuration;
    private readonly HttpClient _httpClient;

    public InstagramAuthService(IConfiguration configuration, HttpClient httpClient)
    {
        _configuration = configuration;
        _httpClient = httpClient;
    }

    public string GetAuthorizationUrl(Guid userId)
    {
        var clientId = _configuration["Instagram:ClientId"];
        var redirectUri = _configuration["Instagram:RedirectUri"];
        var state = userId.ToString();
        return $"https://api.instagram.com/oauth/authorize?client_id={clientId}&redirect_uri={redirectUri}&scope=user_profile,user_media&response_type=code&state={state}";
    }

    public async Task<InstagramTokenResponseDto> ExchangeCodeForTokenAsync(string code, string redirectUri)
    {
        var clientId = _configuration["Instagram:ClientId"];
        var clientSecret = _configuration["Instagram:ClientSecret"];
        var response = await _httpClient.PostAsync(
            "https://api.instagram.com/oauth/access_token",
            new FormUrlEncodedContent(new Dictionary<string, string>
            {
                { "client_id", clientId! },
                { "client_secret", clientSecret! },
                { "grant_type", "authorization_code" },
                { "redirect_uri", redirectUri },
                { "code", code }
            })
        );
        response.EnsureSuccessStatusCode();
        var json = await response.Content.ReadFromJsonAsync<InstagramTokenResponseDto>();
        return json!;
    }
}
