    namespace bloggit.Models;

    public class Comments : BaseModel
    {
        public int Id { get; set; }
        public string Content { get; set; }
        public int BlogId { get; set; }
        public string UserId { get; set; }
        public int? ReplyId { get; set; }
        public Blogs Blog { get; set; }
        public ApplicationUser User { get; set; }
        
        public Comments ReplyToComment { get; set; }
        public ICollection<Comments> Replies { get; set; }
        public ICollection<Reactions> Reaction { get; set; }
    }