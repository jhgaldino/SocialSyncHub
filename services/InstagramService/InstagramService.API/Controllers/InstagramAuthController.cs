using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.Net.Http;
using System.Threading.Tasks;
using System.Text.Json;
using System.Collections.Generic;
using InstagramService.API.Data;
using Microsoft.AspNetCore.Authorization;

namespace InstagramService.API.Controllers
{
    [ApiController]
    [Route("api/instagram/auth")]
    public class InstagramAuthController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly InstagramTokenRepository _tokenRepo;

        public InstagramAuthController(IConfiguration configuration, InstagramTokenRepository tokenRepo)
        {
            _configuration = configuration;
            _tokenRepo = tokenRepo;
        }

        [Authorize]
        [HttpGet("login")]
        public IActionResult Login()
        {
            var clientId = _configuration["Instagram:ClientId"];
            var redirectUri = _configuration["Instagram:RedirectUri"];
            var authUrl = $"https://api.instagram.com/oauth/authorize?client_id={clientId}&redirect_uri={redirectUri}&scope=user_profile,user_media&response_type=code";
            return Redirect(authUrl);
        }

        [Authorize]
        [HttpGet("callback")]
        public async Task<IActionResult> Callback([FromQuery] string code, [FromQuery] string error)
        {
            if (!string.IsNullOrEmpty(error))
            {
                return BadRequest(new { error });
            }
            if (string.IsNullOrEmpty(code))
            {
                return BadRequest(new { error = "Código de autorização não fornecido." });
            }

            var clientId = _configuration["Instagram:ClientId"];
            var clientSecret = _configuration["Instagram:ClientSecret"];
            var redirectUri = _configuration["Instagram:RedirectUri"];

            using var httpClient = new HttpClient();
            var tokenRequest = new HttpRequestMessage(HttpMethod.Post, "https://api.instagram.com/oauth/access_token")
            {
                Content = new FormUrlEncodedContent(new[]
                {
                    new KeyValuePair<string, string>("client_id", clientId),
                    new KeyValuePair<string, string>("client_secret", clientSecret),
                    new KeyValuePair<string, string>("grant_type", "authorization_code"),
                    new KeyValuePair<string, string>("redirect_uri", redirectUri),
                    new KeyValuePair<string, string>("code", code)
                })
            };

            var response = await httpClient.SendAsync(tokenRequest);
            var responseContent = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                return StatusCode((int)response.StatusCode, new { error = "Erro ao obter access_token", details = responseContent });
            }

            var tokenData = JsonSerializer.Deserialize<JsonElement>(responseContent);
            var accessToken = tokenData.GetProperty("access_token").GetString();
            var instagramUserId = tokenData.GetProperty("user_id").GetInt64().ToString();

            // Obtém o ID do usuário autenticado via JWT
            var userId = User.Identity?.Name ?? "anon";

            var token = new InstagramToken
            {
                UserId = userId,
                InstagramUserId = instagramUserId,
                AccessToken = accessToken
            };
            await _tokenRepo.SaveTokenAsync(token);

            return Ok(new { message = "Token salvo com sucesso", token });
        }
    }
}