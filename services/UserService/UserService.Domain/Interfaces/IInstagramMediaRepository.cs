using UserService.Domain.Entities;

namespace UserService.Domain.Interfaces;

public interface IInstagramMediaRepository
{
    Task AddOrUpdateAsync(InstagramMedia media);
    Task<List<InstagramMedia>> GetByUserIdAsync(Guid userId);
}
