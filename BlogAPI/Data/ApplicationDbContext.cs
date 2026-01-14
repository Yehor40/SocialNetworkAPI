using BlogAPI.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace BlogAPI.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }

    public DbSet<User> Users { get; set; } = null!;
    public DbSet<Article> Articles { get; set; } = null!;
    public DbSet<Comment> Comments { get; set; } = null!;
    public DbSet<Vote> Votes { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // --- Seed User Data ---
        var user1 = new User
        {
            UserId = 1,
            Username = "admin",
            Email = "admin@example.com",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("Password123!") // In production, use a more secure way to handle secrets
        };

        modelBuilder.Entity<User>().HasData(user1);

        // --- Seed Article Data ---
        var article1 = new Article
        {
            Id = 1,
            Title = "Welcome to my Blog",
            Perex = "This is the first post on my new blogging engine.",
            Content = "I built this blog using .NET 7, PostgreSQL, and GraphQL. Stay tuned for more updates!",
            Timestamp = DateTime.UtcNow,
            UserId = 1
        };

        modelBuilder.Entity<Article>().HasData(article1);

        // --- Relationships ---
        modelBuilder.Entity<User>()
            .HasMany(u => u.Articles)
            .WithOne(a => a.User)
            .HasForeignKey(a => a.UserId);

        modelBuilder.Entity<User>()
            .HasMany(u => u.Comments)
            .WithOne(c => c.User)
            .HasForeignKey(c => c.UserId);

        modelBuilder.Entity<Article>()
            .HasMany(a => a.Comments)
            .WithOne(c => c.Article)
            .HasForeignKey(c => c.ArticleId);

        modelBuilder.Entity<Comment>()
            .HasMany(c => c.Votes)
            .WithOne(v => v.Comment)
            .HasForeignKey(v => v.CommentId);
            
        // Unique vote per IP per comment
        modelBuilder.Entity<Vote>()
            .HasIndex(v => new { v.CommentId, v.IpAddress })
            .IsUnique();
    }
}

public class ApplicationDbContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
{
    public ApplicationDbContext CreateDbContext(string[] args)
    {
        DotNetEnv.Env.Load();
        var connectionString = Environment.GetEnvironmentVariable("DefaultConnection");

        var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
        optionsBuilder.UseNpgsql(connectionString);
        return new ApplicationDbContext(optionsBuilder.Options);
    }
}