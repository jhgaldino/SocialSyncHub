using Microsoft.AspNetCore.Mvc;
using UserService.Application.Services;
using UserService.Application.DTOs;

namespace UserService.API.Controllers;

[ApiController]
[Route("api/[controller]/instagram")] 
public class InstagramSyncController : ControllerBase
{
    private readonly IInstagramSyncService _syncService;

    public InstagramSyncController(IInstagramSyncService syncService)
    {
        _syncService = syncService;
    }

    [HttpPost("sync/{userId:guid}")]
    public async Task<ActionResult<List<InstagramMediaDto>>> SyncUserMedia(Guid userId)
    {
        var result = await _syncService.SyncUserMediaAsync(userId);
        return Ok(result);
    }
}
