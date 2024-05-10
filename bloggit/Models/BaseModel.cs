namespace bloggit.Models
{
    public class BaseModel
    {
        public bool isLatest { get; set; }
        public bool isDeleted { get; set; }
        public DateTime? CreatedOn { get; set; }
        public DateTime? ModifiedOn { get; set; }
    }
}
