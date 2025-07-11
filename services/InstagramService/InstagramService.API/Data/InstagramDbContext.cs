using Microsoft.EntityFrameworkCore;

namespace InstagramService.API.Data
{
    public class InstagramDbContext : DbContext
    {
        public InstagramDbContext(DbContextOptions<InstagramDbContext> options) : base(options) { }

        public DbSet<InstagramToken> InstagramTokens { get; set; }
    }
}
