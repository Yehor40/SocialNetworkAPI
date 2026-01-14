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
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("Password123!")
        };

        var user2 = new User
        {
            UserId = 2,
            Username = "john_doe",
            Email = "john@example.com",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("Password123!")
        };

        var user3 = new User
        {
            UserId = 3,
            Username = "jane_smith",
            Email = "jane@example.com",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("Password123!")
        };

        modelBuilder.Entity<User>().HasData(user1, user2, user3);

        // --- Seed Article Data ---
        var article1 = new Article
        {
            Id = 1,
            Title = "Welcome to my Blog",
            Perex = "This is the first post on my new blogging engine.",
            Content = "I built this blog using .NET 8, PostgreSQL, and GraphQL. Stay tuned for more updates!",
            Timestamp = new DateTime(2026, 1, 1, 12, 0, 0, DateTimeKind.Utc),
            UserId = 1
        };

        var article2 = new Article
        {
            Id = 2,
            Title = "Getting Started with GraphQL",
            Perex = "Learn the basics of GraphQL and how to integrate it with .NET.",
            Content = "GraphQL is a powerful query language for APIs. In this article, we'll explore how to set up a GraphQL server using HotChocolate in .NET 8.",
            Timestamp = new DateTime(2026, 1, 5, 14, 30, 0, DateTimeKind.Utc),
            UserId = 2
        };

        var article3 = new Article
        {
            Id = 3,
            Title = "PostgreSQL Best Practices",
            Perex = "Tips and tricks for optimizing your PostgreSQL database.",
            Content = "PostgreSQL is a robust relational database. Here are some best practices for indexing, query optimization, and maintaining data integrity.",
            Timestamp = new DateTime(2026, 1, 10, 9, 15, 0, DateTimeKind.Utc),
            UserId = 3
        };

        modelBuilder.Entity<Article>().HasData(article1, article2, article3);

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