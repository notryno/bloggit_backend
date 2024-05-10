namespace bloggit.Models;

public class Reactions: BaseModel
{
    public int Id { get; set; }
    public string Type { get; set; }

    public int? BlogId { get; set; }
    public Blogs Blog { get; set; }

    public int? CommentId { get; set; }
    public Comments Comment { get; set; }

    public string UserId { get; set; }
    public ApplicationUser User { get; set; }
}
