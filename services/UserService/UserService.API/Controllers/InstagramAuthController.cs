using Microsoft.AspNetCore.Mvc;
using UserService.Application.DTOs;
using UserService.Application.Services;
using UserService.Domain.Entities;
using UserService.Domain.Interfaces;
using ErrorOr;

namespace UserService.API.Controllers;

[ApiController]
[Route("api/[controller]/instagram")]
public class InstagramAuthController : ControllerBase
{
    private readonly IInstagramAuthService _instagramAuthService;
    private readonly ISocialAccountRepository _socialAccountRepository;

    public InstagramAuthController(IInstagramAuthService instagramAuthService, ISocialAccountRepository socialAccountRepository)
    {
        _instagramAuthService = instagramAuthService;
        _socialAccountRepository = socialAccountRepository;
    }

    [HttpGet("start/{userId:guid}")]
    public ActionResult<InstagramAuthStartResponseDto> Start(Guid userId)
    {
        var url = _instagramAuthService.GetAuthorizationUrl(userId);
        return Ok(new InstagramAuthStartResponseDto { AuthorizationUrl = url });
    }

    [HttpPost("callback")]
    public async Task<ActionResult<InstagramTokenResponseDto>> Callback([FromBody] InstagramAuthCallbackDto dto)
    {
        // O parâmetro state contém o userId
        if (!Guid.TryParse(dto.State, out var userId))
            return BadRequest("Invalid state/userId");

        var redirectUri = HttpContext.Request.Scheme + "://" + HttpContext.Request.Host + "/api/instagram/callback";
        var token = await _instagramAuthService.ExchangeCodeForTokenAsync(dto.Code, redirectUri);

        // Salva o token na tabela SocialAccount
        var account = new SocialAccount
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            NetworkType = SocialNetworkType.Instagram,
            AccessToken = token.AccessToken,
            Username = token.UserId
        };
        await _socialAccountRepository.AddAsync(account);

        return Ok(token);
    }
}
