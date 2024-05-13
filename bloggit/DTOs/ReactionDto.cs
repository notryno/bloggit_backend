namespace bloggit.DTOs
{
    public class ReactionDto
    {
        public int Id { get; set; }
        public string Type { get; set; }
        public int? BlogId { get; set; }
        public int? CommentId { get; set; }
        public string UserId { get; set; }
    }

    public class CreateReactionDto
    {
        public string Type { get; set; }
        public int? BlogId { get; set; }
        public int? CommentId { get; set; }
        public string UserId { get; set; }
    }

    public class UpdateReactionDto
    {
        public string Type { get; set; }
    }
    
    public class ReactionCountDto
    {
        public int UpvoteCount { get; set; }
        public int DownvoteCount { get; set; }
    }
}