using InstagramService.API.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace InstagramService.API.Controllers
{
    [ApiController]
    [Route("api/instagram/profile")]
    public class InstagramProfileController : ControllerBase
    {
        private readonly InstagramTokenRepository _tokenRepo;
        public InstagramProfileController(InstagramTokenRepository tokenRepo)
        {
            _tokenRepo = tokenRepo;
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> GetProfile()
        {
            var userId = User.Identity?.Name ?? "anon";
            var token = await _tokenRepo.GetByUserIdAsync(userId);
            if (token == null)
                return NotFound(new { error = "Token do Instagram não encontrado para este usuário." });

            // Exemplo: buscar dados reais do Instagram
            using var httpClient = new HttpClient();
            var response = await httpClient.GetAsync($"https://graph.instagram.com/me?fields=id,username&access_token={token.AccessToken}");
            var content = await response.Content.ReadAsStringAsync();
            if (!response.IsSuccessStatusCode)
                return StatusCode((int)response.StatusCode, new { error = "Erro ao buscar dados do Instagram", details = content });

            var profile = JsonSerializer.Deserialize<JsonElement>(content);
            return Ok(profile);
        }
    }
}
