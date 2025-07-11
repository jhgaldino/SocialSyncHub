using UserService.Application.DTOs;

namespace UserService.Application.Services;

public interface IInstagramSyncService
{
    Task<List<InstagramMediaDto>> SyncUserMediaAsync(Guid userId);
}
