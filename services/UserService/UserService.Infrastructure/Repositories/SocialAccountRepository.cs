using Microsoft.EntityFrameworkCore;
using UserService.Domain.Entities;
using UserService.Domain.Interfaces;
using UserService.Infrastructure.Data;

namespace UserService.Infrastructure.Repositories;

public class SocialAccountRepository : ISocialAccountRepository
{
    private readonly AppDbContext _context;
    public SocialAccountRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<SocialAccount>> GetByUserIdAsync(Guid userId)
    {
        return await _context.SocialAccounts.Where(a => a.UserId == userId).ToListAsync();
    }

    public async Task<SocialAccount?> GetByIdAsync(Guid id)
    {
        return await _context.SocialAccounts.FindAsync(id);
    }

    public async Task AddAsync(SocialAccount account)
    {
        _context.SocialAccounts.Add(account);
        await _context.SaveChangesAsync();
    }

    public async Task RemoveAsync(Guid id)
    {
        var entity = await _context.SocialAccounts.FindAsync(id);
        if (entity != null)
        {
            _context.SocialAccounts.Remove(entity);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<SocialAccount?> GetByUserAndNetworkAsync(Guid userId, SocialNetworkType networkType)
    {
        return await _context.SocialAccounts.FirstOrDefaultAsync(a => a.UserId == userId && a.NetworkType == networkType);
    }
}
