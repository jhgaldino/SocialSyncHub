using Microsoft.EntityFrameworkCore;
using UserService.Domain.Entities;

namespace UserService.Infrastructure.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<User> Users { get; set; }
    public DbSet<SocialAccount> SocialAccounts { get; set; }
    public DbSet<InstagramMedia> InstagramMedias { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Email).IsRequired().HasMaxLength(150);
            entity.HasIndex(e => e.Email).IsUnique();
            entity.Property(e => e.CreatedAt).IsRequired();
        });

        modelBuilder.Entity<SocialAccount>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.AccessToken).IsRequired();
            entity.Property(e => e.NetworkType).IsRequired();
            entity.HasOne(e => e.User)
                  .WithMany()
                  .HasForeignKey(e => e.UserId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<InstagramMedia>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.MediaId).IsRequired();
            entity.Property(e => e.MediaType).IsRequired();
            entity.Property(e => e.MediaUrl).IsRequired();
            entity.Property(e => e.Timestamp).IsRequired();
            entity.HasIndex(e => new { e.UserId, e.MediaId }).IsUnique();
        });
    }
} 