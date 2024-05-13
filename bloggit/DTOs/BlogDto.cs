namespace bloggit.DTOs;

public class BlogDto
{
    public int Id { get; set; }
    public string? Title { get; set; }
    public string? Summary { get; set; }
    public string? Content { get; set; }
    public string? Author { get; set; }
    
    public string? AuthorFirstName { get; set; }
    public string? AuthorLastName { get; set; }
    public string? AuthorUserName { get; set; }
    public string? Image { get; set; }
    
    public ICollection<string>? Tags { get; set; }
    public int ReactionCount { get; set; }
    //
    // public ICollection<CommentDto> Comments { get; set; }
    //
    public ICollection<ReactionDto> Reactions { get; set; }
    
    public DateTime? CreatedOn { get; set; }
    
    public DateTime? ModifiedOn { get; set; }
    
    public bool isLatest { get; set; }
    
    public bool isDeleted { get; set; }
}

public class BlogUpdateRequest
{
    public string? Title { get; set; }
    public string? Summary { get; set; }
    public string? Content { get; set; }
    public string? Author { get; set; }
    public string? Image { get; set; }
    
    public ICollection<string>? Tags { get; set; }
}