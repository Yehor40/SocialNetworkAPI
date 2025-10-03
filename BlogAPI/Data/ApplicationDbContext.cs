using BlogAPI.Models;
using Microsoft.EntityFrameworkCore;
using MySql.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace BlogAPI.Data;

public class ApplicationDbContext: DbContext
{
    public ApplicationDbContext(DbContextOptions options) : base(options)
    {
    }
        public DbSet<User> Users { get; set; }
        public DbSet<Article> Articles { get; set; }
        public DbSet<Comments> Comments { get; set; }

        /// <summary>
        /// Configures the model and seeds initial data into the database.
        /// </summary>
        /// <param name="modelBuilder">The builder being used to construct the model for this context.</param>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // --- Seed User Data ---
            var hashedPassword = BCrypt.Net.BCrypt.HashPassword("userpassword123");

            var user1 = new User
            {
                UserId = 1,
                Username = "john.doe",
                Email = "john.doe@example.com",
                PasswordHash = hashedPassword // Store the hashed password
            };

            var user2 = new User
            {
                UserId = 2,
                Username = "jane.smith",
                Email = "jane.smith@example.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("janepassword123")
            };

            modelBuilder.Entity<User>().HasData(user1, user2);

            // --- Seed Article Data ---
            var article1 = new Article
            {
                Id = 101,
                Title = "Introduction to Clean Architecture",
                Perex = "A brief overview of the principles and benefits of Clean Architecture.",
                Content = "Clean Architecture is a software design philosophy...",
                Timestamp = DateTime.UtcNow.AddDays(-7),
                UserId = user1.UserId // Link to john.doe
            };

            var article2 = new Article
            {
                Id = 102,
                Title = "Getting Started with ASP.NET Core",
                Perex = "A guide for beginners to set up their first ASP.NET Core project.",
                Content = "ASP.NET Core is a cross-platform, high-performance framework...",
                Timestamp = DateTime.UtcNow.AddDays(-5),
                UserId = user2.UserId // Link to jane.smith
            };

            modelBuilder.Entity<Article>().HasData(article1, article2);

            // --- Seed Comment Data ---
            var comment1 = new Comments
            {
                Id = 1001,
                Content = "Great article, very insightful!",
                Timestamp = DateTime.UtcNow.AddDays(-6),
                ArticleId = article1.Id, // Link to Article 1
                UserId = user2.UserId // Comment by jane.smith
            };

            var comment2 = new Comments
            {
                Id = 1002,
                Content = "Thanks for the guide, it helped me a lot.",
                Timestamp = DateTime.UtcNow.AddDays(-4),
                ArticleId = article2.Id, // Link to Article 2
                UserId = user1.UserId // Comment by john.doe
            };

            var comment3 = new Comments
            {
                Id = 1003,
                Content = "I have a question about the dependency rule.",
                Timestamp = DateTime.UtcNow.AddDays(-6).AddHours(2),
                ArticleId = article1.Id, // Link to Article 1
                UserId = user1.UserId // Comment by john.doe
            };

            modelBuilder.Entity<Comments>().HasData(comment1, comment2, comment3);
            
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
        }
        /// <summary>
        /// Design-time factory for ApplicationDbContext.
        /// This allows Entity Framework Core tools (like Add-Migration, Update-Database)
        /// to create an instance of ApplicationDbContext without requiring a running web host.
        /// </summary>
        public class ApplicationDbContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
        {
            public ApplicationDbContext CreateDbContext(string[] args)
            {
                var configuration = new ConfigurationBuilder()
                    .SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile("appsettings.json")
                    .Build();
        
                var connectionString = configuration.GetConnectionString("Database");

                var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
                optionsBuilder.UseMySQL(connectionString);        
                return new ApplicationDbContext(optionsBuilder.Options);
            }
        }
    }