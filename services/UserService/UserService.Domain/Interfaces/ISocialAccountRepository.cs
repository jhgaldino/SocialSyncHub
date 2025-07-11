using UserService.Domain.Entities;

namespace UserService.Domain.Interfaces;

public interface ISocialAccountRepository
{
    Task<List<SocialAccount>> GetByUserIdAsync(Guid userId);
    Task<SocialAccount?> GetByIdAsync(Guid id);
    Task AddAsync(SocialAccount account);
    Task RemoveAsync(Guid id);
    Task<SocialAccount?> GetByUserAndNetworkAsync(Guid userId, SocialNetworkType networkType);
}
