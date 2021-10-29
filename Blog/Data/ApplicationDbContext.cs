using Blog.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Blog.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Entities.Blog> Blogs { get; set; }
        public DbSet<Post> Posts { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<Tag> Tags { get; set; }
        public DbSet<ApplicationUser> ApplicationUsers { get; set; }
        public DbSet<BlogApplicationUser> BlogApplicationUser { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<Post>()
                .HasOne(i => i.Blog)
                .WithMany(c => c.Posts)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<Comment>()
                .HasOne(i => i.Post)
                .WithMany(c => c.Comments)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<BlogApplicationUser>()
                .HasKey(ba => new { ba.BlogId, ba.OwnerId });
            builder.Entity<BlogApplicationUser>()
                .HasOne(bc => bc.Blog)
                .WithMany(b => b.BlogApplicationUsers)
                .HasForeignKey(bc => bc.BlogId);
            builder.Entity<BlogApplicationUser>()
                .HasOne(bc => bc.Owner)
                .WithMany(c => c.BlogApplicationUsers)
                .HasForeignKey(bc => bc.OwnerId);
        }
    }
}