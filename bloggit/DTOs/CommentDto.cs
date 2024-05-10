namespace bloggit.DTOs
{
    public class CommentDto
    {
        public int Id { get; set; }
        public string Content { get; set; }
        public int BlogId { get; set; }
        public string UserId { get; set; }
        public int? ReplyId { get; set; }
        public DateTime? CreatedOn { get; set; }
        public DateTime? ModifiedOn { get; set; }
        public List<ReactionDto> Reactions { get; set; }
    }

    public class CreateCommentDto
    {
        public string Content { get; set; }
        public int BlogId { get; set; }
        public string UserId { get; set; }
        public int? ReplyId { get; set; }
    }

    public class UpdateCommentDto
    {
        public int Id { get; set; }
        public string Content { get; set; }
    }
}