using Microsoft.EntityFrameworkCore;
using UserService.Domain.Entities;
using UserService.Domain.Interfaces;
using UserService.Infrastructure.Data;

namespace UserService.Infrastructure.Repositories;

public class InstagramMediaRepository : IInstagramMediaRepository
{
    private readonly AppDbContext _context;
    public InstagramMediaRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task AddOrUpdateAsync(InstagramMedia media)
    {
        var existing = await _context.Set<InstagramMedia>().FirstOrDefaultAsync(m => m.MediaId == media.MediaId && m.UserId == media.UserId);
        if (existing == null)
        {
            _context.Set<InstagramMedia>().Add(media);
        }
        else
        {
            existing.MediaType = media.MediaType;
            existing.MediaUrl = media.MediaUrl;
            existing.Caption = media.Caption;
            existing.Timestamp = media.Timestamp;
            _context.Set<InstagramMedia>().Update(existing);
        }
        await _context.SaveChangesAsync();
    }

    public async Task<List<InstagramMedia>> GetByUserIdAsync(Guid userId)
    {
        return await _context.Set<InstagramMedia>().Where(m => m.UserId == userId).ToListAsync();
    }
}
