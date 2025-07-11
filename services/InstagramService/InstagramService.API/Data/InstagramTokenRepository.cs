using System.Threading.Tasks;
using InstagramService.API.Data;
using Microsoft.EntityFrameworkCore;

namespace InstagramService.API.Data
{
    public class InstagramTokenRepository
    {
        private readonly InstagramDbContext _context;
        public InstagramTokenRepository(InstagramDbContext context)
        {
            _context = context;
        }

        public async Task SaveTokenAsync(InstagramToken token)
        {
            _context.InstagramTokens.Add(token);
            await _context.SaveChangesAsync();
        }

        public async Task<InstagramToken> GetByUserIdAsync(string userId)
        {
            return await _context.InstagramTokens.FirstOrDefaultAsync(t => t.UserId == userId);
        }
    }
}
