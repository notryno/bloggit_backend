namespace bloggit.Models
{
    public class Blogs : BaseModel
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Summary { get; set; }
        public string Content { get; set; }
        public string Author { get; set; }
        public string? Image { get; set; }
        public ApplicationUser User { get; set; }
        public ICollection<Comments> Comments { get; set; }
        public ICollection<Tags> Tags { get; set; }
        public ICollection<Reactions> Reaction { get; set; }

    }
}
