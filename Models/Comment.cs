using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BlogApp.Models
{
    public class Comment
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [Column("text")] 
        public string Content { get; set; }

        [Column("created_at")] 
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        [Column("updated_at")] 
        public DateTime? UpdatedAt { get; set; }

        [Column("user_id")] 
        public int? UserId { get; set; } 
        public User? User { get; set; }

        [Column("blogpost_id")] 
        public int BlogPostId { get; set; }

        public BlogPost BlogPost { get; set; } 
    }
}
