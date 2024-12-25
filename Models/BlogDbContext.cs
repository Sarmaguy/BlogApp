using Microsoft.EntityFrameworkCore;

namespace BlogApp.Models
{
    public class BlogDbContext : DbContext
    {
        public BlogDbContext(DbContextOptions<BlogDbContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<BlogPost> BlogPosts { get; set; }
        public DbSet<Comment> Comments { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<User>().ToTable("users");
            modelBuilder.Entity<BlogPost>().ToTable("blog_posts");
            modelBuilder.Entity<Comment>().ToTable("comments");

            modelBuilder.Entity<BlogPost>()
                .HasOne(p => p.User)
                .WithMany()
                .HasForeignKey(p => p.UserId);

            modelBuilder.Entity<Comment>()
                .HasOne(c => c.BlogPost)
                .WithMany()
                .HasForeignKey(c => c.BlogPostId);
        }
    }
}
