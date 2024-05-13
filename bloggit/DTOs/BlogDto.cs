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

public class PaginateFilter
{
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;

    public string? SortingOption { get; set; } = "random";
}

public class PaginateResponse<T>
{
    public int PageNumber { get; set; }
    public int PageSize { get; set; }
    public int TotalPages { get; set; }
    public int TotalRecords { get; set; }
    public IEnumerable<T> Data { get; set; } = Enumerable.Empty<T>();
}