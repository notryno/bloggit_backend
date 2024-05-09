namespace bloggit.Models
{
    public class Blogs : BaseModel
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public string Tags { get; set; }
        public string Author { get; set; }
    }
}
