namespace bloggit.Services.Service_Implements
{
    public class EmailMessage
    {
        public string To { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
        public List<string> AttachmentPaths { get; set; }
    }
}
