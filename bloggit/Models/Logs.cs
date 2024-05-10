namespace bloggit.Models
{
    public class Logs
    {
        public int Id { get; set; }
        public int? BlogId { get; set; }
        public int? CommentId { get; set; }
        public string UserId { get; set; }
        public string Type { get; set; }
        public DateTime Timestamp { get; set; }
        public string Description { get; set; }
        public ApplicationUser User { get; set; }
        public Blogs Blog { get; set; }
        public Comments Comment { get; set; }
    }
}