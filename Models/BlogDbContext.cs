using Microsoft.EntityFrameworkCore;

namespace BlogApp.Models
{
    public class BlogDbContext : DbContext
    {
        public BlogDbContext(DbContextOptions<BlogDbContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Map the User entity to the "users" table (case-sensitive match)
            modelBuilder.Entity<User>().ToTable("users");
        }
    }
}
