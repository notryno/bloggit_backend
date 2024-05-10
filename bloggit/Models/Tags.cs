namespace bloggit.Models;

public class Tags : BaseModel
{
    public int Id { get; set; }
    public string Name { get; set; }
    public ICollection<Blogs> Blogs { get; set; }
}
